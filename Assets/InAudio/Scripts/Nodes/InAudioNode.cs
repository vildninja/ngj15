using System;
using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

public class InAudioNode : MonoBehaviour, InITreeNode<InAudioNode>
{
    public int GUID;

    public AudioNodeType Type;

    public InAudioNodeBaseData NodeData;
  
    //If we loose the connection, we can rebuild 
    public int ParentGUID;

    public string Name;

    public InAudioNode Parent;

    public bool OverrideParentBus;
    public InAudioBus Bus;

    public List<InAudioNode> Children = new List<InAudioNode>();

#if UNITY_EDITOR
    public bool Filtered = false;

    public bool FoldedOut;

#endif

    [NonSerialized]
    public List<InstanceInfo> CurrentInstances = new List<InstanceInfo>(0);

   
    public InAudioNode GetParent
    {
        get { return Parent; }
        set { Parent = value; }
    }

    public List<InAudioNode> GetChildren
    {
        get { return Children; }
    }

    public string GetName
    {
        get { return Name; }
    }

    public bool IsRoot
    {
        get { return Type == AudioNodeType.Root; }
    }

    public bool IsRootOrFolder
    {
        get { return Type == AudioNodeType.Folder || IsRoot; }
    }

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }

    public bool IsPlayable
    {
        get { return Type != AudioNodeType.Root && Type != AudioNodeType.Folder; }
    }

#if UNITY_EDITOR
    public bool IsFoldedOut
    {
        get { return FoldedOut; }
        set { FoldedOut = value; }
    }

    public bool IsFiltered
    {
        get { return Filtered; }
        set { Filtered = value; }
    }
#endif
}

namespace InAudioSystem
{
    public struct InstanceInfo
    {
        public double Timestamp;
        public InPlayer Player;

        public InstanceInfo(double timestamp, InPlayer player)
        {
            Timestamp = timestamp;
            Player = player;
        }
    }

    

    public enum AudioNodeType
    {
        Root = 0,
        Folder = 1,
        Audio = 2,
        Random = 3,
        Sequence = 4, 
        Voice = 5,
        Multi = 6,
        Track = 7,
    }
}
