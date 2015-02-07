using InAudioSystem;
using InAudioSystem.InAudioEditor;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;

public class AudioEventCreatorGUI : BaseCreatorGUI<InAudioEventNode>
{
    public AudioEventCreatorGUI(EventWindow window)
        : base(window)
    {
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
        Rect treeArea = EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth), GUILayout.Height(height ));
        DrawSearchBar();
        EditorGUILayout.BeginVertical();

        isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.EventTree, treeArea);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawRightSide(Rect area)
    {
        EditorGUILayout.BeginVertical();

        if (SelectedNode != null)
        {
            isDirty |= AudioEventDrawer.Draw(SelectedNode);
        }

        EditorGUILayout.EndVertical();
    }

    public override void OnEnable()
    {
        treeDrawer.Filter(n => false);
        treeDrawer.OnNodeDraw = EventDrawer.EventFoldout;
        treeDrawer.OnContext = OnContext;
        treeDrawer.CanDropObjects = CanDropObjects;
        treeDrawer.OnDrop = OnDrop;
    }

    protected override void OnDrop(InAudioEventNode audioevent, Object[] objects)
    {
        AudioEventWorker.OnDrop(audioevent, objects);
        treeDrawer.SelectedNode = audioevent;
    }

    protected override bool CanDropObjects(InAudioEventNode audioEvent, Object[] objects)
    {
        return AudioEventWorker.CanDropObjects(audioEvent, objects);
    }

    protected override void OnContext(InAudioEventNode node)
    {
        var menu = new GenericMenu();

        #region Duplicate
#if !UNITY_4_1 && !UNITY_4_2
        if (!node.IsRoot)
            menu.AddItem(new GUIContent("Duplicate"), false, data => AudioEventWorker.Duplicate(data as InAudioEventNode),
                node);
        else
            menu.AddDisabledItem(new GUIContent("Duplicate"));
        menu.AddSeparator("");
#endif
        #endregion


        #region Create child

        if (node.Type == EventNodeType.Root || node.Type == EventNodeType.Folder)
        {
            menu.AddItem(new GUIContent(@"Create Event"), false,
                data => { CreateChild(node, EventNodeType.Event); }, node);
            menu.AddItem(new GUIContent(@"Create Folder"), false,
                data => { CreateChild(node, EventNodeType.Folder); }, node);

        }
        if (node.Type == EventNodeType.Event)
        {
            menu.AddDisabledItem(new GUIContent(@"Create Event"));
            menu.AddDisabledItem(new GUIContent(@"Create Folder"));
            
        }

        #endregion

        menu.AddSeparator("");

        #region Delete
        menu.AddItem(new GUIContent(@"Delete"), false, data => {
                treeDrawer.SelectedNode = TreeWalker.GetPreviousVisibleNode(treeDrawer.SelectedNode);
                AudioEventWorker.DeleteNode(node);
            }, node);
        #endregion
        menu.ShowAsContext();
    }
     
    private void CreateChild(InAudioEventNode node, EventNodeType type)
    {
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RegisterUndo(node, "Event Creation");
            AudioEventWorker.CreateNode(node, type);    
        });
        
        node.FoldedOut = true;
    }

    protected override InAudioEventNode Root()
    {
        return InAudioInstanceFinder.DataManager.EventTree;
    }

    protected override GUIPrefs GUIData
    {
        get { return InAudioInstanceFinder.InAudioGuiUserPrefs.EventGUIData; }
    }
}
