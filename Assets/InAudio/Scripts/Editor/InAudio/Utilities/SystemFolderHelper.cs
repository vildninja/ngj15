
using System;
using System.IO;
using System.Linq;
using InAudioSystem;
using UnityEditor;

public static class SystemFolderHelper
{

    //TODO Convert to http://docs.unity3d.com/Documentation/ScriptReference/AssetDatabase.CreateFolder.html
    public static void DeleteFolderContent(string path)
    {
        System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);
        if (directory.Exists)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles())
                file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }
        else
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }

    public static int DeleteUnusedBanks(InAudioBankLink bankRoot)
    {
        if (bankRoot == null)
            return 0;
        if (!System.IO.Directory.Exists(FolderSettings.FullBankPath))
        {
            return 0;
        }

        FileInfo[] banks = GetPrefabsAtPath(FolderSettings.FullBankPath);
        int deleteCount = 0;
        for (int i = 0; i < banks.Length; ++i)
        {
            string name = banks[i].Name;

            //Get the ID which is only numbers
            name = new String(name.Split(new[] { ".prefab" }, StringSplitOptions.RemoveEmptyEntries)[0].Where(Char.IsDigit).ToArray());
            
            InAudioBankLink bankLink = null;
            
            int id = Convert.ToInt32(name, 10);
            if (bankRoot != null)
                bankLink = TreeWalker.FindById(bankRoot, id);

            if (bankLink == null)
            {
                AssetDatabase.DeleteAsset(FolderSettings.BankDeleteDictory + banks[i].Name);
                ++deleteCount;
            }
        }
        return deleteCount;
    }

    private static FileInfo[] GetPrefabsAtPath(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        return dir.GetFiles("*.prefab");
    }

    public static void CreateIfMissing(string path)
    {
        System.IO.Directory.CreateDirectory(path);
    }
}
