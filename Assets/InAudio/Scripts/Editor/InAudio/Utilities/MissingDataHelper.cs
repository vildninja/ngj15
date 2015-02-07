using System;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class MissingDataHelper
    {
        public static void StartFromScratch(InCommonDataManager Manager)
        {
            SystemFolderHelper.CreateIfMissing(FolderSettings.BankCreateFolder);
            try
            {
                DataCleanup.Cleanup(DataCleanup.CleanupVerbose.Silent);
            }
            catch (Exception)
            {
                //Do nothing as something is seriously wrong
            }


            int levelSize = 3;
            GameObject audioGO = new GameObject();
            GameObject eventGO = new GameObject();
            GameObject busGO = new GameObject();
            GameObject bankGO = new GameObject();

            Manager.BusTree = AudioBusWorker.CreateTree(busGO);
            Manager.BankLinkTree = AudioBankWorker.CreateTree(bankGO);
            Manager.AudioTree = AudioNodeWorker.CreateTree(audioGO, levelSize, Manager.BusTree);

            Manager.EventTree = AudioEventWorker.CreateTree(eventGO, levelSize);

            SaveAndLoad.CreateDataPrefabs(Manager.AudioTree.gameObject, Manager.EventTree.gameObject, Manager.BusTree.gameObject,
                Manager.BankLinkTree.gameObject);

            Manager.Load(true);

            if (Manager.BankLinkTree != null)
            {
                var bankLink = Manager.BankLinkTree.Children[0];
                bankLink.Name = "Default - Auto loaded";
                bankLink.AutoLoad = true;

                NodeWorker.AssignToNodes(Manager.AudioTree, node =>
                {
                    var data = (node.NodeData as InFolderData);
                    if (data != null)
                        data.BankLink = Manager.BankLinkTree.GetChildren[0];
                });

                NodeWorker.AssignToNodes(Manager.AudioTree, node => node.Bus = Manager.BusTree);

                AssetDatabase.Refresh();
                DataCleanup.Cleanup(DataCleanup.CleanupVerbose.Silent);
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
            }
            else
            {
                Debug.LogError("There was a problem creating the data.");
            }
        }
    }

}