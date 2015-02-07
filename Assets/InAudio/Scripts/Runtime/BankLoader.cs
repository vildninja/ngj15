using UnityEngine;

namespace InAudioSystem
{
public static class BankLoader
{

    public static InAudioBank Load(InAudioBankLink bankLink)
    {
        if (bankLink == null)
            return null;

        bankLink.LoadedBank = LoadBank(bankLink);

        if (bankLink.LoadedBank == null)
        {
            Debug.LogError("InAudio: Bank with name \"" + bankLink.Name + "\", id " + bankLink.GUID + " could not be found. Please open the Integrity Window and rebuild Bank Integrity");
            return null;
        }


        if (Application.isPlaying && InAudioInstanceFinder.DataManager != null)
        {
            InAudioInstanceFinder.DataManager.BankIsLoaded(bankLink.LoadedBank);

            bool warning = false;
            for (int i = 0; i < bankLink.LoadedBank.Clips.Count; i++)
            {
                var node = bankLink.LoadedBank.Clips[i].Node;
                if (node == null)
                {
                    warning = true;
                    continue;
                }
                var data = node.NodeData as InAudioData;
                if (data == null)
                {
                    warning = true;
                    continue;
                }
                (bankLink.LoadedBank.Clips[i].Node.NodeData as InAudioData).RuntimeClip =
                    bankLink.LoadedBank.Clips[i].Clip;
            }

            if (warning)
            {
                Debug.LogError("One or more audio nodes are missing from the bank \"" + bankLink.Name + "\", id " +
                                bankLink.ID + ".\nPlease open the InAudio Integrity Window and rebuild the banks");
            }
            bankLink.IsLoaded = true;
        }

        return bankLink.LoadedBank;
    }

    public static InAudioBank LoadBank(InAudioBankLink bankLink)
    {
        if (bankLink == null)
            return null;
        return SaveAndLoad.LoadAudioBank(bankLink.ID);
    }

    public static void Unload(InAudioBankLink bankLink)
    {
        InAudioBank bank = bankLink.LoadedBank;
        if (bank != null)
        {
            for (int i = 0; i < bank.Clips.Count; i++)
            {
                var node = bank.Clips[i].Node;
                if (node != null)
                {
                    var audioData = node.NodeData as InAudioData;
                    if (audioData != null)
                    {
                        audioData.Clip = null;
                    }
                }
            }
            Resources.UnloadUnusedAssets();
            bankLink.IsLoaded = false;
        }

    }

    public static void LoadAutoLoadedBanks()
    {
        LoadAuto(InAudioInstanceFinder.DataManager.BankLinkTree);
    }

    public static GameObject GetBankGO(int id)
    {
        return Resources.Load(FolderSettings.BankLoadFolder + id) as GameObject;
    }

    private static void LoadAuto(InAudioBankLink bankLink)
    {

        if (bankLink == null)
        {
            return;
        }
        if (bankLink.AutoLoad)
            Load(bankLink);

        for (int i = 0; i < bankLink.Children.Count; ++i)
        {
            LoadAuto(bankLink.Children[i]);
        }
    }
}
}