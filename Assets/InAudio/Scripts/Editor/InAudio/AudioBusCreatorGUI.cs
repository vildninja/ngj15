using InAudioSystem;
using InAudioSystem.InAudioEditor;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;

public class AudioBusCreatorGUI : BaseCreatorGUI<InAudioBus>
{
    public AudioBusCreatorGUI(AuxWindow window)
        : base(window)
    {
        this.window = window;
    }

    private int leftWidth;
    private int height;

    public bool OnGUI(int leftWidth, int height)
    {
        BaseOnGUI();

        this.leftWidth = leftWidth;
        this.height = height;

        EditorGUIHelper.DrawColums(DrawLeftSide, DrawRightSide);

        return isDirty;
    }

    private void DrawLeftSide(Rect area)
    {
        Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height));
        DrawSearchBar();

        EditorGUILayout.BeginVertical();
        treeArea.y -= 25;
        //treeArea.height += 10;
        isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.BusTree, treeArea);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawRightSide(Rect area)
    {
        if (treeDrawer.SelectedNode != null)
        {
            AudioBusDrawer.Draw(treeDrawer.SelectedNode);
            //InAudioInstanceFinder.DataManager.BusTree.Dirty = true;
            //AudioBusVolumeHelper.UpdateCombinedVolume(InAudioInstanceFinder.DataManager.BusTree);

        }
    }

    protected override bool CanDropObjects(InAudioBus node, Object[] objects)
    {
        if (objects != null && objects.Length > 0 && objects[0] as InAudioBus != null)
        {
            return !NodeWorker.IsChildOf(objects[0] as InAudioBus, node);
        }
        return false;
    }

    protected override void OnDrop(InAudioBus dropOn, Object[] objects)
    {
        var beingDragged = objects[0] as InAudioBus;

        UndoHelper.RecordObject(new Object[] {dropOn, beingDragged, beingDragged.Parent},"Bus drag n drop");
        
        NodeWorker.ReasignNodeParent(beingDragged, dropOn);            
    }

    protected override void OnContext(InAudioBus audioBus)
    {
        var menu = new GenericMenu();

        menu.AddItem(new GUIContent(@"Create Bus"), false, CreateChildBus, audioBus);

        menu.AddSeparator("");

        if (!audioBus.IsRoot)
            menu.AddItem(new GUIContent(@"Delete"), false, data => {
                //treeDrawer.SelectPreviousNode();
                DeleteBus(audioBus);
            }, audioBus);
        else
            menu.AddDisabledItem(new GUIContent(@"Delete"));

        menu.ShowAsContext();
    }

    private void CreateChildBus(object userData)
    {
        InAudioBus bus = userData as InAudioBus;
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RecordObjectFull(bus, "Bus Creation");
            AudioBusWorker.CreateChild(bus);    
        });
        
    }

    protected override bool OnNodeDraw(InAudioBus node, bool isSelected)
    {
        return BusDrawer.Draw(node, isSelected);
    }

    private void DeleteBus(InAudioBus bus)
    {
        //UndoHelper.DoInGroupWithWarning(() =>
        {
            //UndoHelper.RegisterUndo(bus.Parent, "Bus Deletion");
            AudioBusWorker.DeleteBus(bus, InAudioInstanceFinder.DataManager.AudioTree);
        }
        //);
        
    }

    protected override InAudioBus Root()
    {
        return InAudioInstanceFinder.DataManager.BusTree;
    }

    protected override GUIPrefs GUIData
    {
        get { return InAudioInstanceFinder.InAudioGuiUserPrefs.BusGUIData; }
    }
}
