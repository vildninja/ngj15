using System;
using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem
{
public static class AudioNodeWorker  {
    public static InAudioNode CreateNode(GameObject go, InAudioNode parent, int guid, AudioNodeType type)
    {
        var node = go.AddComponentUndo<InAudioNode>();

        node.GUID = guid;
        node.Type = type;
        node.Name = parent.Name + " Child";
        node.Bus = parent.Bus;

        node.AssignParent(parent);

        return node;
    }

    public static InAudioNode CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<InAudioNode>();
        AddDataClass(node);
        node.GUID = guid;
        node.Type = AudioNodeType.Root;
        node.FoldedOut = true;
        node.Name = "Root";
        return node;
    }

    public static InAudioNode CreateTree(GameObject go, int numberOfChildren, InAudioBus bus)
    {
        var Tree = CreateRoot(go, GUIDCreator.Create());
        Tree.Bus = bus;
        for (int i = 0; i < numberOfChildren; ++i)
        {
            var newNode = CreateNode(go, Tree, GUIDCreator.Create(), AudioNodeType.Folder);
            AddDataClass(newNode);
        }
        return Tree;
    }

    public static InAudioNode CreateNode(GameObject go, InAudioNode parent, AudioNodeType type)
    {
        var newNode = CreateNode(go, parent, GUIDCreator.Create(), type);
        AddDataClass(newNode);
        return newNode;
    }

    public static InAudioNodeBaseData AddDataClass(InAudioNode node)
    {
        switch (node.Type)
        {
            case AudioNodeType.Root:
                node.NodeData = node.gameObject.AddComponentUndo<InFolderData>();
                break;
            case AudioNodeType.Audio:
                node.NodeData = node.gameObject.AddComponentUndo<InAudioData>();
                break;
            case AudioNodeType.Random:
                node.NodeData = node.gameObject.AddComponentUndo<RandomData>();
                for (int i = 0; i < node.Children.Count; ++i)
                    (node.NodeData as RandomData).weights.Add(50);
                break;
            case AudioNodeType.Sequence:
                node.NodeData = node.gameObject.AddComponentUndo<InSequenceData>();
                break;
            case AudioNodeType.Multi:
                node.NodeData = node.gameObject.AddComponentUndo<MultiData>();
                break;
            case AudioNodeType.Track:
                node.NodeData = node.gameObject.AddComponentUndo<InTrackData>();
                break;
            case AudioNodeType.Folder:
                var folderData = node.gameObject.AddComponentUndo<InFolderData>();
                //folderData.BankLink = node.GetBank();
                node.NodeData = folderData;
                break;
        }
        return node.NodeData;
    }

    public static void AddNewParent(InAudioNode node, AudioNodeType parentType)
    {
        UndoHelper.RecordObject(new Object[] { node, node.Parent, node.GetBankDirect() }, "Undo Add New Parent for " + node.Name);
        var newParent = CreateNode(node.gameObject, node.Parent, parentType);
        var oldParent = node.Parent;
        newParent.Bus = node.Bus;
        newParent.FoldedOut = true;
        if (node.Type == AudioNodeType.Folder)
        {
            InFolderData data = (InFolderData)newParent.NodeData;
            data.BankLink = oldParent.GetBank();
        }
        int index = oldParent.Children.FindIndex(node);
        NodeWorker.RemoveFromParent(node);
        node.AssignParent(newParent);

        OnRandomNode(newParent);

        NodeWorker.RemoveFromParent(newParent);
        oldParent.Children.Insert(index, newParent);
    }

    private static void OnRandomNode(InAudioNode parent)
    {
        if (parent.Type == AudioNodeType.Random)
            (parent.NodeData as RandomData).weights.Add(50);
    }
     
    public static InAudioNode CreateChild(InAudioNode parent, AudioNodeType newNodeType)
    {
        var bank = parent.GetBank();
        UndoHelper.RecordObject(UndoHelper.Array(parent, parent.NodeData, bank != null ? bank.LazyBankFetch : null), "Undo Node Creation");
        OnRandomNode(parent);

        var child = CreateNode(parent.gameObject, parent, GUIDCreator.Create(), newNodeType);
        parent.FoldedOut = true;
        child.Name = parent.Name + " Child";
        var data = AddDataClass(child);
        if (newNodeType == AudioNodeType.Folder)
        {
            (data as InFolderData).BankLink = parent.GetBank();
        }
        return child;
    }

    public static void ConvertNodeType(InAudioNode node, AudioNodeType newType)
    {
        if (newType == node.Type)
            return;
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RecordObjectFull(new Object[] {node, node.NodeData}, "Change Node Type");
            
            AudioBankWorker.RemoveNodeFromBank(node);
            
            node.Type = newType;
            UndoHelper.Destroy(node.NodeData);
            AddDataClass(node);
            
        });
        
    }

    public static void Duplicate(InAudioNode audioNode)
    {
        UndoHelper.DoInGroup(() => 
        {

            List<Object> toUndo = TreeWalker.FindAll(audioNode, node => node.GetBank().LazyBankFetch).ConvertList<InAudioBank, Object>();

            toUndo.Add(audioNode.Parent);
            toUndo.Add(audioNode.GetBank());

            UndoHelper.RecordObjectFull(toUndo.ToArray(), "Undo Duplication Of " + audioNode.Name);

            if (audioNode.Parent.Type == AudioNodeType.Random)
            {
                (audioNode.Parent.NodeData as RandomData).weights.Add(50);   
            }
            NodeWorker.DuplicateHierarchy(audioNode, (@oldNode, newNode) =>
            { 
                var gameObject = audioNode.gameObject;
                if(oldNode.NodeData != null)
                { 
                    Type type = oldNode.NodeData.GetType();
                    newNode.NodeData = gameObject.AddComponentUndo(type) as InAudioNodeBaseData;
                    EditorUtility.CopySerialized(oldNode.NodeData, newNode.NodeData);
                    if (newNode.Type == AudioNodeType.Audio)
                    {
                        AudioBankWorker.AddNodeToBank(newNode, (oldNode.NodeData as InAudioData).EditorClip);
                    }
                }
            });
        });
    }

    public static void DeleteNode(InAudioNode node)
    {
        UndoHelper.DoInGroup(() =>
        {           
            //UndoHelper.RecordObjectFull(UndoHelper.Array(node.Parent, node.Parent.AudioData), "Undo Deletion of " + node.Name);

            if (node.Parent.Type == AudioNodeType.Random) //We also need to remove the child from the weight list
            {
                var data = node.Parent.NodeData as RandomData;
                if (data != null)
                    data.weights.RemoveAt(node.Parent.Children.FindIndex(node)); //Find in parent, and then remove the weight in the random node
                node.Parent.Children.Remove(node);
            }
            
            DeleteNodeRec(node);
        
        });
    }

    private static void DeleteNodeRec(InAudioNode node)
    {
        AudioBankWorker.RemoveNodeFromBank(node);

        /*TreeWalker.ForEach(InAudioInstanceFinder.DataManager.EventTree, @event =>
        {
            for (int i = 0; i < @event.ActionList.Count; i++)
            {
                var action = @event.ActionList[i];
                if (action.Target == node)
                {
                    UndoHelper.RegisterFullObjectHierarchyUndo(action);
                }
            }
        });*/

        for (int i = 0; i < node.Children.Count; i++)
        {
            DeleteNodeRec(node.Children[i]);
        }


        UndoHelper.Destroy(node.NodeData);
        UndoHelper.Destroy(node);
    }
}
}
