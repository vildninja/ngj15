using System;
using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;
using Object = UnityEngine.Object;

[AddComponentMenu(FolderSettings.ComponentPathPrefabsManager+"Common Data Manager")]
public class InCommonDataManager : MonoBehaviour {
    
    /*[SerializeField]
    private GameObject AudioRootGO;
    [SerializeField]
    private GameObject EventRootGO;
    [SerializeField]
    private GameObject BusRootGO;
    [SerializeField]
    private GameObject BankLinkRootGO;*/

    private InAudioNode AudioRoot; 
    private InAudioEventNode EventRoot;    
    private InAudioBus BusRoot;
    private InAudioBankLink BankLinkRoot; 

    public InAudioNode AudioTree
    {
        get { return AudioRoot; }
        set { AudioRoot = value; }
    }

    public InAudioEventNode EventTree
    {
        get { return EventRoot; }
        set { EventRoot = value; }
    }

    public InAudioBus BusTree
    {
        get { return BusRoot; }
        set { BusRoot = value; }
    }

    public InAudioBankLink BankLinkTree
    {
        get { return BankLinkRoot; }
        set { BankLinkRoot = value; }
    }

    //Only set in runtime
    private List<InAudioBank> LoadedBanks = new List<InAudioBank>();

    public void BankIsLoaded(InAudioBank bank)
    {
        LoadedBanks.Add(bank);
    }

    public void Load(bool forceReload = false)
    {
        if (AudioRoot == null || BankLinkRoot == null || BusRoot == null || EventRoot == null || forceReload)
        {
            Component[] audioData;
            Component[] eventData;
            Component[] busData;
            Component[] bankLinkData;

            SaveAndLoad.LoadManagerData(out audioData, out eventData, out busData, out bankLinkData);
            BusRoot         = CheckData<InAudioBus>(busData);
            AudioRoot       = CheckData<InAudioNode>(audioData);
            EventRoot       = CheckData<InAudioEventNode>(eventData);
            BankLinkTree    = CheckData<InAudioBankLink>(bankLinkData);
            /*if (BusRoot != null)
                BusRootGO = BusRoot.gameObject;
            if (AudioRoot != null)
                AudioRootGO = AudioRoot.gameObject;
            if (EventRoot != null)
                EventRootGO = EventRoot.gameObject;
            if (BankLinkTree != null)
                BankLinkRootGO = BankLinkTree.gameObject;*/
        }
    }

    //Does the components actually exist and does it have a root?
    private T CheckData<T>(Component[] data) where T : Object, InITreeNode<T>
    {
        if (data != null && data.Length > 0 && data[0] as T != null)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] as InITreeNode<T> != null)
                {
                    if((data[i] as InITreeNode<T>).IsRoot)
                    {
                        return data[i] as T;
                    }
                }
            }
        }
        return null; 
    }

    //Does the components actually exist and does it have a root?
    private T CheckData<T>(Component[] data, Func<T, bool> predicate) where T : Object, InITreeNode<T>
    {
        if (data != null && data.Length > 0 && data[0] as T != null)
        {
            return TreeWalker.FindFirst(data[0] as T, predicate);
        }
        return null;
    } 

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        //Instance = this;
        Load();
    }
}
