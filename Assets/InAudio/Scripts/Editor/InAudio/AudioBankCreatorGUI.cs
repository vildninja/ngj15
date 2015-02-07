using System;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
    public class AudioBankCreatorGUI : BaseCreatorGUI<InAudioBankLink>
    {
        public AudioBankCreatorGUI(AuxWindow window) : base(window)
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

            isDirty |= treeDrawer.DrawTree(InAudioInstanceFinder.DataManager.BankLinkTree, treeArea);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawRightSide(Rect area)
        {
            EditorGUILayout.BeginVertical();

            if (SelectedNode != null)
            {
                AudioBankLinkDrawer.Draw(SelectedNode);
            }

            EditorGUILayout.EndVertical();
        }

        protected override bool CanDropObjects(InAudioBankLink node, Object[] objects)
        {
            if (node == null || objects == null)
                return false;

            if (objects.Length > 0 && objects[0] as InAudioBankLink != null && node.Type != AudioBankTypes.Link)
            {
                return !NodeWorker.IsChildOf(objects[0] as InAudioBankLink, node);
            }
            return false;
        }

        protected override bool OnNodeDraw(InAudioBankLink node, bool isSelected)
        {
            return GenericTreeNodeDrawer.Draw(node, isSelected);
        }

        protected override void OnDrop(InAudioBankLink node, Object[] objects)
        {
            UndoHelper.DragNDropUndo(node, "Bank Drag N Drop");
            InAudioBankLink target = objects[0] as InAudioBankLink;
            NodeWorker.ReasignNodeParent(target, node);
        }

        protected override void OnContext(InAudioBankLink node)
        {
            if (node == null)
                return;
            var menu = new GenericMenu();

            if (node.Type == AudioBankTypes.Folder)
            {
                menu.AddItem(new GUIContent(@"Create Child/Folder"), false,
                    data => CreateBank(node, AudioBankTypes.Folder), node);
                menu.AddItem(new GUIContent(@"Create Child/Bank"), false, data => CreateBank(node, AudioBankTypes.Link),
                    node);
            }
            else if (node.Type == AudioBankTypes.Link)
            {
                menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Bank"));
            }

            menu.AddSeparator("");

            /*if (!toDelete.IsRoot)
        {
            menu.AddItem(new GUIContent(@"Delete"), false, data => DeleteNode(InAudioInstanceFinder.DataManager.BankLinkTree, data as AudioBankLink), toDelete);
        }
        else*/
            if (node.IsRoot)
                menu.AddDisabledItem(new GUIContent(@"Cannot delete root"));
            else
            {
                menu.AddItem(new GUIContent(@"Delete If Empty"), false,
                    data => DeleteNode(InAudioInstanceFinder.DataManager.BankLinkTree, data as InAudioBankLink), node);
            }
            menu.ShowAsContext();
        }

        private void DeleteNode(InAudioBankLink root, InAudioBankLink toDelete)
        {
            if (toDelete.GetChildren.Count > 0)
            {
                EditorUtility.DisplayDialog("Cannot delete bank", "Cannot delete folder with bank children", "ok");
                return;
            }


            Func<InAudioNode, bool> usedBankRoot = node =>
            {
                if (node.Type == AudioNodeType.Folder)
                {
                    var data = node.NodeData as InFolderData;
                    if (node.IsRoot && data.BankLink == toDelete)
                    {
                        return true;
                    }
                    else if (node.Type == AudioNodeType.Folder && data.BankLink == toDelete)
                    {
                        return true;
                    }
                }

                return false;
            };

            if (TreeWalker.Any(InAudioInstanceFinder.DataManager.AudioTree, usedBankRoot))
            {
                EditorUtility.DisplayDialog("Cannot delete bank", "Cannot delete bank that is in use", "ok");
                return;
            }

            int nonFolderCount = TreeWalker.Count(root, link => link.Type == AudioBankTypes.Link);
            if (nonFolderCount == 1 && toDelete.Type == AudioBankTypes.Link)
            {
                EditorUtility.DisplayDialog("Cannot delete the bank", "Cannot delete the last bank", "ok");
                return;
            }

            if (toDelete.Type == AudioBankTypes.Link &&
                !EditorUtility.DisplayDialog("Delete bank?", "This cannot be undone.", "Delete", "Do nothing"))
                return;

            if (toDelete.Type == AudioBankTypes.Link)
                AudioBankWorker.DeleteBank(toDelete);
            else if (toDelete.Type == AudioBankTypes.Folder)
                AudioBankWorker.DeleteFolder(toDelete);
        }

        private void CreateBank(InAudioBankLink parent, AudioBankTypes type)
        {
            //TODO make real undo
            UndoHelper.RecordObjectFull(parent, "Bank " + (type == AudioBankTypes.Folder ? "Folder " : "") + "Creation");
            if (type == AudioBankTypes.Folder)
                AudioBankWorker.CreateFolder(parent.gameObject, parent, GUIDCreator.Create());
            else
                AudioBankWorker.CreateBank(parent.gameObject, parent, GUIDCreator.Create());
        }

        protected override InAudioBankLink Root()
        {
            return InAudioInstanceFinder.DataManager.BankLinkTree;
        }

        protected override GUIPrefs GUIData
        {
            get { return InAudioInstanceFinder.InAudioGuiUserPrefs.BankGUIData; }
        }

    }
}