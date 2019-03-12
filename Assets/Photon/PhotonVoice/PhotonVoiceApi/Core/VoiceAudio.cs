using System.Collections;
using System;
using System.Collections.Generic;
namespace Photon.Voice
{
    /// <summary>Audio Source interface.</summary>
    public interface IAudioDesc : IDisposable
    {
        /// <summary>Sampling rate of the audio signal (in Hz).</summary>
        int SamplingRate { get; }
        /// <summary>Number of channels in the audio signal.</summary>
        int Channels { get; }
        /// <summary>If not null, audio object is in invalid state.</summary>
        string Error { get; }
    }
    // Trivial implementation. Used to build erroneous source.
    public class AudioDesc : IAudioDesc
    {
        public AudioDesc(int samplingRate, int channels, string error)
        {
            SamplingRate = samplingRate;
            Channels = channels;
            Error = error;
        }
        public int SamplingRate { get; private set; }
        public int Channels { get; private set; }
        public string Error { get; private set; }
        public void Dispose() { }
    }
    /// <summary>Audio Reader interface.</summary>
    /// Opposed to an IAudioPusher (which will push its audio data whenever it is ready), 
    /// an IAudioReader will deliver audio data when it is "pulled" (it's Read function is called).
	public interface IAudioReader<T> : IDataReader<T>, IAudioDesc
    {
    }
    /// <summary>Audio Pusher interface.</summary>
    /// Opposed to an IAudioReader (which will deliver audio data when it is "pulled"),
    /// an IAudioPusher will push its audio data whenever it is ready,
	public interface IAudioPusher<T> : IAudioDesc
	{
        /// <summary>Set the callback function used for pushing data.</summary>
        /// <param name="callback">Callback function to use.</param>
        /// <param name="localVoice">Outgoing audio stream, for context.</param>
        void SetCallback(Action<T[]> callback, ObjectFactory<T[], int> bufferFactory);
	}
    /// <summary>Interface for an outgoing audio stream.</summary>
    /// A LocalVoice always brings a LevelMeter and a VoiceDetector, which you can access using this interface.
    public interface ILocalVoiceAudio
    {
        /// <summary>The VoiceDetector in use.</summary>
        /// Use it to enable or disable voice detector and set its parameters.
        AudioUtil.IVoiceDetector VoiceDetector { get; }
        /// <summary>The LevelMeter utility in use.</summary>
        AudioUtil.ILevelMeter LevelMeter { get; }
        /// <summary>If true, voice detector calibration is in progress.</summary>
        bool VoiceDetectorCalibrating { get; }
        /// <summary>
        /// Trigger voice detector calibration process.
        /// </summary>
        /// While calibrating, keep silence. Voice detector sets threshold based on measured backgroud noise level.
        /// <param name="durationMs">Duration of calibration (in milliseconds).</param>
        void VoiceDetectorCalibrate(int durationMs);
    }
    /// <summary>Outgoing audio stream.</summary>
    abstract public class LocalVoiceAudio<T> : LocalVoiceFramed<T>, ILocalVoiceAudio
    {
        /// <summary>Create a new LocalVoiceAudio<T> instance.</summary>
        /// <param name="voiceClient">The VoiceClient to use for this outgoing stream.</param>
        /// <param name="voiceId">Numeric ID for this voice.</param>
        /// <param name="encoder">Encoder to use for this voice.</param>
        /// <param name="channelId">Voice transport channel ID to use for this voice.</param>
        /// <returns>The new LocalVoiceAudio<T> instance.</returns>
        public static LocalVoiceAudio<T> Create(VoiceClient voiceClient, byte voiceId, IEncoder encoder, VoiceInfo voiceInfo, int channelId)
        {
            if (typeof(T) == typeof(float))
            {
                if (encoder == null || encoder is IEncoderDataFlow<float>)
                {
                    return new LocalVoiceAudioFloat(voiceClient, encoder as IEncoderDataFlow<float>, voiceId, voiceInfo, channelId) as LocalVoiceAudio<T>;
                }
                else
                    throw new Exception("[PV] CreateLocalVoice: encoder for LocalVoiceAudio<float> is not IEncoderDataFlow<float>: " + encoder.GetType());
            }
            else if (typeof(T) == typeof(short))
            {
                if (encoder == null || encoder is IEncoderDataFlow<short>)
                    return new LocalVoiceAudioShort(voiceClient, encoder as IEncoderDataFlow<short>, voiceId, voiceInfo, channelId) as LocalVoiceAudio<T>;
                else
                    throw new Exception("[PV] CreateLocalVoice: encoder for LocalVoiceAudio<short> is not IEncoderDataFlow<short>: " + encoder.GetType());
            }
            else
            {
                throw new UnsupportedSampleTypeException(typeof(T));
            }
        }
        override public IEncoder CreateDefaultEncoder(VoiceInfo info)
        {
            switch (info.Codec)
            {
                case Codec.AudioOpus:
                    return OpusCodec.EncoderFactory.Create<T>(info, Logger);
                default:
                    throw new UnsupportedCodecException("LocalVoiceAudio.CreateDefaultEncoder<" + (new T[1])[0].GetType() + ">", info.Codec, Logger);
            }
        }
    	public virtual AudioUtil.IVoiceDetector VoiceDetector { get { return voiceDetector; } }
        protected AudioUtil.VoiceDetector<T> voiceDetector;
        protected AudioUtil.VoiceDetectorCalibration<T> voiceDetectorCalibration;
        public virtual AudioUtil.ILevelMeter LevelMeter { get { return levelMeter; } }
        protected AudioUtil.LevelMeter<T> levelMeter;
        /// <summary>Trigger voice detector calibration process.</summary>
        /// While calibrating, keep silence. Voice detector sets threshold basing on measured backgroud noise level.
        /// <param name="durationMs">Duration of calibration in milliseconds.</param>
        public void VoiceDetectorCalibrate(int durationMs)
        {
            voiceDetectorCalibration.Calibrate(durationMs);
        }
        /// <summary>True if the VoiceDetector is currently calibrating.</summary>
        public bool VoiceDetectorCalibrating { get { return voiceDetectorCalibration.IsCalibrating; } }
        protected int channels;
        protected int sourceSamplingRateHz;
        protected bool resampleSource;
        internal LocalVoiceAudio(VoiceClient voiceClient, IEncoderDataFlow<T> encoder, byte id, VoiceInfo voiceInfo, int channelId)
            : base(voiceClient, encoder, id, voiceInfo, channelId,
                  voiceInfo.SamplingRate != 0 ? voiceInfo.FrameSize * voiceInfo.SourceSamplingRate / voiceInfo.SamplingRate : voiceInfo.FrameSize
                  )
        {            
            this.channels = voiceInfo.Channels;
            this.sourceSamplingRateHz = voiceInfo.SourceSamplingRate;
            if (this.sourceSamplingRateHz != voiceInfo.SamplingRate)
            {
                this.resampleSource = true;
                this.voiceClient.transport.LogWarning("[PV] Local voice #" + this.id + " audio source frequency " + this.sourceSamplingRateHz + " and encoder sampling rate " + voiceInfo.SamplingRate + " do not match. Resampling will occur before encoding.");
            }
        }
        protected void initBuiltinProcessors()
        {
            if (this.resampleSource)
            {
                AddPostProcessor(new AudioUtil.Resampler<T>(this.info.FrameSize, channels));
            }
            this.voiceDetectorCalibration = new AudioUtil.VoiceDetectorCalibration<T>(voiceDetector, levelMeter, this.info.SamplingRate, (int)this.channels);
            AddPostProcessor(levelMeter, voiceDetectorCalibration, voiceDetector); // level meter and calibration should be processed even if no signal detected
        }
    }
    /// <summary>Dummy LocalVoiceAudio</summary>
    /// For testing, this LocalVoiceAudio implementation features a <see cref="AudioUtil.VoiceDetectorDummy"></see> and a <see cref="AudioUtil.LevelMeterDummy"></see>
    public class LocalVoiceAudioDummy : LocalVoice, ILocalVoiceAudio
    {
        private AudioUtil.VoiceDetectorDummy voiceDetector;
        private AudioUtil.LevelMeterDummy levelMeter;
        public AudioUtil.IVoiceDetector VoiceDetector { get { return voiceDetector; } }
        public AudioUtil.ILevelMeter LevelMeter { get { return levelMeter; } }
        public bool VoiceDetectorCalibrating { get { return false; } }
        public void VoiceDetectorCalibrate(int durationMs) { }
        public LocalVoiceAudioDummy()
        {
            voiceDetector = new AudioUtil.VoiceDetectorDummy();
            levelMeter = new AudioUtil.LevelMeterDummy();
        }
        /// <summary>A Dummy LocalVoiceAudio instance.</summary>
        public static LocalVoiceAudioDummy Dummy = new LocalVoiceAudioDummy();
    }
    /// <summary>Specialization of <see cref="LocalVoiceAudio"></see> for float audio</summary>
    public class LocalVoiceAudioFloat : LocalVoiceAudio<float>
    {
        internal LocalVoiceAudioFloat(VoiceClient voiceClient, IEncoderDataFlow<float> encoder, byte id, VoiceInfo voiceInfo, int channelId)
            : base(voiceClient, encoder, id, voiceInfo, channelId)
        {
            // these 2 processors go after resampler
            this.levelMeter = new AudioUtil.LevelMeterFloat(this.info.SamplingRate, this.info.Channels);
            this.voiceDetector = new AudioUtil.VoiceDetectorFloat(this.info.SamplingRate, this.info.Channels);
            initBuiltinProcessors();
        }
    }
    /// <summary>Specialization of <see cref="LocalVoiceAudio"></see> for short audio</summary>
    public class LocalVoiceAudioShort : LocalVoiceAudio<short>
    {
        internal LocalVoiceAudioShort(VoiceClient voiceClient, IEncoderDataFlow<short> encoder, byte id, VoiceInfo voiceInfo, int channelId)
            : base(voiceClient, encoder, id, voiceInfo, channelId)
        {
            // these 2 processors go after resampler
            this.levelMeter = new AudioUtil.LevelMeterShort(this.info.SamplingRate, this.info.Channels); //1/2 sec
            this.voiceDetector = new AudioUtil.VoiceDetectorShort(this.info.SamplingRate, this.info.Channels);
            initBuiltinProcessors();
        }
    }
}
