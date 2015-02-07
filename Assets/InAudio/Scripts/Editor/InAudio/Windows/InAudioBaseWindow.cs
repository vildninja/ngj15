using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

public class InAudioBaseWindow : EditorWindow
{
    public InCommonDataManager Manager;

    protected int topHeight = 0;
    protected int LeftWidth = 350;

    protected bool isDirty;

    protected void BaseEnable()
    {
        autoRepaintOnSceneChange = true;
        EditorApplication.modifierKeysChanged += Repaint;

        Manager = InAudioInstanceFinder.DataManager;
        minSize = new Vector2(400,100);

        EditorResources.Reload();
    }

    protected void BaseUpdate()
    {
        if (Event.current != null)
        {
            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        Repaint();
                        break;
                }
            }
        }
    }

    protected void CheckForClose()
    {
        if (Event.current.IsKeyDown(KeyCode.W) && Event.current.modifiers == EventModifiers.Control)
        {
            Close();
        }
    }

    protected bool HandleMissingData()
    {
        if (Manager == null)
        {
            Manager = InAudioInstanceFinder.DataManager;
            if (Manager == null)
            {
                ErrorDrawer.MissingAudioManager();
            }
        }

        if (Manager != null)
        {
            bool areAnyMissing = ErrorDrawer.IsDataMissing(Manager);

            if (areAnyMissing)
            {
                Manager.Load();
            }
            if (ErrorDrawer.IsAllDataMissing(Manager))
            {
                ErrorDrawer.AllDataMissing(Manager);
                return false;
            }
            if (ErrorDrawer.IsDataMissing(Manager))
            {
                ErrorDrawer.MissingData(Manager);
                return false;
            }
            else
            {
                return true;
            }
        }
        else 
            return false;
    }

    protected void PostOnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            //GUIUtility.keyboardControl = 0;
        }
    }

    protected bool IsKeyDown(KeyCode code)
    {
        return Event.current.type == EventType.keyDown && Event.current.keyCode == code;
    }

}
