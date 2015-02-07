using UnityEditor;

namespace InAudioSystem.InAudioEditor
{ 
public static class MultiDataDrawer  {
    public static void Draw(InAudioNode node)
    {

        UndoHelper.GUIUndo(node, "Name Change", () =>
            EditorGUILayout.TextField("Name", node.Name),
            s => node.Name = s);
        NodeTypeDataDrawer.Draw(node); 
    }
}
}