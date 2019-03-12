namespace Photon.Voice.Unity.Editor
{
    using System;
    using System.Collections.Generic;
    using Unity;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Recorder))]
    public class RecorderEditor : Editor
    {
        private Recorder recorder;

        private int unityMicrophoneDeviceIndex;

        private string[] photonDeviceNames;

        private int calibrationTime = 200;

        private void OnEnable()
        {
            recorder = target as Recorder;
            unityMicrophoneDeviceIndex = Math.Max(0,
            ArrayUtility.IndexOf(Microphone.devices, recorder.UnityMicrophoneDevice));
            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (Recorder.PhotonMicrophoneEnumerator.IsSupported)
            {
                photonDeviceNames = new string[Recorder.PhotonMicrophoneEnumerator.Count];
                List<string> tempNames = new List<string>(photonDeviceNames.Length);
                for (int i = 0; i < Recorder.PhotonMicrophoneEnumerator.Count; i++)
                {
                    string micName = Recorder.PhotonMicrophoneEnumerator.NameAtIndex(i);
                    int count = 0;
                    for (int j = 0; j < tempNames.Count; j++)
                    {
                        if (tempNames[j].StartsWith(micName))
                        {
                            count++;
                        }
                    }
                    tempNames.Add(string.Format("{0}{1}", micName, count == 0 ? string.Empty : string.Format(" {0}", count)));
                }
                photonDeviceNames = tempNames.ToArray();
                if (recorder.PhotonMicrophoneDeviceId < 0 ||
                    recorder.PhotonMicrophoneDeviceId >= Recorder.PhotonMicrophoneEnumerator.Count)
                {
                    recorder.PhotonMicrophoneDeviceId = 0;
                }
            }
            #endif
        }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            //            serializedObject.UpdateIfRequiredOrScript();
            if (Application.isPlaying && recorder.RequiresInit)
            {
                if (recorder.IsInitialized)
                {
                    EditorGUILayout.HelpBox("Recorder requires re-initialization. Call Recorder.ReInit().", MessageType.Warning);
                    if (GUILayout.Button("ReInit"))
                    {
                        recorder.ReInit();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Recorder requires initialization. Call Recorder.Init(VoiceClient, Object).", MessageType.Warning);
                }
            }
            VoiceLogger.ExposeLogLevel(serializedObject, recorder);
            EditorGUI.BeginChangeCheck();
            recorder.ReactOnSystemChanges = EditorGUILayout.Toggle(new GUIContent("React On System Changes", "If true, ReInit when Unity detects Audio Config. changes."), recorder.ReactOnSystemChanges);
            recorder.TransmitEnabled = EditorGUILayout.Toggle(new GUIContent("Transmit Enabled", "If true, audio transmission is enabled."), recorder.TransmitEnabled);
            if (recorder.IsInitialized || !recorder.RequiresInit)
            {
                recorder.IsRecording = EditorGUILayout.Toggle(new GUIContent("IsRecording", "If true, audio recording is on."), recorder.IsRecording);
            }
            if (recorder.IsRecording)
            {
                float amplitude = 0f;
                if (recorder.IsCurrentlyTransmitting)
                {
                    amplitude = recorder.LevelMeter.CurrentPeakAmp;
                    if (amplitude > 1f)
                    {
                        amplitude /= 32768;
                    }
                }
                EditorGUILayout.Slider("Level", amplitude, 0, 1);
            }
            recorder.Encrypt = EditorGUILayout.Toggle(new GUIContent("Encrypt", "If true, voice stream is sent encrypted."), recorder.Encrypt);
            recorder.InterestGroup = (byte)EditorGUILayout.IntField(new GUIContent("Interest Group", "Target interest group that will receive transmitted audio."), recorder.InterestGroup);
            if (recorder.InterestGroup == 0)
            {
                recorder.DebugEchoMode = EditorGUILayout.Toggle(new GUIContent("Debug Echo", "If true, outgoing stream routed back to client via server same way as for remote client's streams."), recorder.DebugEchoMode);
            }

            recorder.ReliableMode = EditorGUILayout.Toggle(new GUIContent("Reliable Mode", "If true, stream data sent in reliable mode."), recorder.ReliableMode);

            EditorGUILayout.LabelField("Codec Parameters", EditorStyles.boldLabel);
            recorder.FrameDuration = (OpusCodec.FrameDuration)EditorGUILayout.EnumPopup(new GUIContent("Frame Duration", "Outgoing audio stream encoder delay."), recorder.FrameDuration);
            recorder.SamplingRate = (POpusCodec.Enums.SamplingRate)EditorGUILayout.EnumPopup(
                new GUIContent("Sampling Rate", "Outgoing audio stream sampling rate."), recorder.SamplingRate);
            recorder.Bitrate = EditorGUILayout.IntField(new GUIContent("Bitrate", "Outgoing audio stream bitrate."),
                recorder.Bitrate);

            EditorGUILayout.LabelField("Audio Source Settings", EditorStyles.boldLabel);
            recorder.SourceType = (Recorder.InputSourceType) EditorGUILayout.EnumPopup(new GUIContent("Input Source Type", "Input audio data source type"), recorder.SourceType);
            switch (recorder.SourceType)
            {
                case Recorder.InputSourceType.Microphone:
                    recorder.MicrophoneType = (Recorder.MicType) EditorGUILayout.EnumPopup(
                        new GUIContent("Microphone Type",
                            "Which microphone API to use when the Source is set to Microphone."),
                        recorder.MicrophoneType);
                    switch (recorder.MicrophoneType)
                    {
                        case Recorder.MicType.Unity:
                            unityMicrophoneDeviceIndex = EditorGUILayout.Popup("Microphone Device", unityMicrophoneDeviceIndex, Microphone.devices);
                            recorder.UnityMicrophoneDevice = Microphone.devices[unityMicrophoneDeviceIndex];
                            int minFreq, maxFreq;
                            Microphone.GetDeviceCaps(Microphone.devices[unityMicrophoneDeviceIndex], out minFreq, out maxFreq);
                            EditorGUILayout.LabelField("Microphone Device Caps", string.Format("{0}..{1} Hz", minFreq, maxFreq));
                            break;
                        case Recorder.MicType.Photon:
                            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                            if (Recorder.PhotonMicrophoneEnumerator.IsSupported)
                            {
                                recorder.PhotonMicrophoneDeviceId = EditorGUILayout.Popup("Microphone Device",
                                    recorder.PhotonMicrophoneDeviceId, photonDeviceNames);
                            }
                            else
                            {
                                recorder.PhotonMicrophoneDeviceId = -1;
                                EditorGUILayout.HelpBox("PhotonMicrophoneEnumerator Not Supported", MessageType.Error);
                            }
                            #endif
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case Recorder.InputSourceType.AudioClip:
                    recorder.AudioClip = EditorGUILayout.ObjectField(new GUIContent("Audio Clip", "Source audio clip."), recorder.AudioClip, typeof(AudioClip), false) as AudioClip;
                    recorder.LoopAudioClip =
                        EditorGUILayout.Toggle(new GUIContent("Loop", "Loop playback for audio clip sources."),
                            recorder.LoopAudioClip);
                    break;
                case Recorder.InputSourceType.Factory:
                    EditorGUILayout.HelpBox("Add a custom InputFactory method in code.", MessageType.Info);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            recorder.TypeConvert = (Recorder.SampleTypeConv) EditorGUILayout.EnumPopup(
                new GUIContent("Type Convert",
                    "Force creation of 'short' pipeline and convert audio data to short for 'float' audio sources."),
                recorder.TypeConvert);
            EditorGUILayout.LabelField("Voice Activity Detection (VAD)", EditorStyles.boldLabel);
            recorder.VoiceDetection = EditorGUILayout.Toggle(new GUIContent("Detect", "If true, voice detection enabled."), recorder.VoiceDetection);
            if (recorder.VoiceDetection)
            {
                recorder.VoiceDetectionThreshold =
                    EditorGUILayout.FloatField(new GUIContent("Threshold", "Voice detection threshold (0..1, where 1 is full amplitude)."), recorder.VoiceDetectionThreshold);
                recorder.VoiceDetectionDelayMs =
                    EditorGUILayout.IntField(new GUIContent("Delay (ms)", "Keep detected state during this time after signal level dropped below threshold. Default is 500ms"), recorder.VoiceDetectionDelayMs);
                if (recorder.VoiceDetectorCalibrating)
                {
                    EditorGUILayout.LabelField(string.Format("Calibrating {0} ms", calibrationTime));
                }
                else
                {
                    calibrationTime = EditorGUILayout.IntField("Calibration Time (ms)", calibrationTime);
                    if (recorder.IsInitialized)
                    {
                        if (GUILayout.Button("Calibrate"))
                        {
                            recorder.VoiceDetectorCalibrate(calibrationTime);
                        }
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}