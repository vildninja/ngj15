using System;
using UnityEngine;

namespace InAudioSystem
{
    public static class FolderSettings
    {

        private const string Name = "InAudio";

        private const string RelativePathResources = "InAudio/";

        public const string AudioLoadData       = RelativePathResources + "InAudio AudioSave";
        public const string EventLoadData       = RelativePathResources + "InAudio EventSave";
        public const string BusLoadData         = RelativePathResources + "InAudio BusSave";
        public const string BankLinkLoadData    = RelativePathResources + "InAudio BankLinkSave";

        public const string BankLoadFolder = RelativePathResources + "Banks/InAudio Bank ";
               
        public const string ComponentPathInternal = "InAudio/Internal/";
        public const string ComponentPathInternalPools = "InAudio/Internal/Pools/";
        public const string ComponentPathInternalData = "InAudio/Internal/Data/";

        public const string ComponentPathPrefabs = "InAudio/Prefab Scripts/";
        public const string ComponentPathPrefabsManager = ComponentPathPrefabs + "Manager/";
        public const string ComponentPathPrefabsGUIPrefs = ComponentPathPrefabs + "GUI Prefs/";


#if UNITY_EDITOR
        private readonly static string FullPathResources = AssetFolder + "InAudio/Resources/InAudio/";

        //Prefabs
        public readonly static string GUIUserPrefs          = AssetFolder + Name + "/Prefabs/GUIUserPrefs.prefab";
        public readonly static string AudioManagerPath      = AssetFolder + Name + "/Prefabs/InAudio Manager.prefab";
        public readonly static string EmptyTemplatePath     = AssetFolder + Name + "/Prefabs/PrefabTemplate.prefab";

        //Data
        public readonly static string AudioSaveDataPath     = FullPathResources + "InAudio AudioSave.prefab";
        public readonly static string EventSaveDataPath     = FullPathResources + "InAudio EventSave.prefab";
        public readonly static string BusSaveDataPath       = FullPathResources + "InAudio BusSave.prefab";
        public readonly static string BankLinkSaveDataPath  = FullPathResources + "InAudio BankLinkSave.prefab";
        public readonly static string AudioDataPath         = FullPathResources + "AudioData/";

        //Creating the folders
        public static readonly string CreateFolderResources = AssetFolder + Name + "/Resources/" + Name + "/";
        public static string BankCreateFolder       = CreateFolderResources + "Banks/";
        public static string AudioDataCreateFolder  = CreateFolderResources + "AudioData/";

        //For cleanup
        public readonly static string BankDeleteDictory = FullPathResources + "Banks/";
        private const string BankRelativeDictory = "/" + Name + "/Resources/" + Name + "/" + "Banks/";
        public readonly static string FullBankPath = Application.dataPath + "/" + (AssetFolder.TrimStart("Assets/".ToCharArray())) + BankRelativeDictory;
       

        public static string GetIconPath(string name)
        {
            return AssetFolder + "InAudio/Icons/" + name + ".png";
        }

        public static string GetNewBankPath(int guid)
        {        
            string bankSaveLocation = FullPathResources + "Banks/InAudio Bank ";
            return bankSaveLocation + guid + ".prefab";
        }
        
        private static string relativePath;
        private static string AssetFolder
        {
            get
            { 
                if (relativePath == null)
                    relativePath = BuildPath();
                return relativePath;
            }
        }

        private static string BuildPath()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(Resources.Load("InAudio/Root"));
            if (path != null)
            {
                return (path.Split(new[] { "InAudio" }, StringSplitOptions.None)[0]);
            }
            return path;
        }
#endif
    }

}
