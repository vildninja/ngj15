using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
public static class TrackDataDrawer
{
    private static int selectedArea;
    private static Vector2 ScrollArea;
    //private static InAudioNode activeNode;

    public static void Draw(InAudioNode node)
    {
        EditorGUILayout.BeginVertical();
        var trackData = (node.NodeData as InTrackData);
        NodeTypeDataDrawer.DrawName(node);

        UndoHelper.GUIUndo(trackData, "Track length", ref trackData.TrackLength, () => EditorGUILayout.FloatField("Track length", trackData.TrackLength));
        UndoHelper.GUIUndo(trackData, "Zoom", ref trackData.Zoom, () => EditorGUILayout.FloatField("Zoom", trackData.Zoom));
        
        selectedArea = GUILayout.SelectionGrid(selectedArea, new []{"Track", "Normal"}, 2);

        if (selectedArea == 1)
        {
            NodeTypeDataDrawer.Draw(node);
        }
        else
        {
            EditorGUILayout.BeginVertical();
            ScrollArea = EditorGUILayout.BeginScrollView(ScrollArea, false, true);
            
            foreach (var layer in trackData.Layers)
            {
                DrawItem(node, layer);
            }  
            
            if (GUILayout.Button("Add Layer", GUILayout.Width(150)))
            {
                UndoHelper.RecordObjectFull(trackData, "Add layer");
                trackData.Layers.Add(new InLayerData());
            }

            if (GUILayout.Button("Delete Last Layer", GUILayout.Width(150)))
            {   
                UndoHelper.RecordObjectFull(trackData, "Remove layer");
                {
                    if (trackData.Layers.Count > 0)
                    {
                        trackData.Layers.RemoveAt(trackData.Layers.Count - 1);
                    }
                }
                
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    private static Vector2 area;

    private static float DrawItem(InAudioNode node, InLayerData item)
    {
        Rect dragArea =EditorGUILayout.BeginVertical();
        //var trackData = node.NodeData as InTrackData;

        
        EditorGUILayout.BeginHorizontal();

        
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        GUILayout.Label("Left");
        item.ScrollPos.y = EditorGUILayout.BeginScrollView(item.ScrollPos, false, false, GUILayout.Height(150)/*, GUILayout.Width(EditorWindow.GetWindow<InAudioWindow>().position.width - 365)*/).y;
        GUILayout.Label("Inside left");
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Right");
        item.ScrollPos.x = EditorGUILayout.BeginScrollView(item.ScrollPos, true, false, GUILayout.Height(150)/*, GUILayout.Width(EditorWindow.GetWindow<InAudioWindow>().position.width - 365)*/).x;
        GUILayout.Label("");
        Rect start = GUILayoutUtility.GetLastRect();
        //start.y += 20;
        GUI.Label(start, "Hello"); 

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        DropAreaGUI<InAudioNode>(dragArea, otherNode => !TreeWalker.IsParentOf(node, otherNode) || node == otherNode);
        
        return 0;
    }

    private static void AfterDrag(InAudioNode inAudioNode)
    { 
        Debug.Log("Ok");
    }

    private static bool Predicate(InAudioNode inAudioNode)
    {
        
      /*  if (!TreeWalker.IsParentOf(inAudioNode, activeNode))
        {
            return true;    
        }
        return false;
    */
        return false;
    }

    private static void DropAreaGUI<T>(Rect drop_area, Func<T, bool> predicate) where T : Object, InITreeNode<T>
    {
        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                
                if (!drop_area.Contains(evt.mousePosition) || predicate(DragAndDrop.objectReferences[0] as T))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.None;
                    return;
                }            

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Debug.Log("Ook");
                    
                }
                Event.current.Use();
                break;
        }
    }
}

}