// -----------------------------------------------------------------------
// <copyright file="LoadBalancingTransport.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2015 Exit Games GmbH
// </copyright>
// <summary>
//   Extends Photon Realtime API with audio streaming functionality.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;
using LoadBalancing = Photon.Realtime;

namespace Photon.Voice
{
    /// <summary>
    /// PhotonVoice communication uses a single type of event, but differentiates transmission Channels 
    /// by encoding a channelId into VoiceEventCode.
    /// </summary>
    /// Transmission Channels are not for selective forwarding: use InterestGroups for that.
    /// Instead, they are to differentiate opus audio from (future) other codecs or media.
    /// 
    /// For this purpose, a range of event codes of length <see cref="LoadBalancingPeer.ChannelCount"></see>, starting from Code0, is used.
    class VoiceEventCode
    {
        /// <summary>
        /// Start of voice event codes range.
        /// </summary>
        /// Change if it conflicts with other event codes used in the same Photon room.
        public const byte Code0 = 201;

        /// <summary>Get the event code for the given channel ID.</summary>
        /// <param name="channelID">Channel ID to get event code for.</param>
        /// <returns>The corresponding event code.</returns>
        static public byte GetCode(int channelID)
        {
            return (byte)(Code0 + channelID);
        }

        /// <summary>Try to get the channel ID for the given event code.</summary>
        /// <param name="evCode">Event code to find Channel ID from.</param>
        /// <param name="maxChannels">Maximum Channel ID in use.</param>
        /// <param name="channelID">(output) Channel ID found.</param>
        /// <returns>True if a valid channel ID could be recovered from evCode, false otherwise.</returns>
        static public bool TryGetChannelID(byte evCode, int maxChannels, out byte channelID)
        {
            if (evCode >= Code0 && evCode < Code0 + maxChannels)
            {
                channelID = (byte)(evCode - Code0);
                return true;
            }
            else
            {
                channelID = 0;
                return false;
            }
        }
    }

    /// <summary>
    /// Extends LoadBalancingClient with audio streaming functionality.
    /// </summary>
    /// <remarks>
    /// Use your normal LoadBalancing workflow to join a Voice room. 
    /// All standard LoadBalancing features are available.
    ///
    /// To work with audio:
    /// - Create outgoing audio streams with Client.CreateLocalVoice.
    /// - Handle new incoming audio streams info with <see cref="OnRemoteVoiceInfoAction"/> .
    /// - Handle incoming audio streams data with <see cref="OnAudioFrameAction"/> .
    /// - Handle closing of incoming audio streams with <see cref="OnRemoteVoiceRemoved">.
    /// </remarks>
    public class LoadBalancingTransport : LoadBalancingClient, IVoiceTransport, IDisposable
    {
        /// <summary>The <see cref="VoiceClient"></see> implementation associated with this LoadBalancingTransport.</summary>
        public VoiceClient VoiceClient { get { return this.voiceClient; } }

        protected VoiceClient voiceClient;

        public void LogError(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ERROR, string.Format(fmt, args)); }
        public void LogWarning(string fmt, params object[] args) { this.DebugReturn(DebugLevel.WARNING, string.Format(fmt, args)); }
        public void LogInfo(string fmt, params object[] args) { this.DebugReturn(DebugLevel.INFO, string.Format(fmt, args)); }
        public void LogDebug(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ALL, string.Format(fmt, args)); }

        public int AssignChannel(VoiceInfo v)
        {
            // 0 is for user events
            return 1 + Array.IndexOf(Enum.GetValues(typeof(Codec)), v.Codec);            
        }

        public bool IsChannelJoined(int channelId) { return this.State == LoadBalancing.ClientState.Joined; }

        public void SetDebugEchoMode(LocalVoice v)
        {
            if (this.State == LoadBalancing.ClientState.Joined)
            {
                if (v.DebugEchoMode)
                {
                    SendVoicesInfo(new List<LocalVoice>() { v }, v.channelId, this.LocalPlayer.ActorNumber);
                }
                else
                {
                    SendVoiceRemove(v, v.channelId, this.LocalPlayer.ActorNumber);
                }
            }
        }

        /// <summary>
        /// Initializes a new <see cref="LoadBalancingTransport"/>.
        /// </summary>
        /// <param name="connectionProtocol">Connection protocol (UDP or TCP). <see cref="ConnectionProtocol"></see></param>
        public LoadBalancingTransport(ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp) : base(connectionProtocol)
        {
            base.EventReceived += onEventActionVoiceClient;
            base.StateChanged += onStateChangeVoiceClient;
            this.voiceClient = new VoiceClient(this);
            var voiceChannelsCount = Enum.GetValues(typeof(Codec)).Length + 1; // channel per stream type, channel 0 is for user events
            if (LoadBalancingPeer.ChannelCount < voiceChannelsCount)
            {
                this.LoadBalancingPeer.ChannelCount = (byte)voiceChannelsCount;
            }
        }

        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// Call this method regularly (2 to 20 times a second).
        /// </summary>
        new public void Service()
        {
            base.Service();
            this.voiceClient.Service();
        }

        [Obsolete("Use LoadBalancingPeer::OpChangeGroups().")]
        public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            return this.LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
        }

        [Obsolete("Use GlobalInterestGroup.")]
        public byte GlobalAudioGroup
        {
            get { return GlobalInterestGroup; }
            set { GlobalInterestGroup = value; }
        }
        /// <summary>
        /// Set global audio group for this client. This call sets InterestGroup for existing local voices and for created later to given value.
        /// Client set as listening to this group only until LoadBalancingPeer.OpChangeGroups() called. This method can be called any time.
        /// </summary>
        /// <see cref="LocalVoice.InterestGroup"/>
        /// <see cref="LoadBalancingPeer.OpChangeGroups(byte[], byte[])"/>
        public byte GlobalInterestGroup
        {
            get { return this.voiceClient.GlobalInterestGroup; }
            set
            {
                this.voiceClient.GlobalInterestGroup = value;
                if (this.State == LoadBalancing.ClientState.Joined)
                {
                    if (this.voiceClient.GlobalInterestGroup != 0)
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalInterestGroup });
                    }
                    else
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], null);
                    }
                }                
            }
        }


        #region nonpublic

        object sendLock = new object();

        //
        public void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId)
        {
            object content = voiceClient.buildVoicesInfo(voices, true);

            var sendOpt = new SendOptions()
            {
                Reliability = true,
                Channel = (byte)channelId
            };

            var opt = new LoadBalancing.RaiseEventOptions();
            if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            lock (sendLock)
            {
                this.OpRaiseEvent(VoiceEventCode.GetCode(channelId), content, opt, sendOpt);
            }

            if (targetPlayerId == 0) // send debug echo infos to myself if broadcast requested
            {
                SendDebugEchoVoicesInfo(channelId);
            }
        }

        /// <summary>Send VoicesInfo events to the local player for all voices that have DebugEcho enabled.</summary>
        /// This function will call <see cref="SendVoicesInfo"></see> for all local voices of our <see cref="VoiceClient"></see>
        /// that have DebugEchoMode set to true, with the given channel ID, and the local Player's ActorNumber as target.
        /// <param name="channelId">Transport Channel ID</param>
        public void SendDebugEchoVoicesInfo(int channelId)
        {
            var voices = voiceClient.LocalVoices.Where(x => x.DebugEchoMode);
            if (voices.Count() > 0)
            { 
                SendVoicesInfo(voices, channelId, this.LocalPlayer.ActorNumber);
            }
        }

        public void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId)
        {
            object content = voiceClient.buildVoiceRemoveMessage(voice);
            var sendOpt = new SendOptions()
            {
                Reliability = true,
                Channel = (byte)channelId
            };

            var opt = new LoadBalancing.RaiseEventOptions();
            if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            if (voice.DebugEchoMode)
            {
                opt.Receivers = ReceiverGroup.All;
            }
            lock (sendLock)
            {

                this.OpRaiseEvent(VoiceEventCode.GetCode(channelId), content, opt, sendOpt);
            }
        }

        public void SendFrame(ArraySegment<byte> data, byte evNumber, byte voiceId, int channelId, LocalVoice localVoice)
        {
            object[] content = new object[] { voiceId, evNumber, data };

            var sendOpt = new SendOptions()
            {
                Reliability = localVoice.Reliable,
                Channel = (byte)channelId,
                Encrypt = localVoice.Encrypt
            };

            var opt = new LoadBalancing.RaiseEventOptions();
            if (localVoice.DebugEchoMode)
            {
                opt.Receivers = LoadBalancing.ReceiverGroup.All;
            }
            opt.InterestGroup = localVoice.InterestGroup;
            lock (sendLock)
            {
                this.OpRaiseEvent((byte)VoiceEventCode.GetCode(channelId), content, opt, sendOpt);
            }
            this.LoadBalancingPeer.SendOutgoingCommands();
        }

        public string ChannelIdStr(int channelId) { return null; }
        public string PlayerIdStr(int playerId) { return null; }

        private void onEventActionVoiceClient(EventData ev)
        {
            byte channel;
            // check for voice event first
            if (VoiceEventCode.TryGetChannelID(ev.Code, this.LoadBalancingPeer.ChannelCount, out channel))
            {
                // Payloads are arrays. If first array element is 0 than next is event subcode. Otherwise, the event is data frame with voiceId in 1st element.                    
                this.voiceClient.onVoiceEvent(ev[(byte)LoadBalancing.ParameterCode.CustomEventContent], channel, (int)ev[LoadBalancing.ParameterCode.ActorNr], this.LocalPlayer.ActorNumber);
            }
            else
            {
                int playerId;
                switch (ev.Code)
                {
                    case (byte)LoadBalancing.EventCode.Join:
                        playerId = (int)ev[LoadBalancing.ParameterCode.ActorNr];
                        if (playerId == this.LocalPlayer.ActorNumber)
                        {
                        }
                        else
                        {
                            this.voiceClient.sendVoicesInfo(playerId);// send to new joined only
                        }
                        break;
                    case (byte)LoadBalancing.EventCode.Leave:
                        {
                            playerId = (int)ev[LoadBalancing.ParameterCode.ActorNr];
                            if (playerId == this.LocalPlayer.ActorNumber)
                            {
                                this.voiceClient.clearRemoteVoices();
                            }
                            else
                            {
                                onPlayerLeave(playerId);
                            }
                        }
                        break;
                }
            }
        }

        void onStateChangeVoiceClient(LoadBalancing.ClientState fromState, LoadBalancing.ClientState state)
        {
            switch (state)
            {
                case LoadBalancing.ClientState.Joined:
                    this.voiceClient.clearRemoteVoices();
                    this.voiceClient.sendVoicesInfo(0);// my join, broadcast
                    if (this.voiceClient.GlobalInterestGroup != 0)
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalInterestGroup });
                    }
                    break;
                case LoadBalancing.ClientState.Disconnected:
                    this.voiceClient.clearRemoteVoices();
                    break;
            }
        }
        private void onPlayerLeave(int playerId)
        {
            if (this.voiceClient.removePlayerVoices(playerId))
            {
                this.DebugReturn(DebugLevel.INFO, "[PV] Player " + playerId + " voices removed on leave");
            }            
            else
            {
                this.DebugReturn(DebugLevel.WARNING, "[PV] Voices of player " + playerId + " not found when trying to remove on player leave");
            }
        }

        #endregion

        /// <summary>
        /// Releases all resources used by the <see cref="LoadBalancingTransport"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.voiceClient.Dispose();
        }
    }
}
