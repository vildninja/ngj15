using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class FolderDrawer
{
    public static void Draw(InAudioNode node)
    {
        EditorGUILayout.BeginVertical();

        #region Bank 

        UndoHelper.GUIUndo(node, "Name Change", ref node.Name, () =>
            EditorGUILayout.TextField("Name", node.Name));
        var data = node.NodeData as InFolderData;
        if (node.Type == AudioNodeType.Folder)
        {
            bool overrideparent = EditorGUILayout.Toggle("Override Parent Bank", data.OverrideParentBank);
            if (overrideparent != data.OverrideParentBank)
            {
                AudioBankWorker.ChangeBankOverride(node, InAudioInstanceFinder.DataManager.BankLinkTree, InAudioInstanceFinder.DataManager.AudioTree);
            }
        }
        else
            EditorGUILayout.LabelField(""); //To fill out the area from the toggle

        
        if (data.OverrideParentBank == false && node.Type != AudioNodeType.Root)
        {
            GUI.enabled = false;
        }

        EditorGUILayout.BeginHorizontal();

        var parentLink = FindParentBank(node);
        if (data.OverrideParentBank)
        {
            if (data.BankLink != null)
            {
                EditorGUILayout.LabelField("Bank", data.BankLink.GetName);
            }
            else
            {
                if (parentLink != null)
                    EditorGUILayout.LabelField("Bank", "Missing Bank, using parent bank" + parentLink.GetName);
                else
                {
                    EditorGUILayout.LabelField("Bank", "Missing Banks, no bank found");
                }
            }
        }
        else
        {
            if (parentLink != null)
                EditorGUILayout.LabelField("Using Bank", parentLink.GetName);
            else
            {
                EditorGUILayout.LabelField("Using Bank", "Missing");
            }
        }

        bool wasEnabled = GUI.enabled;
        GUI.enabled = true;
        if(GUILayout.Button("Find", GUILayout.Width(50)))
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(parentLink);
        }
        
        Rect findArea = GUILayoutUtility.GetLastRect();
        findArea.y += 20;
        if (GUI.Button(findArea, "Find"))
        {
            EditorWindow.GetWindow<AuxWindow>().FindBank(data.BankLink);
        }

        GUI.enabled = wasEnabled;

        GUILayout.Button("Drag new bank here", GUILayout.Width(140));

        var newBank = HandleDragging(GUILayoutUtility.GetLastRect());
        if (newBank != null)
        {
            AudioBankWorker.ChangeBank(node, newBank, InAudioInstanceFinder.DataManager.BankLinkTree, InAudioInstanceFinder.DataManager.AudioTree);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUI.enabled = false;
        if (data.BankLink != null)
            EditorGUILayout.LabelField("Node Bank", data.BankLink.GetName);
        else
            EditorGUILayout.LabelField("Node Bank", "Missing Bank");
        GUI.enabled = true;
#endregion

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        #region Bus
        DataDrawerHelper.DrawBus(node);
        #endregion

        EditorGUILayout.EndVertical();
    } 

    private static InAudioBankLink HandleDragging(Rect area)
    {
        if (area.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated ||
            Event.current.type == EventType.DragPerform)
        {
            if (DragAndDrop.objectReferences.Length != 0)
            {
                var bankLink = DragAndDrop.objectReferences[0] as InAudioBankLink;
                if (bankLink != null && bankLink.Type == AudioBankTypes.Link)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        return DragAndDrop.objectReferences[0] as InAudioBankLink;
                    }
                }
            }
            else if(Event.current.type == EventType.Repaint)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
        return null;
    }

    private static InAudioBankLink FindParentBank(InAudioNode node)
    {
        if (node == null)
            return null;

        var data = node.NodeData as InFolderData;
        if (data.OverrideParentBank)
        {
            return data.BankLink;
        }
        else if (node.IsRoot)
        {
            return data.BankLink;
        }
        else
            return FindParentBank(node.Parent);
    }
}


}