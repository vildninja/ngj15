using InAudioSystem;
using InAudioSystem.InAudioEditor;
using UnityEngine;

[AddComponentMenu(FolderSettings.ComponentPathPrefabsGUIPrefs + "GUI User Prefs")]
public class InAudioGUIUserPrefs : MonoBehaviour
{
#if UNITY_EDITOR
    public GUIPrefs AudioGUIData = new GUIPrefs();
    public GUIPrefs EventGUIData = new GUIPrefs();
    public GUIPrefs BusGUIData = new GUIPrefs();
    public GUIPrefs BankGUIData = new GUIPrefs();

    [System.NonSerialized] public InSpline SelectedSplineController;
#endif
}

namespace InAudioSystem.InAudioEditor 
{
    [System.Serializable]
    public class GUIPrefs 
    {
        public int SelectedNode;
        public Vector2 Position;
    }

}
