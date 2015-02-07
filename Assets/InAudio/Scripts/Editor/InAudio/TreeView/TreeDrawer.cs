using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem
{

public class TreeDrawer<T> where T : UnityEngine.Object, InITreeNode<T>
{
    public T SelectedNode
    {
        get { return selectedNode; }
        set { selectedNode = value; }
    }


    public bool IsDirty
    {
        get;
        private set;
    }

    public Vector2 ScrollPosition;

    public delegate void OnContextDelegate(T node);
    public OnContextDelegate OnContext;

    public delegate bool OnNodeDrawDelegate(T node, bool isSelected);
    public OnNodeDrawDelegate OnNodeDraw;

    public delegate void OnDropDelegate(T node, UnityEngine.Object[] objects);
    public OnDropDelegate OnDrop;

    public delegate bool CanDropObjectsDelegate(T node, UnityEngine.Object[] objects);
    public CanDropObjectsDelegate CanDropObjects;

    private T selectedNode;
    private T draggingNode;
    private Rect selectedArea;

    //When dragging something over the tree
    private T HoverOver;
    private Rect HoverOverArea;

    private bool triggerFilter = false;
    private Func<T, bool> filterFunc;

    private bool canDropObjects;
    private Vector2 dragStart;
    private bool wantToDrag;
    private bool focusOnSelectedNode;

    private int controlID;

    private Rect _area;

    private T root;

    private float maxY;
    private bool clickedWithin = false;

    private Vector2 clickPos;

    public bool DrawTree(T treeRoot, Rect area)
    {
        var fullArea = area.Add(ScrollPosition);

        if (Event.current.type == EventType.MouseDown)
        {
            if (fullArea.Contains(Event.current.mousePosition))
            {
                clickedWithin = true;
                clickPos = Event.current.mousePosition;
            }
            else
            {
                clickedWithin = false;
            }
        }

        if (SelectedNode == null)
            selectedNode = treeRoot;

        if (SelectedNode == null) //If it's still null
            return false;

        int startIndent = EditorGUI.indentLevel;
        ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, true);
        
        if (treeRoot == null || OnNodeDraw == null)
            return true;

        root = treeRoot;
        _area = area;

       

        if (selectedNode.IsFiltered)
            selectedNode = treeRoot;

        if (triggerFilter)
        {
            FilterNodes(treeRoot, filterFunc);
            
            triggerFilter = false;
            IsDirty = true;
        }

        IsDirty = false;

        bool canDropObject = false;
        Vector2 mousePos = Event.current.mousePosition;

        if (fullArea.Contains(mousePos) )
        {
            canDropObject = HandleDragging();
        }


        DrawTree(treeRoot, EditorGUI.indentLevel);

        PostDrawDragHandle(canDropObject);
         
        ContextHandle();
      
        KeyboardControl();

        EditorGUILayout.EndScrollView();

        EditorGUI.indentLevel = startIndent;

        if (focusOnSelectedNode)
        {
            ScrollPosition.y = selectedArea.y;
            focusOnSelectedNode = false;
        }

        return IsDirty;
    }

    //Draw all nodes recursively 
    void DrawTree(T node, int indentLevel)
    {
        if (node != null)
        {
            if (node.IsFiltered)
                return;
            EditorGUI.indentLevel = indentLevel + 1;
            DrawNode(node);
            EditorGUI.indentLevel = indentLevel - 1;

            if (!node.IsFoldedOut)
                return;
            
            if(Event.current.type == EventType.Layout)
                NodeWorker.RemoveNullChildren(node);
            for (int i = 0; i < node.GetChildren.Count; ++i)
            {
                T child = node.GetChildren[i];
                DrawTree(child, indentLevel + 1);
            }
        }
    }

    private void ContextHandle()
    {
        Rect area = _area;
        area.y += ScrollPosition.y;
        if (Event.current.type == EventType.ContextClick && area.Contains(Event.current.mousePosition) && selectedArea.Contains(Event.current.mousePosition) && OnContext != null)
        {
            OnContext(selectedNode);

            Event.current.Use();
        }
    }

    private void KeyboardControl()
    {
        #region keyboard control

        if (GUIUtility.keyboardControl != 0)
            return;

        bool hasPressedDown = false;
        bool hasPressedUp = false;
        if (GUIUtility.keyboardControl != 0)
            return;

        if (Event.current.IsKeyDown(KeyCode.RightArrow))
        {
            SelectedNode.IsFoldedOut = true;
            Event.current.Use();
        }
        if (Event.current.IsKeyDown(KeyCode.LeftArrow))
        {
            SelectedNode.IsFoldedOut = false;
            Event.current.Use();
        }

        if (Event.current.IsKeyDown(KeyCode.UpArrow))
        {
            hasPressedUp = true;
            selectedNode = TreeWalker.FindPreviousUnfoldedNode(selectedNode, arg => !arg.IsFiltered);
            Event.current.Use();
        }
        if (Event.current.IsKeyDown(KeyCode.DownArrow))
        {
            hasPressedDown = true;
            selectedNode = TreeWalker.FindNextNode(SelectedNode, arg => !arg.IsFiltered );
            Event.current.Use();
        }
        if (Event.current.IsKeyDown(KeyCode.Home))
        {
            ScrollPosition = new Vector2();
            selectedNode = root;
        }
        if (Event.current.IsKeyDown(KeyCode.End))
        {
            //selectedNode = TreeWalker.;
        }

        if (hasPressedDown && (_area.y + ScrollPosition.y + _area.height - selectedArea.height * 2 < selectedArea.y + selectedArea.height))
        {
            ScrollPosition.y += selectedArea.height;
        }

        if (hasPressedUp && (_area.y + ScrollPosition.y + selectedArea.height  > selectedArea.y))
        {
            ScrollPosition.y -= selectedArea.height;
        }
        #endregion
    }

    private void DrawNode(T child)
    {
        bool previousState = child.IsFoldedOut;
        
        child.IsFoldedOut = OnNodeDraw(child, child == selectedNode);

        Rect lastArea = GUILayoutUtility.GetLastRect();     

        if (lastArea.Contains(Event.current.mousePosition))
        {
            HoverOver = child;
            HoverOverArea = lastArea;
        }

        if (CheckIsSelected(lastArea) || (child == selectedNode || child.IsFoldedOut != previousState))
        {
            if (!Event.current.control)
            {
                if (selectedNode != child)
                {
                    GUIUtility.keyboardControl = 0;
                    IsDirty = true;
                }
                selectedNode = child;
                AssignSelectedArea(lastArea);
            }
        }

        if (CheckIsSelected(lastArea))
        {
            draggingNode = child;
        }
        /*if (Event.current.control && CheckIsSelected(lastArea))
        {
            draggingNode = child;
            draggingArea = lastArea;
        }*/

        if(Event.current.type == EventType.repaint)
            maxY = lastArea.y + lastArea.height;
    }

    private void DrawBackground(Rect area)
    {
        GUI.DrawTexture(area, EditorResources.Background);
    }

    private bool CheckIsSelected(Rect area)
    {
        return (Event.current.type == EventType.MouseDown || Event.current.type == EventType.ContextClick) && Event.current.mousePosition.x > _area.x && Event.current.mousePosition.x < _area.x + area.width - 50 && area.Contains(Event.current.mousePosition) && Event.current.type != EventType.Repaint;
    }

    private void AssignSelectedArea(Rect area)
    {
        selectedArea = area;
        selectedArea.width += 15;
    }

    //FilterBy: true if node contains search
    private bool FilterNodes(T node, Func<T, bool> filter)
    {
        if (node == null)
            return false;
        node.IsFiltered = false;
        if (node.GetChildren.Count > 0)
        {
            bool allChildrenFilted = true;
            foreach (var child in node.GetChildren)
            {
                bool filtered = FilterNodes(child, filter);
                if (!filtered)
                {
                    allChildrenFilted = false;
                }
            }
            node.IsFiltered = allChildrenFilted; //If all children are filtered, this node also becomes filtered unless its name is not filtered
            if(node.IsFiltered)
                node.IsFiltered = filter(node);
            return node.IsFiltered;
        }
        else
        {
            node.IsFiltered = filter(node);
            return node.IsFiltered;
        }
    }

    public void Filter(Func<T, bool> filter)
    {
        filterFunc = filter;
        triggerFilter = true;
    }

    private bool HandleDragging()
    {
        Rect scrolledArea = _area; 
        scrolledArea.y += ScrollPosition.y;

        if (Event.current.ClickedWithin(scrolledArea, clickPos) && clickedWithin && Event.current.type != EventType.Used && Event.current.button == 0 && Event.current.mousePosition.y < maxY + 2)
        {
            dragStart = clickPos;
            wantToDrag = true;
        }

        if (wantToDrag && Event.current.type != EventType.Used && (Event.current.type != EventType.DragUpdated || Event.current.type != EventType.DragPerform) && Event.current.type == EventType.MouseDrag && Vector2.Distance(dragStart, Event.current.mousePosition) > 10.0f)
        {
            wantToDrag = false;
            if (draggingNode != null )
            {
                DragAndDrop.PrepareStartDrag();
                //GUIUtility.hotControl = GUIDCreator.Create();
                DragAndDrop.SetGenericData(draggingNode.GetName, draggingNode);
                DragAndDrop.paths = null;
                DragAndDrop.objectReferences = new UnityEngine.Object[] { draggingNode };
                DragAndDrop.StartDrag("InAudio Tree Drag N Drop");
                Event.current.Use();
            }
            
        }

        if (Event.current.type == EventType.MouseUp)
        {
            DragAndDrop.PrepareStartDrag();
        }

        bool canDropObjects = false;
        if (CanDropObjects != null)
            canDropObjects = CanDropObjects(HoverOver, DragAndDrop.objectReferences);
        if (canDropObjects)
        {
            DrawBackground(HoverOverArea);
            IsDirty = true;
        }
        return canDropObjects;
    }

    private void PostDrawDragHandle(bool canDropObject)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {
            if (canDropObject)
            {
                if (HoverOver != null)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        OnDrop(HoverOver, DragAndDrop.objectReferences);
                    }
                }
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
            }
        }
    }

    public void FocusOnSelectedNode()
    {
        focusOnSelectedNode = true;
    }
}
}