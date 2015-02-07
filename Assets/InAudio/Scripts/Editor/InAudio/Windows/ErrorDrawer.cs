using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

namespace  InAudioSystem.InAudioEditor
{
    public static class ErrorDrawer {
        public static void AddManagerToScene()
        {
            var go = AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject;
            if (go != null)
                PrefabUtility.InstantiatePrefab(go);
            else
            {
                Debug.LogError("The audio manager could not be found in the project.\nEither try and find it manually or reimport InAudio from the Asset Store");
            }
        }

        public static void MissingAudioManager()
        {
            EditorGUILayout.HelpBox("The audio manager could not be found in the scene\nClick the \"Fix it for me\" button or drag the prefab found at \"InAudio/Prefabs/InAudio Manager\" from the Project window into the scene", MessageType.Error, true);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fix it"))
            {
                AddManagerToScene();
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Find Audio Manager Prefab")) 
            {
                EditorApplication.ExecuteMenuItem("Window/Project");
                var go = AssetDatabase.LoadAssetAtPath(FolderSettings.AudioManagerPath, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    EditorGUIUtility.PingObject(go);
                    Selection.objects = new Object[] { go};
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static bool IsDataMissing(InCommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingbus = manager.BusTree == null;
            bool missingBankLink = manager.BankLinkTree == null;

            return missingaudio || missingaudioEvent || missingbus || missingBankLink;
        }

        public static bool IsAllDataMissing(InCommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingbus = manager.BusTree == null;
            bool missingBankLink = manager.BankLinkTree == null;

            return missingaudio && missingaudioEvent && missingbus && missingBankLink;
        }

        public static bool MissingData(InCommonDataManager manager)
        {
            bool missingaudio = manager.AudioTree == null;
            bool missingaudioEvent = manager.EventTree == null;
            bool missingbus = manager.BusTree == null;
            bool missingBankLink = manager.BankLinkTree == null;

            bool areAnyMissing = missingaudio || missingaudioEvent || missingbus || missingBankLink;

            if (areAnyMissing)
            {
                string missingAudioInfo = missingaudio ? "Missing Audio Data\n" : "";
                string missingEventInfo =  missingaudioEvent ? "Missing Event Data\n" : "";
                string missingBusInfo = missingbus ? "Missing Bus Data\n" : "";
                string missingBankLinkInfo = missingBankLink ? "Missing BankLink Data\n" : "";
                EditorGUILayout.HelpBox(missingAudioInfo + missingEventInfo + missingBusInfo + missingBankLinkInfo + "Please go to the Aux Window and create the missing data",
                    MessageType.Error, true);
                if (GUILayout.Button("Open Aux Window"))
                {
                    EditorWindow.GetWindow<AuxWindow>().SelectDataCreation();
                }

                EditorGUILayout.Separator();
                if (GUILayout.Button("Try To Reload Data"))
                {
                    manager.Load(true);
                }
            }
            return areAnyMissing;
        }

        public static bool AllDataMissing(InCommonDataManager manager)
        {
            EditorGUILayout.HelpBox("No InAudio project found. Create InAudio project?", MessageType.Info);
            if (GUILayout.Button("Create InAudio data"))
            {
                MissingDataHelper.StartFromScratch(manager);
            }

            return true;
        }
    }   
}
