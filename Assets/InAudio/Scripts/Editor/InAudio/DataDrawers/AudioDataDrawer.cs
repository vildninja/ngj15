using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
public static class AudioDataDrawer
{
    
    public static void Draw(InAudioNode node)
    {
        UndoHelper.GUIUndo(node, "Name Change", ref node.Name, () => 
            EditorGUILayout.TextField("Name", node.Name));

        Rect area = GUILayoutUtility.GetLastRect();
        
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        InAudioData audioData = node.NodeData as InAudioData;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("");
        area = GUILayoutUtility.GetLastRect();
        Rect objectArea = area;
        if(Application.isPlaying)
            objectArea.width -= 100;
        var clip = (AudioClip)EditorGUI.ObjectField(objectArea, audioData.EditorClip, typeof(AudioClip), false);
        Rect buttonArea = area;
        if (Application.isPlaying)
        {
            buttonArea.x += buttonArea.width - 100;
            buttonArea.width = 70;
            GUI.enabled = false;
            EditorGUI.LabelField(buttonArea, "Is Loaded");
            buttonArea.x += 70;
            buttonArea.width = 10;
            EditorGUI.Toggle(buttonArea, audioData.RuntimeClip != null);
            GUI.enabled = true;
        }



        if(GUILayout.Button("Preview", GUILayout.Width(60)))
        {
            AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
            var root = TreeWalker.FindParentBeforeFolder(node);
            source.volume = RuntimeHelper.ApplyVolume(root, node);
            source.pitch = RuntimeHelper.ApplyPitch(root, node);
            source.clip = audioData.EditorClip;
            source.Play();
        }

        if (GUILayout.Button("Raw", GUILayout.Width(45)))
        {
            AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
            source.clip = audioData.EditorClip;
            source.volume = 1.0f;
            source.pitch = 1.0f;
            source.Play();
        }

        if (GUILayout.Button("Stop", GUILayout.Width(45)))
        {
            AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
            source.Stop();
            source.clip = null;
        }

        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        if (audioData.RuntimeClip != null && Application.isPlaying)
        {       
            audioData.RuntimeClip = clip;
        }
        
        if (clip != audioData.EditorClip) //Assign new clip
        {
            UndoHelper.RecordObjectFull(new Object[] { audioData, node.GetBank().LazyBankFetch }, "Changed " + node.Name + " Clip");
            audioData.EditorClip = clip;
            audioData.RuntimeClip = clip;
            AudioBankWorker.SwapClipInBank(node, clip);
            
            EditorUtility.SetDirty(node.GetBank().LazyBankFetch.gameObject);
            EditorUtility.SetDirty(node.NodeData.gameObject);
        }

        EditorGUILayout.EndHorizontal();

        NodeTypeDataDrawer.Draw(node);
    }
}
}