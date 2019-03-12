using System.Collections;
using POpusCodec.Enums;
using POpusCodec;
using System;
using System.Collections.Generic;
namespace Photon.Voice
{
    public class OpusCodec
    {
        public enum FrameDuration
        {
            Frame2dot5ms = 2500,
            Frame5ms = 5000,
            Frame10ms = 10000,
            Frame20ms = 20000,
            Frame40ms = 40000,
            Frame60ms = 60000
        }
        public static class EncoderFactory
        {
            public static IEncoder Create<T>(VoiceInfo i, ILogger logger)
            {
                var x = new T[1];
                if (x[0].GetType() == typeof(float))
                    return new EncoderFloat(i, logger);
                else if (x[0].GetType() == typeof(short))
                    return new EncoderShort(i, logger);
                else
                    throw new UnsupportedCodecException("EncoderFactory.Create<" + x[0].GetType() + ">", i.Codec, logger);
            }
        }
        abstract public class Encoder<T> : IEncoderDataFlowDirect<T>
        {        
            protected OpusEncoder encoder;
            protected bool disposed;
            protected Encoder(VoiceInfo i, ILogger logger)
            {
                try
                {
                    encoder = new OpusEncoder((SamplingRate)i.SamplingRate, (Channels)i.Channels, i.Bitrate, OpusApplicationType.Voip, (Delay)(i.FrameDurationUs * 2 / 1000));
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                    if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                    {
                        Error = "Exception in OpusCodec.Encoder constructor";
                    }
                    logger.LogError("[PV] OpusCodec.Encoder: " + Error);
                }
            }
            public string Error { get; private set; }
            public void Dispose()
            {
                lock (this)
                {
                    if (encoder != null)
                    {
                        encoder.Dispose();
                    }
                    disposed = true;
                }
            }
            public abstract ArraySegment<byte> EncodeAndGetOutput(T[] buf);
        }
        public class EncoderFloat : Encoder<float>
        {
            private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });
            internal EncoderFloat(VoiceInfo i, ILogger logger) : base(i, logger) { }
            public override ArraySegment<byte> EncodeAndGetOutput(float[] buf)
            {
                lock (this)
                {
                    if (disposed || Error != null) return EmptyBuffer;
                    else return encoder.Encode(buf);
                }
            }
        }
        public class EncoderShort : Encoder<short>
        {
            private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });
            internal EncoderShort(VoiceInfo i, ILogger logger) : base(i, logger) { }
            public override ArraySegment<byte> EncodeAndGetOutput(short[] buf)
            {
                lock (this)
                {
                    if (disposed || Error != null) return EmptyBuffer;
                    else return encoder.Encode(buf);
                }
            }
        }
        public class Decoder : IDecoderDirect
        {
            OpusDecoder decoder;
            ILogger logger;
            public Decoder(ILogger logger)
            {
                this.logger = logger;
            }
            public void Open(VoiceInfo i)
            {
                try
                {
                    decoder = new OpusDecoder((SamplingRate)i.SamplingRate, (Channels)i.Channels);
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                    if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                    {
                        Error = "Exception in OpusCodec.Decoder constructor";
                    }
                    logger.LogError("[PV] OpusCodec.Decoder: " + Error);
                }
            }
            public string Error { get; private set; }
            public byte[] DecodeToByte(byte[] buf)
            {
                throw new NotImplementedException();
            }
            private static readonly float[] EmptyBufferFloat = new float[0];
            private static readonly short[] EmptyBufferShort = new short[0];
            public float[] DecodeToFloat(byte[] buf)
            {
                return Error == null ? decoder.DecodePacketFloat(buf) : EmptyBufferFloat;
            }
            public short[] DecodeToShort(byte[] buf)
            {
                return Error == null ? decoder.DecodePacketShort(buf) : EmptyBufferShort;
            }
            public void Dispose()
            {
                if (decoder != null)
                {
                    decoder.Dispose();
                }
            }
        }
        public class Util
        {
            internal static int bestEncoderSampleRate(int f)
            {
                int diff = int.MaxValue;
                int res = (int)SamplingRate.Sampling48000;
                foreach (var x in Enum.GetValues(typeof(SamplingRate)))
                {
                    var d = Math.Abs((int)x - f);
                    if (d < diff)
                    {
                        diff = d;
                        res = (int)x;
                    }
                }
                return res;
            }
        }
    }
}
