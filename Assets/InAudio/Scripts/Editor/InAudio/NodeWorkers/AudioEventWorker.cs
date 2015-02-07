using System;
using System.Collections.Generic;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem
{
public static class AudioEventWorker  {
    private static InAudioEventNode CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponentUndo<InAudioEventNode>();
        node.Type = EventNodeType.Root;
        node.GUID = guid;
        node.FoldedOut = true;
        node.Name = "Root";
        return node;
    }

    private static InAudioEventNode CreateFolder(GameObject go, int guid, InAudioEventNode parent)
    {
        var node = go.AddComponentUndo<InAudioEventNode>();
        node.Type = EventNodeType.Folder;
        node.GUID = guid;
        node.Name = parent.Name + " Child";
        node.AssignParent(parent);
        return node;
    }

    public static void DeleteNode(InAudioEventNode node)
    {
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RegisterUndo(node.Parent, "Event Deletion");
            node.Parent.Children.Remove(node); 
            DeleteNodeRec(node);
        });
    }

    private static void DeleteNodeRec(InAudioEventNode node)
    {
        for (int i = 0; i < node.ActionList.Count; i++)
        {
            UndoHelper.Destroy(node.ActionList[i]);
        }
        for (int i = 0; i < node.Children.Count; ++i)
        {
            DeleteNodeRec(node.Children[i]);
        }

        UndoHelper.Destroy(node);
    }

   
    private static InAudioEventNode CreateEvent(GameObject go, InAudioEventNode parent, int guid, EventNodeType type)
    {
        var node = go.AddComponentUndo<InAudioEventNode>();
        node.Type = type;
        node.GUID = guid;
        node.Name = parent.Name + " Child";
        node.AssignParent(parent);
        return node;
    }

    public static InAudioEventNode CreateTree(GameObject go, int levelSize)
    {
        var tree = CreateRoot(go, GUIDCreator.Create());

        for (int i = 0; i < levelSize; ++i)
        {
            CreateFolder(go, GUIDCreator.Create(), tree);
        }

        return tree;
    }

    public static InAudioEventNode CreateNode(InAudioEventNode parent, EventNodeType type)
    {
        var child = CreateEvent(parent.gameObject, parent, GUIDCreator.Create(), type);
        child.FoldedOut = true;
        
        return child;
    }

    public static void ReplaceActionDestructiveAt(InAudioEventNode audioEvent, EventActionTypes enumType, int toRemoveAndInsertAt)
    {
        //A reel mess this function.
        //It adds a new component of the specied type, replaces the current at the toRemoveAndInsertAt index, and then deletes the old one
        float delay = audioEvent.ActionList[toRemoveAndInsertAt].Delay;
        Object target = audioEvent.ActionList[toRemoveAndInsertAt].Target;
        var newActionType = ActionEnumToType(enumType);

        UndoHelper.Destroy(audioEvent.ActionList[toRemoveAndInsertAt]);
        //UndoHelper.RecordObject(audioEvent, "Event Action Creation");

        audioEvent.ActionList.RemoveAt(toRemoveAndInsertAt);
        var added = AddEventAction(audioEvent, newActionType, enumType);

        added.Delay = delay;
        added.Target = target; //Attempt to set the new value, will only work if it is the same type

        audioEvent.ActionList.Insert(toRemoveAndInsertAt, added);
        audioEvent.ActionList.RemoveLast();
    }

    public static T AddEventAction<T>(InAudioEventNode audioevent, EventActionTypes enumType) where T : AudioEventAction
    {
        var eventAction = audioevent.gameObject.AddComponentUndo<T>();
        audioevent.ActionList.Add(eventAction);
        eventAction.EventActionType = enumType;
        return eventAction;
    }

    public static AudioEventAction AddEventAction(InAudioEventNode audioevent, Type eventActionType, EventActionTypes enumType) 
    {
        UndoHelper.RecordObject(audioevent, "Event Action Creation");
        var eventAction = audioevent.gameObject.AddComponentUndo(eventActionType) as AudioEventAction;
        audioevent.ActionList.Add(eventAction);
        eventAction.EventActionType = enumType;

        return eventAction;
    }

    public static InAudioEventNode DeleteActionAtIndex(InAudioEventNode audioevent, int index)
    {
        
        UndoHelper.RecordObject(audioevent, "Event Action Creation");
        UndoHelper.Destroy(audioevent.ActionList[index]);
            
        
        audioevent.ActionList.RemoveAt(index);

        return audioevent;
    }

    public static InAudioEventNode Duplicate(InAudioEventNode audioEvent)
    {
        return NodeWorker.DuplicateHierarchy(audioEvent, (@oldNode, newNode) =>
        {
            newNode.ActionList.Clear();
            for (int i = 0; i < oldNode.ActionList.Count; i++)
            {
                newNode.ActionList.Add(NodeWorker.CopyComponent(oldNode.ActionList[i]));
            }
        });
    }


    public static Type ActionEnumToType(EventActionTypes actionType)
    {
        switch(actionType)
        {
            case EventActionTypes.Play:
                return typeof( InEventAudioAction);
            case EventActionTypes.Stop:
                return typeof( InEventAudioAction);
            case EventActionTypes.StopAll:
                return typeof( InEventAudioAction);
            case EventActionTypes.BankLoading:
                return typeof( InEventBankLoadingAction);
            case EventActionTypes.SetBusVolume:
                return typeof( InEventBusAction);
            case EventActionTypes.Break:
                return typeof( InEventAudioAction);
            case EventActionTypes.StopAllInBus:
                return typeof( InEventBusStopAction);
            case EventActionTypes.SetBusMute:
                return typeof(InEventBusMuteAction);
        }
        return null;
    }

    public static bool CanDropObjects(InAudioEventNode audioEvent, Object[] objects)
    {
        if (objects.Length == 0 || audioEvent == null)
            return false;

        if (audioEvent.Type == EventNodeType.Event)
        {
            bool bankLinkDrop;
            bool audioBusDrop;
            var audioNodeDrop = CanDropNonEvent(objects, out bankLinkDrop, out audioBusDrop);

            return audioNodeDrop | bankLinkDrop | audioBusDrop;
        }
        else if (audioEvent.Type == EventNodeType.Folder || audioEvent.Type == EventNodeType.Root)
        {
            var draggingEvent = objects[0] as InAudioEventNode;
            if (draggingEvent != null)
            {

                if (draggingEvent.Type == EventNodeType.Event)
                    return true;
                if ((draggingEvent.Type == EventNodeType.Folder && !NodeWorker.IsChildOf(draggingEvent, audioEvent)) ||
                    draggingEvent.Type == EventNodeType.EventGroup)
                    return true;
            }
            else 
            {
                bool bankLinkDrop;
                bool audioBusDrop;
                var audioNodeDrop = CanDropNonEvent(objects, out bankLinkDrop, out audioBusDrop);

                return audioNodeDrop | bankLinkDrop | audioBusDrop;
            }
        }
        else if (audioEvent.Type == EventNodeType.EventGroup)
        {
            var draggingEvent = objects[0] as InAudioEventNode;
            if (draggingEvent == null)
                return false;
            if (draggingEvent.Type == EventNodeType.Event)
                return true;
        }
        

        return false;
    }

    private static bool CanDropNonEvent(Object[] objects, out bool bankLinkDrop, out bool audioBusDrop)
    {
        var audioNodes = GetConvertedList<InAudioNode>(objects.ToList());
        bool audioNodeDrop = audioNodes.TrueForAll(node => node != null && node.IsPlayable);

        var audioBankLinks = GetConvertedList<InAudioBankLink>(objects.ToList());
        bankLinkDrop = audioBankLinks.TrueForAll(node => node != null && node.Type == AudioBankTypes.Link);

        var busNodes = GetConvertedList<InAudioBus>(objects.ToList());
        audioBusDrop = busNodes.TrueForAll(node => node != null);
        return audioNodeDrop;
    }

    private static List<T> GetConvertedList<T>(List<Object> toConvert) where T : class
    {
        return toConvert.ConvertAll(obj => obj as T);
    }

    public static bool OnDrop(InAudioEventNode audioevent, Object[] objects)
    {
        UndoHelper.DoInGroup(() =>
        {
            //if (audioevent.Type == EventNodeType.Folder)
            //{
            //    UndoHelper.RecordObjectInOld(audioevent, "Created event");
            //    audioevent = CreateNode(audioevent, EventNodeType.Event);
            //}

            if (objects[0] as InAudioEventNode)
            {
                var movingEvent = objects[0] as InAudioEventNode;

                UndoHelper.RecordObjectFull(new Object[] { audioevent, movingEvent, movingEvent.Parent }, "Event Move");
                NodeWorker.ReasignNodeParent((InAudioEventNode)objects[0], audioevent);
                audioevent.IsFoldedOut = true;
            }

            var audioNode = objects[0] as InAudioNode;
            if (audioNode != null && audioNode.IsPlayable)
            {

                UndoHelper.RecordObjectFull(audioevent, "Adding of Audio Action");
                var action = AddEventAction<InEventAudioAction>(audioevent,
                    EventActionTypes.Play);
                action.Node = audioNode;
 
            }

            var audioBank = objects[0] as InAudioBankLink;
            if (audioBank != null)
            {
                UndoHelper.RecordObjectFull(audioevent, "Adding of Bank Load Action");
                var action = AddEventAction<InEventBankLoadingAction>(audioevent,
                    EventActionTypes.BankLoading);
                action.BankLink = audioBank;
            }

            var audioBus = objects[0] as InAudioBus;
            if (audioBus != null)
            {
                UndoHelper.RecordObjectFull(audioevent, "Adding of Bus Volume");
                var action = AddEventAction<InEventBusAction>(audioevent, EventActionTypes.SetBusVolume);
                action.Bus = audioBus;
            }
            Event.current.Use();
        });
        return true;
    }

}
}
