using InAudioSystem;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

public class IntegrityGUI
{
    public IntegrityGUI(InAudioBaseWindow window)
    {
    }

    public void OnEnable()
    {
           
    }

    public bool OnGUI()
    {
        EditorGUILayout.HelpBox("Do not Undo these operations! No guarantee about what could break.", MessageType.Warning);
        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("While Banks should work every time, it can happen that audio nodes gets deattached from their bank when working in the editor. \nThis will reassign all nodes to their correct bank.", MessageType.Info);
        if (GUILayout.Button("Fix Bank integrity"))
        {
            AudioBankWorker.RebuildBanks(InAudioInstanceFinder.DataManager.BankLinkTree, InAudioInstanceFinder.DataManager.AudioTree);
            Debug.Log("All Banks rebuild");
        }

        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
        if (!UndoHelper.IsNewUndo)
        {
            EditorGUILayout.HelpBox("As you are not using Unity 4.3 or later, there is likely unused audio data stored." +
                                    "\nThis is because data is not always deleted since the Undo system does not support undo of deletion." +
                                    "\nCleanup will remove all unused data.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("No nodes should be unused, but in the case there is this will remove all unused data.\nNo performance is lost if unused nodes remains, but it does waste a bit of memory. This will clean up any unused data", MessageType.Info);
        }

        if (GUILayout.Button("Clean up unused data"))
        {
            DataCleanup.Cleanup();
        }
        return false;
    }

}
