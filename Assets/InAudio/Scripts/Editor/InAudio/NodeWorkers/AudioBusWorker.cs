using System.Collections.Generic;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEngine;

namespace InAudioSystem
{

public static class AudioBusWorker
{
    private static InAudioBus CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<InAudioBus>();
        node.GUID = guid;
        node.FoldedOut = true;
        node.Name = "Master Bus";
        return node;
    }

    private static InAudioBus CreateBus(GameObject go, InAudioBus parent, int guid)
    {
        var node = go.AddComponentUndo<InAudioBus>();
        node.GUID = guid;
        node.name = parent.Name + " Child";
        node.AssignParent(parent);
        return node;
    }

    public static InAudioBus CreateTree(GameObject go)
    {
        var tree = CreateRoot(go, GUIDCreator.Create());
        return tree;
    }

    public static void DeleteBus(InAudioBus bus, InAudioNode root)
    {
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RecordObjectFull(bus.Parent, "Bus deletion");
            bus.Parent.Children.Remove(bus);
            HashSet<InAudioBus> toDelete = new HashSet<InAudioBus>();
            GetBusesToDelete(toDelete, bus);

            var runtimePlayers = bus.RuntimePlayers;
            if (runtimePlayers != null)
            {
                for (int i = 0; i < runtimePlayers.Count; ++i)
                {
                    runtimePlayers[i].SetNewBus(bus.Parent);
                }
            }

            List<InAudioNode> affectedNodes = new List<InAudioNode>();
            //Get all affected nodes 
            TreeWalker.FindAllNodes(root, node => toDelete.Contains(node.GetBus()), affectedNodes);

            toDelete.ToArray().ForEach(UndoHelper.Destroy);
            
            for (int i = 0; i < affectedNodes.Count; ++i)
            {
                affectedNodes[i].Bus = bus.Parent;
            }           
        });
    }


    private static void GetBusesToDelete(HashSet<InAudioBus> toDelete, InAudioBus bus)
    {
        toDelete.Add(bus);
        for (int i = 0; i < bus.Children.Count; ++i)
        {
            GetBusesToDelete(toDelete, bus.Children[i]);
        }
    }

    public static InAudioBus CreateChild(InAudioBus parent)
    {
        var child = CreateBus(parent.gameObject, parent, GUIDCreator.Create());
        child.FoldedOut = true;
        child.Name = parent.Name + " Child";

        return child;
    }

    public static bool CanBeDuckedBy(InAudioBus selectedNode, InAudioBus dragging)
    {
        var draggingBus = dragging;
        if (draggingBus == null)
            return false;
        //Does it already exist in the collection?
        if (selectedNode.DuckedBy.TrueForAny(data => data.DuckedBy == dragging))
            return false;

        if (draggingBus.IsRoot)
            return false;

        if (NodeWorker.IsChildOf(selectedNode, draggingBus))
            return false;

        if (NodeWorker.IsParentOf(selectedNode.GetParent, draggingBus))
            return false;
        return true;
    }
}
}
