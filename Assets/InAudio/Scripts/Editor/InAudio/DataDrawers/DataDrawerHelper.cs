using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class DataDrawerHelper
{

    public static void DrawBus(InAudioNode node)
    {

        if (!node.IsRoot)
        {
            bool overrideParent = EditorGUILayout.Toggle("Override Parent Bus", node.OverrideParentBus);
            if (overrideParent != node.OverrideParentBus)
            {
                UndoHelper.RecordObjectFull(new Object[] {node.NodeData, node}, "Override parent bus");
                node.OverrideParentBus = overrideParent;
            }
            if (!node.OverrideParentBus)
                GUI.enabled = false;
        }
        EditorGUILayout.BeginHorizontal();

        //
        
        if (node.GetBus() != null)
        {

            var usedBus = node.GetBus();
            if (usedBus != null)
                EditorGUILayout.TextField("Used Bus", usedBus.Name);
            else
            {
                EditorGUILayout.TextField("Used Bus", "Missing bus");
            }
        }
        else
        {
            GUILayout.Label("Missing node");
        }

        if (GUILayout.Button("Find"))
        {
            SearchHelper.SearchFor(node.Bus);
        }

        GUI.enabled = true;
        GUILayout.Button("Drag bus here to assign");

        var buttonArea = GUILayoutUtility.GetLastRect();
        var bus = HandleBusDrag(buttonArea);
        if (bus != null)
        {
            UndoHelper.RecordObjectFull(node, "Assign bus");
            node.Bus = bus;
            node.OverrideParentBus = true;
            Event.current.Use();
        }


        EditorGUILayout.EndHorizontal();
    }

    private static InAudioBus HandleBusDrag(Rect area)
    {
        if (area.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated ||
            Event.current.type == EventType.DragPerform)
        {
            bool canDropObject = true;
            int clipCount = DragAndDrop.objectReferences.Count(obj => obj is InAudioBus);

            if (clipCount != DragAndDrop.objectReferences.Length || clipCount == 0)
                canDropObject = false;

            if (canDropObject)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                {
                    return DragAndDrop.objectReferences[0] as InAudioBus;
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
        return null;
    }
}
}