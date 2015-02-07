using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

public class InAudioEventNode : MonoBehaviour, InITreeNode<InAudioEventNode>
{
    public int GUID;

    public EventNodeType Type;

    public bool FoldedOut;

    public string Name;

    public InAudioEventNode Parent;

    public float Delay;

    public bool Filtered;

    public List<InAudioEventNode> Children = new List<InAudioEventNode>();

    public List<AudioEventAction> ActionList = new List<AudioEventAction>();

    public void AssignParent(InAudioEventNode node)
    {
        node.Children.Add(this);
        Parent = node;
    }

    public InAudioEventNode GetParent
    {
        get { return Parent; }
        set { Parent = value; }
    }

    public List<InAudioEventNode> GetChildren
    {
        get { return Children; }
    }

    public bool IsFoldedOut
    {
        get
        {
            return FoldedOut;
        }
        set
        {
            FoldedOut = value;
        }
    }

    public string GetName
    {
        get { return Name; }
    }

    public bool IsRoot
    {
        get { return Type == EventNodeType.Root; }
    }

    public bool IsFiltered
    {
        get { return Filtered; }
        set { Filtered = value; }
    }

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }
}

namespace InAudioSystem
{
    public enum EventNodeType
    {
        Root,
        Folder,
        EventGroup,
        Event
    }
}
