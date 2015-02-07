using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class RandomDataDrawer
    {
        public static void Draw(InAudioNode node)
        {
            //UndoHandler.CheckUndo(new UnityEngine.Object[] { node, node.AudioData }, "Random Data Node Change");
            UndoHelper.GUIUndo(node, "Name Change", ref node.Name, () =>
                EditorGUILayout.TextField("Name", node.Name));
            NodeTypeDataDrawer.Draw(node);
            EditorGUILayout.Separator();

            InAudioNodeData baseData = (InAudioNodeData)node.NodeData;
            if (baseData.SelectedArea == 0)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField("Weights");

                var weights = (node.NodeData as RandomData).weights;
                if (node.Children.Count == weights.Count)
                {

                    for (int i = 0; i < node.Children.Count; ++i)
                    {
                        var child = node.Children[i];


                        int index = i;
                        UndoHelper.GUIUndo(node.NodeData, "Weights",
                            () => EditorGUILayout.IntSlider(child.Name, weights[index], 0, 100), i1 =>
                                weights[index] = i1);

                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("The number of weights does not match the children count", MessageType.Error);
                    if (GUILayout.Button("Fix weights"))
                    {
                        weights.Clear();
                        for (int i = 0; i < node.Children.Count; i++)
                        {
                            weights.Add(50);
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }
            //UndoHandler.CheckGUIChange();

        }
    }
}