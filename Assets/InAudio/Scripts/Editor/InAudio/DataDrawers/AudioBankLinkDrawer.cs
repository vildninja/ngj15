using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
public static class AudioBankLinkDrawer
{
    public static void Draw(InAudioBankLink bankLink)
    { 
        EditorGUILayout.BeginVertical();

        UndoHelper.GUIUndo(bankLink, "Name Change", ref bankLink.Name, () => 
            EditorGUILayout.TextField("Name", bankLink.Name));

        if (bankLink.Type == AudioBankTypes.Link)
        {
            EditorGUIHelper.DrawID(bankLink.GUID);

            bool autoLoad = EditorGUILayout.Toggle("Auto load", bankLink.AutoLoad);
            if (autoLoad != bankLink.AutoLoad) //Value has changed
            {
                UndoHelper.RecordObjectFull(bankLink, "Bank Auto Load");
                bankLink.AutoLoad = autoLoad;
            }

            Rect lastArea = GUILayoutUtility.GetLastRect();
            lastArea.y += 28;
            lastArea.width = 200;
            if(GUI.Button(lastArea, "Find Folders using this bank"))
            {
                EditorWindow.GetWindow<InAudioWindow>().Find(audioNode => audioNode.GetBank() != bankLink);
            }
             
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (Application.isPlaying)
            {
                EditorGUILayout.Toggle("Is Loaded", bankLink.IsLoaded);
            }
        }

        EditorGUILayout.EndVertical();
        //UndoCheck.Instance.CheckDirty(node);
      
    }
}
}