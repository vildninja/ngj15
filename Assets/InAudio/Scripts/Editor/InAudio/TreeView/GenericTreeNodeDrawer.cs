using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.TreeDrawer
{
    public class GenericTreeNodeDrawer
    {
        private static GUIStyle noMargain;

        public static bool Draw<T>(T node, bool isSelected) where T : Object, InITreeNode<T>
        {
            if (noMargain == null)
            {
                noMargain = new GUIStyle();
                noMargain.margin = new RectOffset(0, 0, 0, 0);
            }

            Rect area = EditorGUILayout.BeginHorizontal();
            if (isSelected)
                GUI.DrawTexture(area, EditorResources.Background);

            GUILayout.Space(EditorGUI.indentLevel*16);

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (folded || node.GetChildren.Count == 0)
                picture = EditorResources.Minus;
            else
                picture = EditorResources.Plus;
            
            GUILayout.Label(picture, noMargain, GUILayout.Height(EditorResources.Minus.height),
                GUILayout.Width(EditorResources.Minus.width));
            Rect foldRect = GUILayoutUtility.GetLastRect();
            if (Event.current.ClickedWithin(foldRect))
            {
                folded = !folded;
                Event.current.Use();
            }
            Texture icon = TreeNodeDrawerHelper.LookUpIcon(node);

         
            TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), icon, noMargain);
            EditorGUILayout.LabelField("");
            

            EditorGUILayout.EndHorizontal();
            Rect labelArea = GUILayoutUtility.GetLastRect();
            Rect buttonArea = labelArea;
            if (!node.IsRoot)
            {
                buttonArea.x = buttonArea.x + 56 + EditorGUI.indentLevel*16;
                buttonArea.width = 20;
                buttonArea.height = 14;
                GUI.Label(buttonArea, EditorResources.Up, noMargain);
                if (Event.current.ClickedWithin(buttonArea))
                {
                    NodeWorker.MoveNodeOneUp(node);
                    Event.current.Use();
                }
                buttonArea.y += 15;
                GUI.Label(buttonArea, EditorResources.Down, noMargain);
                if (Event.current.ClickedWithin(buttonArea))
                {
                    NodeWorker.MoveNodeOneDown(node);
                    Event.current.Use();
                }
                labelArea.x += 20;
            }
            labelArea.y += 6;
            labelArea.x += 65;
            EditorGUI.LabelField(labelArea, node.GetName);

            return folded;
        }
    }

    public static class TreeNodeDrawerHelper
    {
        public static void DrawIcon(Rect lastArea, Texture icon, GUIStyle style)
        {
            Rect iconRect = GUILayoutUtility.GetLastRect();
            iconRect.height = 16;
            iconRect.width = 16;
            iconRect.x += 33;
            iconRect.y += 8;
            GUI.Label(iconRect, icon, style);
        }

        public static Texture LookUpIcon<T>(T node) where T : Object, InITreeNode<T>
        {
            if (node is InAudioNode) 
            {
                InAudioNode audioNode = node as InAudioNode;
                if (audioNode.Type == AudioNodeType.Audio)
                    return EditorResources.Audio;
                if (audioNode.Type == AudioNodeType.Folder || audioNode.IsRoot)
                    return EditorResources.Folder;
                if (audioNode.Type == AudioNodeType.Random)
                    return EditorResources.Dice;
                if (audioNode.Type == AudioNodeType.Sequence)
                    return EditorResources.List;
                if (audioNode.Type == AudioNodeType.Multi)
                    return EditorResources.Tree;
            }
            else if (node is InAudioBankLink)
            {
                InAudioBankLink link = node as InAudioBankLink;
                if(link.Type == AudioBankTypes.Link)
                    return EditorResources.Bank;
                else
                {
                    return EditorResources.Folder;
                }
            }

            return null;
        }
    }
    
}
