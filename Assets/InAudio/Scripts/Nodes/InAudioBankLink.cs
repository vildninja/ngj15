using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

namespace InAudioSystem
{

    public enum AudioBankTypes
    {
        Folder, Link
   }
}

public class InAudioBankLink : MonoBehaviour, InITreeNode<InAudioBankLink>
{
    public int GUID;

    public AudioBankTypes Type;

    public string Name;

    public InAudioBankLink Parent;

    public List<InAudioBankLink> Children = new List<InAudioBankLink>();

    public bool AutoLoad = false;

    [System.NonSerialized]
    public InAudioBank LoadedBank;

#if UNITY_EDITOR
    public bool FoldedOut;

    public bool Filtered = false;
#endif

    public InAudioBank LazyBankFetch
    {
        get
        {
            if (LoadedBank == null)
            {
                LoadedBank = BankLoader.Load(this);
            }

            return LoadedBank;
        }
    }

    [System.NonSerialized]
    private bool isLoaded;

    public bool IsLoaded
    {
        get
        {
            return isLoaded;
        }
        set
        {
            isLoaded = value;
        }
    }

    public InAudioBankLink GetParent
    {
        get
        {
            return Parent;
        }
        set
        {
            Parent = value;
        }
    }

    public List<InAudioBankLink> GetChildren
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
