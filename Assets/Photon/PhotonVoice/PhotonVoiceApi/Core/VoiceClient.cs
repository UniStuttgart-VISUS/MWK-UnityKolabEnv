// -----------------------------------------------------------------------
// <copyright file="VoiceClient.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
//#define PHOTON_VOICE_VIDEO_ENABLE
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
#if NETFX_CORE
using Windows.System.Threading;
#endif
namespace Photon.Voice
{
    enum EventSubcode : byte
    {
        VoiceInfo = 1,
        VoiceRemove = 2,
        Frame = 3,
    }
    enum EventParam : byte
    {
        VoiceId = 1,
        SamplingRate = 2,
        Channels = 3,
        FrameDurationUs = 4,
        Bitrate = 5,
        UserData = 10,
        EventNumber = 11,
        Codec = 12,
    }
    public interface ILogger
    {
        void LogError(string fmt, params object[] args);
        void LogWarning(string fmt, params object[] args);
        void LogInfo(string fmt, params object[] args);
        void LogDebug(string fmt, params object[] args);
    }
    interface IVoiceTransport : ILogger
    {
        int AssignChannel(VoiceInfo v);
        bool IsChannelJoined(int channelId);
        void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId);
        void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId);
        void SendFrame(ArraySegment<byte> data, byte evNumber, byte voiceId, int channelId, LocalVoice localVoice);
        string ChannelIdStr(int channelId);
        string PlayerIdStr(int playerId);
        void SetDebugEchoMode(LocalVoice v);
    }
    /// <summary>
    /// Base class for Voice clients implamantations
    /// </summary>        
    public class VoiceClient : IDisposable
    {
        internal IVoiceTransport transport;
        /// <summary>Lost frames counter.</summary>
        public int FramesLost { get; internal set; }
        /// <summary>Received frames counter.</summary>
        public int FramesReceived { get; private set; }
        /// <summary>Sent frames counter.</summary>
        public int FramesSent { get { int x = 0; foreach (var v in this.localVoices) { x += v.Value.FramesSent; } return x; } }
        /// <summary>Sent frames bytes counter.</summary>
        public int FramesSentBytes { get { int x = 0; foreach (var v in this.localVoices) { x += v.Value.FramesSentBytes; } return x; } }
        /// <summary>Average time required voice packet to return to sender.</summary>
        public int RoundTripTime { get; private set; }
        /// <summary>Average round trip time variation.</summary>
        public int RoundTripTimeVariance { get; private set; }
        /// <summary>Do not log warning when duplicate info received.</summary>
        public bool SuppressInfoDuplicateWarning { get; set; }
        /// <summary>Remote voice info event delegate.</summary>        
        public delegate void RemoteVoiceInfoDelegate(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options);
        /// <summary>
        /// Register a method to be called when remote voice info arrived (after join or new new remote voice creation).
        /// Metod parameters: (int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options);
        /// </summary>
        public RemoteVoiceInfoDelegate OnRemoteVoiceInfoAction { get; set; }
        /// <summary>Lost frames simulation ratio.</summary>
        public int DebugLostPercent { get; set; }
        private int prevRtt = 0;
        /// <summary>Iterates through copy of all local voices list.</summary>
        public IEnumerable<LocalVoice> LocalVoices
        {
            get
            {
                var res = new LocalVoice[this.localVoices.Count];
                this.localVoices.Values.CopyTo(res, 0);
                return res;
            }
        }
        /// <summary>Iterates through copy of all local voices list of given channel.</summary>
        public IEnumerable<LocalVoice> LocalVoicesInChannel(int channelId)
        {
            List<LocalVoice> channelVoices;
            if (this.localVoicesPerChannel.TryGetValue(channelId, out channelVoices))
            {
                var res = new LocalVoice[channelVoices.Count];
                channelVoices.CopyTo(res, 0);
                return res;
            }
            else
            {
                return new LocalVoice[0];
            }
        }
        /// <summary>Iterates through all remote voices infos.</summary>
        public IEnumerable<RemoteVoiceInfo> RemoteVoiceInfos
        {
            get
            {
                foreach (var channelVoices in this.remoteVoices)
                {
                    foreach (var playerVoices in channelVoices.Value)
                    {
                        foreach (var voice in playerVoices.Value)
                        {
                            yield return new RemoteVoiceInfo(channelVoices.Key, playerVoices.Key, voice.Key, voice.Value.Info);
                        }
                    }
                }
            }
        }
        /// <summary>Creates VoiceClient instance</summary>
        internal VoiceClient(IVoiceTransport transport)
        {
            this.transport = transport;
        }
        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// Call this method regularly (2..20 times a second).
        /// </summary>
        public void Service()
        {
            foreach (var v in localVoices)
            {
                v.Value.service();
            }
        }
        private LocalVoice createLocalVoice(VoiceInfo voiceInfo, int channelId, Func<byte, int, LocalVoice> voiceFactory)
        {
            if (channelId == ChannelAuto)
            {
                channelId = this.transport.AssignChannel(voiceInfo);
            }
            var newId = getNewVoiceId();
            if (newId != 0)
            {
                LocalVoice v = voiceFactory(newId, channelId);
                if (v != null)
                {
                    addVoice(newId, channelId, v);
                    this.transport.LogInfo(v.LogPrefix + " added enc: " + v.info.ToString());
                    return v;
                }
            }
            return null;
        }
        public const int ChannelAuto = -1; // any number not used as channel id in transports
        /// <summary>
        /// Creates basic outgoing stream w/o data processing support. Provided encoder should generate output data stream.
        /// </summary>
        /// <param name="voiceInfo">Outgoing stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport. Set to VoiceClient.ChannelAuto to let transport automatically assign channel.</param>
        /// <param name="encoder">Encoder producing the stream.</param>
        /// <returns>Outgoing stream handler.</returns>
        public LocalVoice CreateLocalVoice(VoiceInfo voiceInfo, int channelId = ChannelAuto, IEncoder encoder = null)
        {
            return (LocalVoice)createLocalVoice(voiceInfo, channelId, (vId, chId) => new LocalVoice(this, encoder, vId, voiceInfo, chId));
        }
        /// <summary>
        /// Creates outgoing stream consuming sequence of values passed in array buffers of arbitrary length which repacked in frames of constant length for further processing and encoding.
        /// </summary>
        /// <typeparam name="T">Type of data consumed by outgoing stream (element type of array buffers).</typeparam>
        /// <param name="voiceInfo">Outgoing stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport. Set to VoiceClient.ChannelAuto to let transport automatically assign channel.</param>
        /// <param name="encoder">Encoder compressing data stream in pipeline.</param>
        /// <returns>Outgoing stream handler.</returns>
        public LocalVoiceFramed<T> CreateLocalVoiceFramed<T>(VoiceInfo voiceInfo, int frameSize, int channelId = ChannelAuto, IEncoderDataFlow<T> encoder = null)
        {
            return (LocalVoiceFramed<T>)createLocalVoice(voiceInfo, channelId, (vId, chId) => new LocalVoiceFramed<T>(this, encoder, vId, voiceInfo, chId, frameSize));
        }
        /// <summary>
        /// Creates outgoing audio stream. Adds audio specific features (e.g. resampling, level meter) to processing pipeline and to returning stream handler.
        /// </summary>
        /// <typeparam name="T">Element type of audio array buffers.</typeparam>
        /// <param name="voiceInfo">Outgoing audio stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport. Set to VoiceClient.ChannelAuto to let transport automatically assign channel.</param>
        /// <param name="encoder">Audio encoder. Set to null to use default Opus encoder.</param>
        /// <returns>Outgoing stream handler.</returns>
        /// <remarks>
        /// voiceInfo.sourceSamplingRate and voiceInfo.SamplingRate may do not match. Automatic resampling will occur in this case.
        /// </remarks>
        public LocalVoiceAudio<T> CreateLocalVoiceAudio<T>(VoiceInfo voiceInfo, int channelId = ChannelAuto, IEncoder encoder = null)
        {
            return (LocalVoiceAudio<T>)createLocalVoice(voiceInfo, channelId, (vId, chId) => LocalVoiceAudio<T>.Create(this, vId, encoder, voiceInfo, chId));
        }
        /// <summary>
        /// Creates outgoing audio stream of type automatically assigned and adds procedures (callback or serviceable) for consuming given audio source data.
        /// Adds audio specific features (e.g. resampling, level meter) to processing pipeline and to returning stream handler.
        /// </summary>
        /// <param name="voiceInfo">Outgoing audio stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="source">Streaming audio source.</param>
        /// <param name="forceShort">For audio sources producing buffers of 'float' type, creates stream of 'short' type and adds converter.</param>
        /// <param name="channelId">Transport channel specific to transport. Set to VoiceClient.ChannelAuto to let transport automatically assign channel.</param>
        /// <param name="encoder">Audio encoder. Set to null to use default Opus encoder.</param>
        /// <returns>Outgoing stream handler.</returns>
        /// <remarks>
        /// voiceInfo.sourceSamplingRate and voiceInfo.SamplingRate may do not match. Automatic resampling will occur in this case.
        /// </remarks>
        public Voice.LocalVoice CreateLocalVoiceAudioFromSource(Voice.VoiceInfo voiceInfo, Voice.IAudioDesc source, bool forceShort = false, int channelId = ChannelAuto, IEncoder encoder = null)
        {
            if (source is Voice.IAudioPusher<float>)
            {
                if (forceShort)
                {
                    var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, channelId, encoder);                    
                    // we can safely reuse the same buffer in callbacks from native code
                    // 
                    var bufferFactory = new FactoryReusableArray<float>(0);
                    ((Voice.IAudioPusher<float>)source).SetCallback(buf => {
                        var shortBuf = localVoice.BufferFactory.New(buf.Length);
                        AudioUtil.Convert(buf, shortBuf, buf.Length);
                        localVoice.PushDataAsync(shortBuf);
                    }, bufferFactory);
                    return localVoice;
                }
                else
                {
                    var localVoice = CreateLocalVoiceAudio<float>(voiceInfo, channelId, encoder);
                    ((Voice.IAudioPusher<float>)source).SetCallback(buf => localVoice.PushDataAsync(buf), localVoice.BufferFactory);
                    return localVoice;
                }
            }
            else if (source is Voice.IAudioPusher<short>)
            {
                var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, channelId, encoder);
                ((Voice.IAudioPusher<short>)source).SetCallback(buf => localVoice.PushDataAsync(buf), localVoice.BufferFactory);
                return localVoice;
            }
            else if (source is Voice.IAudioReader<float>)
            {
                if (forceShort)
                {
                    transport.LogInfo("[PV] Creating local voice with source samples type conversion from float to short.");
                    var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, channelId, encoder);
                    localVoice.LocalUserServiceable = new Voice.BufferReaderPushAdapterAsyncPoolFloatToShort(localVoice, source as Voice.IAudioReader<float>);
                    return localVoice;
                }
                else
                {
                    var localVoice = CreateLocalVoiceAudio<float>(voiceInfo, channelId, encoder);
                    localVoice.LocalUserServiceable = new Voice.BufferReaderPushAdapterAsyncPool<float>(localVoice, source as Voice.IAudioReader<float>);
                    return localVoice;
                }
            }
            else if (source is Voice.IAudioReader<short>)
            {
                var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, channelId, encoder);
                localVoice.LocalUserServiceable = new Voice.BufferReaderPushAdapterAsyncPool<short>(localVoice, source as Voice.IAudioReader<short>);
                return localVoice;
            }
            else
            {
                transport.LogError("[PV] CreateLocalVoiceAudioFromSource does not support Voice.IAudioDesc of type {0}", source.GetType());
                return Voice.LocalVoiceAudioDummy.Dummy;
            }
        }
#if PHOTON_VOICE_VIDEO_ENABLE
        /// <summary>
        /// Creates outgoing video stream consuming sequence of image buffers.
        /// </summary>
        /// <param name="voiceInfo">Outgoing stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport. Set to VoiceClient.ChannelAuto to let transport automatically assign channel.</param>
        /// <param name="encoder">Encoder compressing video data. Set to null to use default VP8 implementation.</param>
        /// <returns>Outgoing stream handler.</returns>
        public LocalVoiceVideo CreateLocalVoiceVideo(VoiceInfo voiceInfo, int channelId = ChannelAuto, IEncoder encoder = null)
        {
            return (LocalVoiceVideo)createLocalVoice(voiceInfo, channelId, (vId, chId) => new LocalVoiceVideo(this, encoder, vId, voiceInfo, chId));
        }
#endif
        private byte getNewVoiceId()
        {
            // id assigned starting from 1 and up to 255
            byte newId = 0; // non-zero if successfully assigned
            if (voiceIdCnt == 255)
            {
                // try to reuse id
                var ids = new bool[256];
                foreach (var v in localVoices)
                {
                    ids[v.Value.id] = true;
                }
                // ids[0] is not used
                for (byte id = 1; id != 0 /* < 256 */ ; id++)
                {
                    if (!ids[id])
                    {
                        newId = id;
                        break;
                    }
                }
            }
            else
            {
                voiceIdCnt++;
                newId = voiceIdCnt;
            }
            return newId;
        }
        void addVoice(byte newId, int channelId, LocalVoice v)
        {
            localVoices[newId] = v;
            List<LocalVoice> voiceList;
            if (!localVoicesPerChannel.TryGetValue(channelId, out voiceList))
            {
                voiceList = new List<LocalVoice>();
                localVoicesPerChannel[channelId] = voiceList;
            }
            voiceList.Add(v);
            if (this.transport.IsChannelJoined(channelId))
            {
                this.transport.SendVoicesInfo(new List<LocalVoice>() { v }, channelId, 0); // broadcast if joined
            }
            v.InterestGroup = this.GlobalInterestGroup;
        }
        /// <summary>
        /// Removes local voice (outgoing data stream).
        /// <param name="voice">Handler of outgoing stream to be removed.</param>
        /// </summary>
        public void RemoveLocalVoice(LocalVoice voice)
        {
            this.localVoices.Remove(voice.id);
            this.localVoicesPerChannel[voice.channelId].Remove(voice);
            if (this.transport.IsChannelJoined(voice.channelId))
            {
                this.transport.SendVoiceRemove(voice, voice.channelId, 0);
            }
            voice.Dispose();
            this.transport.LogInfo(voice.LogPrefix + " removed");
        }
        internal void sendVoicesInfo(int targetPlayerId)
        {
            foreach (var ch in this.localVoicesPerChannel.Keys)
            {
                sendChannelVoicesInfo(ch, targetPlayerId);
            }
        }
        internal void sendChannelVoicesInfo(int channelId, int targetPlayerId)
        {
            if (this.transport.IsChannelJoined(channelId))
            {
                List<LocalVoice> voiceList;
                if (this.localVoicesPerChannel.TryGetValue(channelId, out voiceList))
                {
                    this.transport.SendVoicesInfo(voiceList, channelId, targetPlayerId);
                }
            }
        }
        internal void onVoiceEvent(object content0, int channelId, int playerId, int localPlayerId)
        {
            object[] content = (object[])content0;
            if ((byte)content[0] == (byte)0)
            {
                switch ((byte)content[1])
                {
                    case (byte)EventSubcode.VoiceInfo:
                        this.onVoiceInfo(channelId, playerId, content[2]);
                        break;
                    case (byte)EventSubcode.VoiceRemove:
                        this.onVoiceRemove(channelId, playerId, content[2]);
                        break;
                    default:
                        this.transport.LogError("[PV] Unknown sevent subcode " + content[1]);
                        break;
                }
            }
            else
            {
                byte voiceId = (byte)content[0];
                byte evNumber = (byte)content[1];
                byte[] receivedBytes = (byte[])content[2];
                if (playerId == localPlayerId)
                {
                    LocalVoice voice;
                    if (this.localVoices.TryGetValue(voiceId, out voice))
                    {
                        int sendTime;
                        if (voice.eventTimestamps.TryGetValue(evNumber, out sendTime))
                        {
                            int rtt = Environment.TickCount - sendTime;
                            int rttvar = rtt - prevRtt;
                            prevRtt = rtt;
                            if (rttvar < 0) rttvar = -rttvar;
                            this.RoundTripTimeVariance = (rttvar + RoundTripTimeVariance * 19) / 20;
                            this.RoundTripTime = (rtt + RoundTripTime * 19) / 20;
                        }
                    }
                    //internal Dictionary<byte, DateTime> localEventTimestamps = new Dictionary<byte, DateTime>();
                }
                this.onFrame(channelId, playerId, voiceId, evNumber, receivedBytes);
            }
        }
        internal byte GlobalInterestGroup
        {
            get { return this.globalInterestGroup; }
            set
            {
                this.globalInterestGroup = value;
                foreach (var v in this.localVoices)
                {
                    v.Value.InterestGroup = this.globalInterestGroup;
                }
            }
        }
        #region nonpublic
        private byte globalInterestGroup;
        private byte voiceIdCnt = 0;
        private Dictionary<byte, LocalVoice> localVoices = new Dictionary<byte, LocalVoice>();
        private Dictionary<int, List<LocalVoice>> localVoicesPerChannel = new Dictionary<int, List<LocalVoice>>();
        // channel id -> player id -> voice id -> voice
        private Dictionary<int, Dictionary<int, Dictionary<byte, RemoteVoice>>> remoteVoices = new Dictionary<int, Dictionary<int, Dictionary<byte, RemoteVoice>>>();
        internal object[] buildVoicesInfo(IEnumerable<LocalVoice> voicesToSend, bool logInfo)
        {
            object[] infos = new object[voicesToSend.Count()];
            object[] content = new object[] { (byte)0, EventSubcode.VoiceInfo, infos };
            int i = 0;
            foreach (var v in voicesToSend)
            {
                infos[i] = new Dictionary<byte, object>() {
                    { (byte)EventParam.VoiceId, v.id },
                    { (byte)EventParam.Codec, v.info.Codec },
                    { (byte)EventParam.SamplingRate, v.info.SamplingRate },
                    { (byte)EventParam.Channels, v.info.Channels },
                    { (byte)EventParam.FrameDurationUs, v.info.FrameDurationUs },
                    { (byte)EventParam.Bitrate, v.info.Bitrate },
                    { (byte)EventParam.UserData, v.info.UserData },
                    { (byte)EventParam.EventNumber, v.evNumber }
                };
                i++;
                if (logInfo)
                {
                    this.transport.LogInfo(v.LogPrefix + " Sending info: " + v.info.ToString() + " ev=" + v.evNumber);
                }
            }
            return content;
        }
        internal object[] buildVoiceRemoveMessage(LocalVoice v)
        {
            byte[] ids = new byte[] { v.id };
            object[] content = new object[] { (byte)0, EventSubcode.VoiceRemove, ids };
            this.transport.LogInfo(v.LogPrefix + " remove sent");
            return content;
        }
        internal void clearRemoteVoices()
        {
            foreach (var channelVoices in remoteVoices)
            {
                foreach (var playerVoices in channelVoices.Value)
                {
                    foreach (var voice in playerVoices.Value)
                    {
                        voice.Value.removeAndDispose();
                    }
                }
            }
            remoteVoices.Clear();
            this.transport.LogInfo("[PV] Remote voices cleared");
        }
        internal void clearRemoteVoicesInChannel(int channelId)
        {
            Dictionary<int, Dictionary<byte, RemoteVoice>> channelVoices = null;
            if (this.remoteVoices.TryGetValue(channelId, out channelVoices))
            {
                foreach (var playerVoices in channelVoices)
                {
                    foreach (var voice in playerVoices.Value)
                    {
                        voice.Value.removeAndDispose();
                    }
                }
                this.remoteVoices.Remove(channelId);
            }
            this.transport.LogInfo("[PV] Remote voices for channel " + this.channelStr(channelId) + " cleared");
        }
        private void onVoiceInfo(int channelId, int playerId, object payload)
        {
            Dictionary<int, Dictionary<byte, RemoteVoice>> channelVoices = null;
            if (!this.remoteVoices.TryGetValue(channelId, out channelVoices))
            {
                channelVoices = new Dictionary<int, Dictionary<byte, RemoteVoice>>();
                this.remoteVoices[channelId] = channelVoices;
            }
            Dictionary<byte, RemoteVoice> playerVoices = null;
            if (!channelVoices.TryGetValue(playerId, out playerVoices))
            {
                playerVoices = new Dictionary<byte, RemoteVoice>();
                channelVoices[playerId] = playerVoices;
            }
            foreach (var el in (object[])payload)
            {
                var h = (Dictionary<byte, Object>)el;
                var voiceId = (byte)h[(byte)EventParam.VoiceId];
                if (!playerVoices.ContainsKey(voiceId))
                {
                    var eventNumber = (byte)h[(byte)EventParam.EventNumber];
                    var info = VoiceInfo.CreateFromEventPayload(h);
                    this.transport.LogInfo("[PV] ch#" + this.channelStr(channelId) + " p#" + this.playerStr(playerId) + " v#" + voiceId + " Info received: " + info.ToString() + " ev=" + eventNumber);
                    // create default decoder                   
                    RemoteVoiceOptions options = new RemoteVoiceOptions();
                    // create default decoder
                    // may be overwritten in OnRemoteVoiceInfoAction call
                    options.Decoder = VoiceCodec.CreateDefaultDecoder(channelId, playerId, voiceId, info, this.transport);
                    if (this.OnRemoteVoiceInfoAction != null)
                    {
                        this.OnRemoteVoiceInfoAction(channelId, playerId, voiceId, info, ref options);
                    }
                    playerVoices[voiceId] = new RemoteVoice(this, options, channelId, playerId, voiceId, info, eventNumber);
                }
                else
                {
                    if (!this.SuppressInfoDuplicateWarning)
                    {
                        this.transport.LogWarning("[PV] Info duplicate for voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId));
                    }
                }
            }
        }
        private void onVoiceRemove(int channelId, int playerId, object payload)
        {
            var voiceIds = (byte[])payload;
            Dictionary<int, Dictionary<byte, RemoteVoice>> channelVoices = null;
            if (this.remoteVoices.TryGetValue(channelId, out channelVoices))
            {
                Dictionary<byte, RemoteVoice> playerVoices = null;
                if (channelVoices.TryGetValue(playerId, out playerVoices))
                {
                    foreach (var voiceId in voiceIds)
                    {
                        RemoteVoice voice;
                        if (playerVoices.TryGetValue(voiceId, out voice))
                        {
                            playerVoices.Remove(voiceId);
                            this.transport.LogInfo("[PV] Remote voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId) + " removed");
                            voice.removeAndDispose();
                        }
                        else
                        {
                            this.transport.LogWarning("[PV] Remote voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId) + " not found when trying to remove");
                        }
                    }
                }
                else
                {
                    this.transport.LogWarning("[PV] Remote voice list of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId) + " not found when trying to remove voice(s)");
                }
            }
            else
            {
                this.transport.LogWarning("[PV] Remote voice list of channel " + this.channelStr(channelId) + " not found when trying to remove voice(s)");
            }
        }
        Random rnd = new Random();
        private void onFrame(int channelId, int playerId, byte voiceId, byte evNumber, byte[] receivedBytes)
        {
            if (this.DebugLostPercent > 0 && rnd.Next(100) < this.DebugLostPercent)
            {
                this.transport.LogWarning("[PV] Debug Lost Sim: 1 packet dropped");
                return;
            }
            FramesReceived++;
            Dictionary<int, Dictionary<byte, RemoteVoice>> channelVoices = null;
            if (this.remoteVoices.TryGetValue(channelId, out channelVoices))
            {
                Dictionary<byte, RemoteVoice> playerVoices = null;
                if (channelVoices.TryGetValue(playerId, out playerVoices))
                {
                    RemoteVoice voice = null;
                    if (playerVoices.TryGetValue(voiceId, out voice))
                    {
                        voice.receiveBytes(receivedBytes, evNumber);
                    }
                    else
                    {
                        this.transport.LogWarning("[PV] Frame event for not inited voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId));
                    }
                }
                else
                {
                    this.transport.LogWarning("[PV] Frame event for voice #" + voiceId + " of not inited player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId));
                }
            }
            else
            {
                this.transport.LogWarning("[PV] Frame event for voice #" + voiceId + " of not inited channel " + this.channelStr(channelId));
            }
        }
        internal bool removePlayerVoices(int playerId)
        {
            foreach (var ch in this.remoteVoices.Keys)
            {
                removePlayerVoices(ch, playerId);
            }
            return true;
        }
        internal bool removePlayerVoices(int channelId, int playerId)
        {
            Dictionary<int, Dictionary<byte, RemoteVoice>> channelVoices = null;
            if (this.remoteVoices.TryGetValue(channelId, out channelVoices))
            {
                Dictionary<byte, RemoteVoice> playerVoices = null;
                if (channelVoices.TryGetValue(playerId, out playerVoices))
                {
                    channelVoices.Remove(playerId);
                    foreach (var v in playerVoices)
                    {
                        v.Value.removeAndDispose();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        internal string channelStr(int channelId)
        {
            var str = this.transport.ChannelIdStr(channelId);
            if (str != null)
            {
                return channelId + "(" + str + ")";
            }
            else
            {
                return channelId.ToString();
            }
        }
        internal string playerStr(int playerId)
        {
            var str = this.transport.PlayerIdStr(playerId);
            if (str != null)
            {
                return playerId + "(" + str + ")";
            }
            else
            {
                return playerId.ToString();
            }
        }
        //public string ToStringFull()
        //{
        //    return string.Format("Photon.Voice.Client, local: {0}, remote: {1}",  localVoices.Count, remoteVoices.Count);
        //}
        #endregion
        public void Dispose()
        {
            foreach (var v in this.localVoices)
            {
                v.Value.Dispose();
            }
            foreach (var channelVoices in this.remoteVoices)
            {
                foreach (var playerVoices in channelVoices.Value)
                {
                    foreach (var voice in playerVoices.Value)
                    {
                        voice.Value.Dispose();
                    }
                }
            }
        }
    }
}
