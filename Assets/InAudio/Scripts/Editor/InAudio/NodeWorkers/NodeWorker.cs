using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem
{

    public static class NodeWorker
    {
        public static bool IsChildOf<T>(T node, T potentialChild) where T : Object, InITreeNode<T>
        {
            if (node == potentialChild)
                return true;

            for (int i = 0; i < node.GetChildren.Count; ++i)
            {
                bool isChild = IsChildOf(node.GetChildren[i], potentialChild);
                if (isChild)
                    return true;
            }
            return false;
        }

        public static bool IsParentOf<T>(T baseNode, T potentialParent) where T : Object, InITreeNode<T>
        {
            if (baseNode == potentialParent)
                return true;

            if (baseNode.GetParent != null)
                return IsParentOf(baseNode.GetParent, potentialParent);

            return false;
        }

        public static void RemoveFromParent<T>(T node) where T : Object, InITreeNode<T>
        {
            node.GetParent.GetChildren.Remove(node);
        }

        public static void RemoveNullChildren<T>(T node) where T : Object, InITreeNode<T>
        {
            var children = node.GetChildren;
            for (int i = 0; i < children.Count; ++i)
            {
                if (children[i] == null)
                {
                    children.RemoveAt(i);
                }
            }
        }

        //public static void AssignParent<T>(T node, T newParent) where T : Object, ITreeNode<T>
        //{
        //    if (node != null && newParent != null)
        //    {
        //        newParent.GetChildren.Add(node);
        //        node.GetParent = newParent;
        //    }
        //}

        public static void ReasignNodeParent<T>(T current, T newParent) where T : Object, InITreeNode<T>
        {
            RemoveFromParent(current);
            current.AssignParent(newParent);
        }

        public static void ReasignNodeParent(InAudioNode current, InAudioNode newParent)
        {
            if (current.Parent.Type == AudioNodeType.Random)
            {
                //Remove self from parent random

                int currentIndexInParent = current.Parent.Children.IndexOf(current);
                (current.Parent.NodeData as RandomData).weights.RemoveAt(currentIndexInParent);
            }
            RemoveFromParent(current);
            current.AssignParent(newParent);
            if (newParent.Type == AudioNodeType.Random)
            {
                (newParent.NodeData as RandomData).weights.Add(50);
            }

        }

        public static void MoveNodeOneUp<T>(T node) where T : Object, InITreeNode<T>
        {

            if (node is InAudioNode)//As parent could be a random one, it needs to record this first
            {
                UndoHelper.RecordObject(new Object[] { node.GetParent, (node.GetParent as InAudioNode).NodeData },
                    "Undo Reorder Of " + node.GetName);
            }
            else
            {
                UndoHelper.RecordObject(new Object[] { node.GetParent }, "Undo Reorder Of " + node.GetName);
            }

            var children = node.GetParent.GetChildren;
            var index = children.IndexOf(node);
            if (index != 0 && children.Count > 0)
            {
                //TODO Remove hack
                if (node.GetType() == typeof(InAudioNode))
                {
                    var audioNode = node as InAudioNode;
                    if (audioNode.Parent.Type == AudioNodeType.Random)
                    {
                        (audioNode.Parent.NodeData as RandomData).weights.SwapAtIndexes(index, index - 1);
                    }
                }
                children.SwapAtIndexes(index, index - 1);
            }
        }


        public static void MoveNodeOneDown<T>(T node) where T : Object, InITreeNode<T>
        {

            if (node is InAudioNode)
            {
                UndoHelper.RecordObject(new Object[] { node.GetParent, (node.GetParent as InAudioNode).NodeData },
                    "Undo Reorder Of " + node.GetName);
            }
            else
            {
                UndoHelper.RecordObject(new Object[] { node.GetParent }, "Undo Reorder Of " + node.GetName);
            }


            var children = node.GetParent.GetChildren;
            var index = children.IndexOf(node);
            if (children.Count > 0 && index != children.Count - 1)
            {
                //TODO Remove hack
                if (node.GetType() == typeof(InAudioNode))
                {
                    var audioNode = node as InAudioNode;
                    if (audioNode.Parent.Type == AudioNodeType.Random)
                    {
                        (audioNode.Parent.NodeData as RandomData).weights.SwapAtIndexes(index, index + 1);
                    }
                }
                children.SwapAtIndexes(index, index + 1);
            }
        }

        public static void AssignToNodes(InAudioNode node, Action<InAudioNode> assignFunc)
        {
            assignFunc(node);
            for (int i = 0; i < node.Children.Count; ++i)
            {
                AssignToNodes(node.Children[i], assignFunc);
            }
        }

        public static T DuplicateHierarchy<T>(T toCopy, Action<T, T> elementAction = null) where T : Component, InITreeNode<T>
        {
            return CopyHierarchy(toCopy, toCopy.GetParent, elementAction);
        }

        private static T CopyHierarchy<T>(T toCopy, T parent, Action<T, T> elementAction) where T : Component, InITreeNode<T>
        {
            T newNode = CopyComponent(toCopy);
            newNode.AssignParent(parent);
            newNode.ID = GUIDCreator.Create();

            if (elementAction != null)
                elementAction(toCopy, newNode);

            int childrenCount = newNode.GetChildren.Count;
            for (int i = 0; i < childrenCount; i++)
            {
                CopyHierarchy(newNode.GetChildren[i], newNode, elementAction);
            }
            newNode.GetChildren.RemoveRange(0, childrenCount);
            return newNode;
        }

        public static T CopyComponent<T>(T toCopy) where T : Component
        {
            T newComp = toCopy.gameObject.AddComponentUndo(toCopy.GetType()) as T;
            EditorUtility.CopySerialized(toCopy, newComp);

            return newComp;
        }

        /*public static GameObject CreateFolder<T>(string name, string path, T root)
        {
            SystemFolderHelper.CreateIfMissing(FolderSettings.AudioDataCreateFolder);
            AssetDatabase.CopyAsset(FolderSettings.EmptyTemplatePath,
                FolderSettings.AudioDataPath + "AudioData " + GUIDCreator.Create() + ".prefab");
        }*/
    }
}