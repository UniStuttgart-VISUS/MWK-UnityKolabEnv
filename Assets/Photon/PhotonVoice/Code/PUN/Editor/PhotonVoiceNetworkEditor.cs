using UnityEngine;

namespace Photon.Voice.PUN.Editor
{
    using Unity.Editor;
    using UnityEditor;

    [CustomEditor(typeof(PhotonVoiceNetwork))]
    public class PhotonVoiceNetworkEditor : VoiceConnectionEditor
    {
        private SerializedProperty autoConnectAndJoinSp;
        private SerializedProperty autoLeaveAndDisconnectSp;
        private SerializedProperty autoCreateSpeakerIfNotFoundSp;

        protected override void OnEnable()
        {
            base.OnEnable();
            autoConnectAndJoinSp = serializedObject.FindProperty("AutoConnectAndJoin");
            autoLeaveAndDisconnectSp = serializedObject.FindProperty("AutoLeaveAndDisconnect");
            autoCreateSpeakerIfNotFoundSp = serializedObject.FindProperty("AutoCreateSpeakerIfNotFound");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(autoConnectAndJoinSp, new GUIContent("Auto Connect And Join", "Auto connect voice client and join a voice room when PUN client is joined to a PUN room"));
            EditorGUILayout.PropertyField(autoLeaveAndDisconnectSp, new GUIContent("Auto Leave And Disconnect", "Auto disconnect voice client when PUN client is not joined to a PUN room"));
            EditorGUILayout.PropertyField(autoCreateSpeakerIfNotFoundSp, new GUIContent("Create Speaker If Not Found", "Auto instantiate a GameObject and attach a Speaker component to link to a remote audio stream if no candidate could be foun"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
            base.OnInspectorGUI();
        }
    }
}
