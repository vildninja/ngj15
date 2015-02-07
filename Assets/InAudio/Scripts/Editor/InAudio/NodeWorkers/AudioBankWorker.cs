using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem
{
 
public static class AudioBankWorker {
    private static InAudioBankLink CreateNode(GameObject go, InAudioBankLink parent, int guid)
    {
        var node = go.AddComponentUndo<InAudioBankLink>();
        node.GUID = guid;
        node.IsFoldedOut = true;
        node.AssignParent(parent);
        return node;
    }

    private static InAudioBankLink CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<InAudioBankLink>();
        node.GUID = guid;
        node.IsFoldedOut = true;
        node.Name = "Root";
        node.Type = AudioBankTypes.Folder;
        return node;
    }
 
    public static InAudioBankLink CreateFolder(GameObject go, InAudioBankLink parent, int guid)
    {
        var node = CreateNode(go, parent, guid);
        node.Name = parent.Name + " Child Folder";
        node.Type = AudioBankTypes.Folder;
        return node;
    }

    private static InAudioBankLink CreateBankLink(GameObject go, InAudioBankLink parent, int guid)
    {
        var node = CreateNode(go, parent, guid);
        node.Name = parent.Name + " Child"; 
        node.Type = AudioBankTypes.Link;
        return node;
    }

    public static InAudioBankLink CreateBank(GameObject go, InAudioBankLink parent, int guid)
    {
        InAudioBankLink link = CreateBankLink(go, parent, guid);

        SaveAndLoad.CreateAudioBank(guid);
        return link;
    }

    public static InAudioBankLink CreateTree(GameObject go)
    {
        var root = CreateRoot(go, GUIDCreator.Create());
        var link = CreateBankLink(go, root, GUIDCreator.Create());
        SaveAndLoad.CreateAudioBank(link.GUID);
        return root;
    }

    public static bool SwapClipInBank(InAudioNode node, AudioClip newClip)
    {
        var bank = node.GetBank();
        
        var clipTuple = bank.LazyBankFetch.Clips;

        for (int i = 0; i < clipTuple.Count; i++)
        {
            if (clipTuple[i].Node == node)
            {
                
                clipTuple[i].Clip = newClip;

                return true;
            }
        }
        return false;
    }

    public static void AddNodeToBank(InAudioNode node, AudioClip clip)
    {
        var bank = node.GetBank();
        if (bank != null)
        {
            if (bank.LazyBankFetch == null)
            {
                Debug.LogError("Please open the InAudio Integrity window and \"Fix Bank integrity\"\n"
                    +"Bank " + bank.Name + " with id " + bank.ID + " does not have an attached bank storage.\n");               
            }
            else {
                bank.LazyBankFetch.Clips.Add(CreateTuple(node, clip));
                EditorUtility.SetDirty(bank.LazyBankFetch);
            }

        }
    }

    public static void RemoveNodeFromBank(InAudioNode node)
    {
        var bankLink = node.GetBank();
        if (bankLink != null)
        {
            
            var bank = bankLink.LazyBankFetch;
            UndoHelper.RecordObjectFull(bank, "Node from bank removal");
            bank.Clips.RemoveAll(p => p.Node == node);
        }
    }

    private static BankTuple CreateTuple(InAudioNode node, AudioClip clip)
    {
        BankTuple tuple = new BankTuple();
        tuple.Node = node;
        tuple.Clip = clip;
        return tuple;
    }


    public static void RebuildBanks(InAudioBankLink rootBank, InAudioNode root)
    {
        SystemFolderHelper.CreateIfMissing(FolderSettings.BankCreateFolder);
        TreeWalker.ForEach(rootBank, CreateIfMissing);
        TreeWalker.ForEach(rootBank, DeleteAllNodesFromBanks);
        TreeWalker.ForEach(root, node =>
        {
            var folderData = node.NodeData as InFolderData;
            if (folderData != null && folderData.BankLink == null)
            {
                folderData.BankLink = rootBank;
            }
        });
        TreeWalker.ForEach(root, AddNodesToBank);

        //Set the bank of the root node if it is missing
        InFolderData inFolderData = root.NodeData as InFolderData;
        if(inFolderData != null && inFolderData.BankLink == null)
        {
            inFolderData.BankLink = TreeWalker.FindFirst(rootBank, link => link.Type == AudioBankTypes.Link);
        }
        if (inFolderData.BankLink != null)
            TreeWalker.ForEach(root, SetBanks);
    }

    private static void CreateIfMissing(InAudioBankLink bankLink)
    {
        
        if (bankLink != null && bankLink.Type == AudioBankTypes.Link)
        {
            bankLink.LoadedBank = BankLoader.LoadBank(bankLink);
            if (bankLink.LoadedBank == null)
            {
                GameObject go = BankLoader.GetBankGO(bankLink.ID);
                if (go == null)
                {
                    SaveAndLoad.CreateAudioBank(bankLink.ID);
                    Debug.Log("Created missing Bank "+bankLink.Name + " with id " + bankLink.ID);
                }
                else
                {
                    var bank = go.AddComponent<InAudioBank>();
                    bank.GUID = bankLink.ID;
                    Debug.Log("Created missing Bank \n" + bankLink.Name + "\n with id " + bankLink.ID);
                }
            }
        }
    }

    private static void SetBanks(InAudioNode node)
    {
        if(node.IsRootOrFolder)
        {
            InFolderData inFolderData = (node.NodeData as InFolderData);
            if(inFolderData != null)
            {
                if (inFolderData.BankLink == null)
                    inFolderData.BankLink = node.GetBank();
            }
        }
    }

    public static void ChangeBank(InAudioNode node, InAudioBankLink newBank, InAudioBankLink rootBank, InAudioNode root)
    {
        var all = TreeWalker.FindAll(rootBank, link => link.Type == AudioBankTypes.Link ? link.LazyBankFetch : null);
        UndoHelper.RecordObject(all.ToArray().AddObj(node.NodeData), "Changed Bank");
        InFolderData data = (node.NodeData as InFolderData);
        data.BankLink = newBank;
        RebuildBanks(rootBank, root);
    }

    public static void ChangeBankOverride(InAudioNode node, InAudioBankLink rootBank, InAudioNode root)
    {
        var all = TreeWalker.FindAll(rootBank, link => link.Type == AudioBankTypes.Link ? link.LazyBankFetch : null);
        
        UndoHelper.RecordObject(all.ToArray().AddObj(node.NodeData), "Changed Bank");
        InFolderData data = (node.NodeData as InFolderData);
        data.OverrideParentBank = !data.OverrideParentBank;
        RebuildBanks(rootBank, root);        
    }

    private static void AddNodesToBank(InAudioNode audioNode)
    {
        if (audioNode.Type == AudioNodeType.Audio)
        {
            var nodeData = audioNode.NodeData as InAudioData;
            if (nodeData != null)
            {
                AddNodeToBank(audioNode, nodeData.EditorClip);
            }
        }
    }

    private static void DeleteAllNodesFromBanks(InAudioBankLink audioBankLink)
    {
        if (audioBankLink.Type == AudioBankTypes.Link)
        {
            if (audioBankLink.LazyBankFetch != null)
                audioBankLink.LazyBankFetch.Clips.Clear();
        }
    }

    public static void DeleteBank(InAudioBankLink toDelete)
    {        
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(toDelete.LazyBankFetch.gameObject));
        toDelete.Parent.GetChildren.Remove(toDelete);
        Object.DestroyImmediate(toDelete, true);
    }

    public static void DeleteFolder(InAudioBankLink toDelete)
    {
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RecordObjectFull(toDelete.Parent, "Delete Bank Folder");
            toDelete.Parent.GetChildren.Remove(toDelete);
            UndoHelper.Destroy(toDelete);
        });
    }
}
}
