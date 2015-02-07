using System.Collections.Generic;
using InAudioSystem;
using InAudioSystem.Runtime;
using UnityEngine;

public class InAudioBus : MonoBehaviour, InITreeNode<InAudioBus>
{
    /// <summary>
    /// Volume set in the editor
    /// </summary>
    public float Volume = 1.0f;

    /// <summary>
    /// Initial runtime volume
    /// </summary>
   public float SelfVolume = 1.0f;

#if UNITY_EDITOR
    /// <summary>
   /// Only to easily visualize the hiarchy volume. Only used in BusDrawer
    /// </summary>
    public float CombinedVolume = 1.0f;
#endif

    public int GUID;

    public string Name;

    public InAudioBus Parent;

    public List<InAudioBus> Children = new List<InAudioBus>();

    public bool Mute = false;

    public List<DuckingData> DuckedBy = new List<DuckingData>();

    /// <summary>
    /// Do we need to update the attach audio players?
    /// </summary>
    [System.NonSerialized]
    public bool Dirty = true;

    [System.NonSerialized] public bool RuntimeMute = false;

    /// <summary>
    /// The nodes during runtime that is in this bus
    /// </summary>
    [System.NonSerialized]
    public List<InPlayer> NodesInBus = new List<InPlayer>();

    /// <summary>
    /// The volume to set it's children to. The final volume of the bus so to say
    /// </summary>
    [System.NonSerialized]
    public float FinalVolume = 1.0f;

    /// <summary>
    /// What the volume for itself is, set by SelfVolume when the game starts
    /// </summary>
    [System.NonSerialized] 
    public float RuntimeSelfVolume = 1.0f;

    /// <summary>
    /// Whether the bus is playing something (including children)
    /// </summary>
    [System.NonSerialized]
    public bool IsActive = false;

    /// <summary>
    /// The last ducking volume caluclated, goes between -1 <-> 0
    /// </summary>
    [System.NonSerialized]
    public float LastDuckedVolume = 0.0f;

    /// <summary>
    /// 
    /// </summary>
    [System.NonSerialized]
    public Fader Fader = new Fader();

    //External audio sources in the game
    [System.NonSerialized] 
    public List<ExternalAudioSource> ExternalSources = new List<ExternalAudioSource>(0);


#if UNITY_EDITOR
    public bool FoldedOut;

    public bool Filtered = false;
    
#endif

    public List<InPlayer> RuntimePlayers
    {
        get 
        {
            return NodesInBus;
        }
    }

    public InAudioBus GetParent
    {
        get { return Parent; }
        set { Parent = value; }
    }

    public List<InAudioBus> GetChildren
    {
        get { return Children; }
    }


    public string GetName
    {
        get { return Name; }
    }

    public bool IsRoot
    {
        get { return Parent == null; }
    }

    public int ID
    {
        get { return GUID; }
        set { GUID = value; }
    }

    
    #if UNITY_EDITOR
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

    public bool IsFiltered
    {
        get
        {
            return Filtered;
        }
        set
        {
            Filtered = value;
        }
    }
    #endif
}
