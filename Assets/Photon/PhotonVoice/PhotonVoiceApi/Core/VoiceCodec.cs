// -----------------------------------------------------------------------
// <copyright file="VoiceCodec.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
//#define PHOTON_VOICE_VIDEO_ENABLE
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Photon.Voice
{   
    /// <summary>Generic media encoder interface.</summary>
    public interface IEncoder : IDisposable
    {
        /// <summary>If not null, the object is in invalid state.</summary>
        string Error { get; }
    }
    /// <summary>Interface for a generic media encoder data flow.</summary>
    public interface IEncoderDataFlow<T> : IEncoder
    {
    }
    /// <summary>Interface for an encoder data flow that synchronously encodes data.</summary>
    public interface IEncoderDataFlowDirect<T> : IEncoderDataFlow<T>
    {
        /// <summary>Encode the given uncompressed media data.</summary>
        /// <param name="buf">Array containing raw (uncompressed) data (e.g. audio samples).</param>
        /// <returns>Encoded (compressed) data.</returns>
        ArraySegment<byte> EncodeAndGetOutput(T[] buf);
    }
    // Returns compressed image instantly
    public interface IEncoderNativeImageDirect : IEncoder
    {
        IEnumerable<ArraySegment<byte>> EncodeAndGetOutput(IntPtr[] buf, int width, int height, int[] stride, ImageFormat imageFormat, Rotation rotation, Flip flip);
    }
    /// <summary>Interface for an encoder data flow that returns compressed data independently (produces output on its own) or asynchronously (usually from a queue).</summary>
    public interface IEncoderQueued : IEncoder
    {
        /// <summary>Get an Enumerable of buffers containing encoded (compressed) data.</summary>
        /// <returns>Encoded (compressed) data.</returns>
        IEnumerable<ArraySegment<byte>> GetOutput();
    }
    /// <summary>Generic media decoder interface.</summary>
    public interface IDecoder : IDisposable
    {
        /// <summary>Open (initialize) the decoder.</summary>
        /// <param name="info">Properties of the data stream to decode.</param>
        void Open(VoiceInfo info);
        /// <summary>If not null, the object is in invalid state.</summary>
        string Error { get; }
    }
    /// <summary>Interface for a media decoder that synchronously decodes data.</summary>
    public interface IDecoderDirect : IDecoder
    {
        /// <summary>Decode the given raw data buffer.</summary>
        /// <param name="buf">Buffer of encoded (compressed) data.</param>
        /// <returns>Buffer of decoded (uncompressed) data.</returns>
        byte[] DecodeToByte(byte[] buf);
        /// <summary>Decode the given raw data buffer to floating point audio.</summary>
        /// Only sensible for audio data.
        /// <param name="buf">Buffer of encoded (compressed) data.</param>
        /// <returns>Buffer of decoded (uncompressed) data.</returns>
        float[] DecodeToFloat(byte[] buf);
        /// <summary>Decode the given raw data buffer to 'short' (16-bit) audio.</summary>
        /// Only sensible for audio data.
        /// <param name="buf">Buffer of encoded (compressed) data.</param>
        /// <returns>Buffer of decoded (uncompressed) data.</returns>
        short[] DecodeToShort(byte[] buf);
    }
    /// <summary>Interface for a media decoder that feeds its data output into a separate method or callback asynchronously, or does not produce output at all.</summary>
    public interface IDecoderQueued : IDecoder
    {
        /// <summary>Decode the given raw data buffer.</summary>
        /// <remarks>This function will be called also for every missing frame, with buf = null.</remarks>
        /// <param name="buf">Buffer of encoded (compressed) data.</param>
        void Decode(byte[] buf);
    }
    public delegate void OnImageOutputNative(IntPtr buf, int width, int height, int stride);
    public interface IDecoderQueuedOutputImageNative : IDecoderQueued
    {
        ImageFormat OutputImageFormat { get; set; }
        Flip OutputImageFlip { get; set; }
        // if provided, decoder writes output to it 
        Func<int, int, IntPtr> OutputImageBufferGetter { get; set; }
        OnImageOutputNative OnOutputImage { get; set; }
    }
    /// <summary>Exception thrown if an unsupported audio sample type is encountered.</summary>
    /// <remarks>
    /// PhotonVoice generally supports 32-bit floating point ("float") or 16-bit signed integer ("short") audio,
    /// but it usually won't be converted automatically due to the high CPU overhead (and potential loss of precision) involved.
    /// </remarks>
    class UnsupportedSampleTypeException : Exception
    {
        /// <summary>Create a new UnsupportedSampleTypeException.</summary>
        /// <param name="t">The sample type actually encountered.</param>
        public UnsupportedSampleTypeException(Type t) : base("[PV] unsupported sample type: " + t) { }
    }
    /// <summary>Exception thrown if an unsupported codec is encountered.</summary>
    /// <remarks>PhotonVoice currently only supports one Codec, <see cref="Codec.AudioOpus"></see>.
    class UnsupportedCodecException : Exception
    {
        /// <summary>Create a new UnsupportedCodecException.</summary>
        /// <param name="info">The info prepending standard message.</param>
        /// <param name="codec">The codec actually encountered.</param>
        /// <param name="logger">Loogger.</param>
        public UnsupportedCodecException(string info, Codec codec, ILogger logger) : base("[PV] " + info + ": unsupported codec: " + codec) { }
    }
    /// <summary>Enum for Media Codecs supported by PhotonVoice.</summary>
    /// <remarks>Transmitted in <see cref="VoiceInfo"></see>. Do not change the values of this Enum!</remarks>
    public enum Codec
    {
        /// <summary>OPUS audio</summary>
        AudioOpus = 11
#if PHOTON_VOICE_VIDEO_ENABLE
        , VideoVP8 = 21
#endif
    }
    public enum ImageFormat
    {
        I420, // native vpx (no format conversion before encodong)                        
        YV12, // native vpx (no format conversion before encodong)
        Android420,
        RGBA,
        ABGR,
        BGRA,
        ARGB,
    }
    public enum Rotation
    {
        Rotate0 = 0,      // No rotation.
        Rotate90 = 90,    // Rotate 90 degrees clockwise.
        Rotate180 = 180,  // Rotate 180 degrees.
        Rotate270 = 270,  // Rotate 270 degrees clockwise.
    }
    public enum Flip
    {
        None,
        Vertical,
        Horizontal
    }
    public class ImageBufferInfo
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int[] Stride { get; private set; }
        public ImageFormat Format { get; private set; }
        public Rotation Rotation { get; set; }
        public Flip Flip { get; set; }
        public ImageBufferInfo(int width, int height, int[] stride, ImageFormat format)
        {
            Width = width;
            Height = height;
            Stride = stride;
            Format = format;
        }
    }
    public class ImageBufferNative
    {
        public ImageBufferNative(ImageBufferInfo info)
        {
            Info = info;
        }
        public ImageBufferInfo Info { get; protected set; }
        public IntPtr[] Planes { get; protected set; }
        // Release resources for dispose or reuse.
        public virtual void Release() { }
        public virtual void Dispose() { }
    }
    // Allocates native buffers for planes
    // Supports releasing to image pool with allocation reuse
    public class ImageBufferNativeAlloc : ImageBufferNative, IDisposable
    {
        ImageBufferNativePool<ImageBufferNativeAlloc> pool;
        public ImageBufferNativeAlloc(ImageBufferNativePool<ImageBufferNativeAlloc> pool, ImageBufferInfo info) : base(info)
        {
            this.pool = pool;
            Planes = new IntPtr[info.Stride.Length];
            for (int i = 0; i < info.Stride.Length; i++)
            {
                Planes[i] = System.Runtime.InteropServices.Marshal.AllocHGlobal(info.Stride[i] * info.Height);
            }
        }
        public override void Release()
        {
            if (pool != null)
            {
                pool.Release(this);
            }
        }
        public override void Dispose()
        {
            for (int i = 0; i < Info.Stride.Length; i++)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(Planes[i]);
            }
        }
    }
    // Acquires byte[] plane via GHandle. Optimized for single plane images.
    // Supports releasing to image pool after freeing GHandle (object itself reused only)
    public class ImageBufferNativeGCHandleSinglePlane : ImageBufferNative, IDisposable
    {
        ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool;
        GCHandle planeHandle;
        public ImageBufferNativeGCHandleSinglePlane(ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool, ImageBufferInfo info) : base(info)
        {
            if (info.Stride.Length != 1)
            {
                throw new Exception("ImageBufferNativeGCHandleSinglePlane wrong plane count " + info.Stride.Length);
            }
            this.pool = pool;
            Planes = new IntPtr[1];
        }
        public void PinPlane(byte[] plane)
        {
            planeHandle = GCHandle.Alloc(plane, GCHandleType.Pinned);
            Planes[0] = planeHandle.AddrOfPinnedObject();
        }
        public override void Release()
        {
            planeHandle.Free();
            if (pool != null)
            {
                pool.Release(this);
            }
        }
        public override void Dispose()
        {
        }
    }
    internal static class VoiceCodec
    {
        internal static IDecoder CreateDefaultDecoder(int channelId, int playerId, byte voiceId, VoiceInfo info, ILogger logger)
        {
            switch (info.Codec)
            {
                case Codec.AudioOpus:
                    return new OpusCodec.Decoder(logger);
#if PHOTON_VOICE_VIDEO_ENABLE
                case Codec.VideoVP8:
                    return new VPxCodec.Decoder();
#endif
                default:
                    return null;
            }
        }
    }
}
