using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(InAudio))]
public class InAudioDrawer : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Email");
        EditorGUILayout.SelectableLabel("inaudio@outlook.com");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Documentation"))
        {
            Application.OpenURL("http://innersystems.net/wiki");
        }
        if (GUILayout.Button("Forum"))
        {
            Application.OpenURL("http://forum.unity3d.com/threads/232490-InAudio-Advanced-Audio-for-Unity");
        }
        if (GUILayout.Button("Website"))
        {
            Application.OpenURL("http://innersystems.net/");
        }
    }
}
