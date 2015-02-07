using System;
using System.Collections.Generic;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class AudioCreatorGUI : BaseCreatorGUI<InAudioNode>
    {
        public AudioCreatorGUI(InAudioWindow window)
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

            isDirty |= treeDrawer.DrawTree(window.Manager.AudioTree, treeArea);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void DrawRightSide(Rect area)
        {
            EditorGUILayout.BeginVertical();

            if (SelectedNode != null)
            {
                DrawTypeControls(SelectedNode);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTypeControls(InAudioNode node)
        {
            try
            {
                var type = node.Type;
                if (node.NodeData != null)
                {
                    switch (type)
                    {
                        case AudioNodeType.Audio:
                            AudioDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Sequence:
                            SequenceDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Random:
                            RandomDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Multi:
                            MultiDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Folder:
                            FolderDrawer.Draw(node);
                            break;
                        case AudioNodeType.Track:
                            TrackDataDrawer.Draw(node);
                            break;
                        case AudioNodeType.Root:
                            FolderDrawer.Draw(node);
                            break;
                    }
                }
                else if (SelectedNode.Type == AudioNodeType.Folder || SelectedNode.Type == AudioNodeType.Root)
                {
                    FolderDrawer.Draw(node);
                }
                else
                {
                    EditorGUILayout.HelpBox("Corrupt data. The data for this node does either not exist or is corrupt.",
                        MessageType.Error, true);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create new element", GUILayout.Width(150)))
                    {
                        AudioNodeWorker.AddDataClass(node);
                    }


                    EditorGUILayout.EndHorizontal();
                }
            }
            catch (ExitGUIException e)
            {
                throw e;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            /*catch (Exception e)
            {
                //While this catch was made to catch persistent errors,  like a missing null check, it can also catch other errors
                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox("An exception is getting caught while trying to draw node. ", MessageType.Error,
                    true);
                if (GUILayout.Button("Create new element", GUILayout.Width(150)))
                {
                    AudioNodeWorker.AddDataClass(node);
                }
                EditorGUILayout.TextArea(e.ToString());
                EditorGUILayout.EndVertical();
            }*/
        }

        protected override bool CanDropObjects(InAudioNode node, UnityEngine.Object[] objects)
        {
            if (node == null || objects == null)
                return false;

            int clipCount = DragAndDrop.objectReferences.Count(p => p is AudioClip);
            int nodeCount = DragAndDrop.objectReferences.Count(p => p is InAudioNode);

            if (DragAndDrop.objectReferences.Length == 0)
                return false;

            if (clipCount == objects.Length) //Handle clip count
            {
                //clipCount == 1 means it overrides the current clip
                if (node.Type == AudioNodeType.Audio && clipCount != 1)
                    return false;
                return true;
            }
            else if (nodeCount == objects.Length) //Handle audio node drag n drop
            {
                if (node.Type == AudioNodeType.Audio) //Can't drop on an audionode as it can't have children
                    return false;

                var draggingNode = objects[0] as InAudioNode;

                if (!node.IsRootOrFolder && draggingNode.IsRootOrFolder)
                    return false;

                if (node == draggingNode)
                    return false;

                return !NodeWorker.IsChildOf(objects[0] as InAudioNode, node);
            }
            return false;
        }

        protected override void OnDrop(InAudioNode node, UnityEngine.Object[] objects)
        {
            if (objects[0] as InAudioNode != null) //Drag N Drop internally in the tree, change the parent
            {
                node.IsFoldedOut = true;
                var nodeToMove = objects[0] as InAudioNode;

                UndoHelper.RecordObject(
                    new UnityEngine.Object[] { node, node.NodeData, nodeToMove.Parent.NodeData, nodeToMove, nodeToMove.Parent }.AddObj(
                        TreeWalker.FindAll(InAudioInstanceFinder.DataManager.BankLinkTree,
                            link => link.Type == AudioBankTypes.Link ? link.LazyBankFetch : null).ToArray()),
                    "Audio Node Move");

                NodeWorker.ReasignNodeParent(nodeToMove, node);
                AudioBankWorker.RebuildBanks(InAudioInstanceFinder.DataManager.BankLinkTree,
                    InAudioInstanceFinder.DataManager.AudioTree);
            }
            else if (node.Type != AudioNodeType.Audio) //Create new audio nodes when we drop clips
            {
                UndoHelper.RecordObject(UndoHelper.NodeUndo(node), "Adding Nodes to " + node.Name);

                AudioClip[] clips = objects.Convert(o => o as AudioClip);

                Array.Sort(clips, (clip, audioClip) => StringLogicalComparer.Compare(clip.name, audioClip.name));

                for (int i = 0; i < clips.Length; ++i)
                {
                    var clip = clips[i];
                    var child = AudioNodeWorker.CreateChild(node, AudioNodeType.Audio);
                    var path = AssetDatabase.GetAssetPath(clip);
                    try
                    {
                        //Try and get the name of the clip. Gets the name and removes the end. Assets/IntroSound.mp3 -> IntroSound
                        int lastIndex = path.LastIndexOf('/') + 1;
                        child.Name = path.Substring(lastIndex, path.LastIndexOf('.') - lastIndex);
                    }
                    catch (Exception)
                    //If it happens to be a mutant path. Not even sure if this is possible, but better safe than sorry
                    {
                        child.Name = node.Name + " Child";
                    }

                    (child.NodeData as InAudioData).EditorClip = clip;
                    (child.NodeData as InAudioData).RuntimeClip = clip;

                    AudioBankWorker.AddNodeToBank(child, clip);
                    Event.current.Use();
                }
            }
            else //Then it must be an audio clip dropped on an audio node, so assign the clip to that node
            {
                var nodeData = (node.NodeData as InAudioData);
                if (nodeData != null)
                {
                    UndoHelper.RecordObject(UndoHelper.NodeUndo(node), "Change Audio Clip In " + node.Name);
                    nodeData.EditorClip = objects[0] as AudioClip;
                    if (Application.isPlaying)
                    {
                        if (node.GetBank().IsLoaded)
                            nodeData.RuntimeClip = objects[0] as AudioClip;
                    }
                    AudioBankWorker.SwapClipInBank(node, objects[0] as AudioClip);
                }

            }
        }

        protected override void OnContext(InAudioNode node)
        {
            var menu = new GenericMenu();

            #region Duplicate

#if !UNITY_4_1 && !UNITY_4_2
            if (!node.IsRoot)
                menu.AddItem(new GUIContent("Duplicate"), false, data => AudioNodeWorker.Duplicate(node), node);
            else
                menu.AddDisabledItem(new GUIContent("Duplicate"));
            menu.AddSeparator("");
#endif

            #endregion
            #region Create child

            if (node.Type == AudioNodeType.Audio || node.Type == AudioNodeType.Voice)
            //If it is a an audio source, it cannot have any children
            {
                menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Audio"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Random"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Sequence"));
                menu.AddDisabledItem(new GUIContent(@"Create Child/Multi"));
#if UNITY_TRACK
                //menu.AddDisabledItem(new GUIContent(@"Create Child/Track"));
#endif
                //menu.AddDisabledItem(new GUIContent(@"Create Child/Track"));
                //menu.AddDisabledItem(new GUIContent(@"Create Child/Voice"));
            }
            else
            {
                if (node.Type == AudioNodeType.Root || node.Type == AudioNodeType.Folder)
                    menu.AddItem(new GUIContent(@"Create Child/Folder"), false,
                        (obj) => CreateChild(node, AudioNodeType.Folder), node);
                else
                    menu.AddDisabledItem(new GUIContent(@"Create Child/Folder"));
                menu.AddItem(new GUIContent(@"Create Child/Audio"), false,
                    (obj) => CreateChild(node, AudioNodeType.Audio), node);
                menu.AddItem(new GUIContent(@"Create Child/Random"), false,
                    (obj) => CreateChild(node, AudioNodeType.Random), node);
                menu.AddItem(new GUIContent(@"Create Child/Sequence"), false,
                    (obj) => CreateChild(node, AudioNodeType.Sequence), node);
                menu.AddItem(new GUIContent(@"Create Child/Multi"), false,
                    (obj) => CreateChild(node, AudioNodeType.Multi), node);
#if UNITY_TRACK
                menu.AddItem(new GUIContent(@"Create Child/Track"), false,
                    (obj) => CreateChild(node, AudioNodeType.Track), node);
#endif
                //menu.AddItem(new GUIContent(@"Create Child/Track"), false,      (obj) => CreateChild(node, AudioNodeType.Track), node);
                //menu.AddItem(new GUIContent(@"Create Child/Voice"), false,      (obj) => CreateChild(node, AudioNodeType.Voice), node);
            }

            #endregion

            menu.AddSeparator("");

            #region Add new parent

            if (node.Parent != null &&
                (node.Parent.Type == AudioNodeType.Folder || node.Parent.Type == AudioNodeType.Root))
                menu.AddItem(new GUIContent(@"Add Parent/Folder"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Folder), node);
            else
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Folder"));
            menu.AddDisabledItem(new GUIContent(@"Add Parent/Audio"));
            if (node.Parent != null && node.Type != AudioNodeType.Folder)
            {
                menu.AddItem(new GUIContent(@"Add Parent/Random"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Random), node);
                menu.AddItem(new GUIContent(@"Add Parent/Sequence"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Sequence), node);
                menu.AddItem(new GUIContent(@"Add Parent/Multi"), false,
                    (obj) => AudioNodeWorker.AddNewParent(node, AudioNodeType.Multi), node);
                //menu.AddItem(new GUIContent(@"Add Parent/Track"), false, (obj) =>       AudioNodeWorker.AddNewParent(node, AudioNodeType.Track), node);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Random"));
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Sequence"));
                menu.AddDisabledItem(new GUIContent(@"Add Parent/Multi"));
                //menu.AddDisabledItem(new GUIContent(@"Add Parent/Track"));
            }
            //menu.AddDisabledItem(new GUIContent(@"Add Parent/Voice"));

            #endregion

            menu.AddSeparator("");

            #region Convert to

            if (node.Children.Count == 0 && !node.IsRootOrFolder)
                menu.AddItem(new GUIContent(@"Convert To/Audio"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Audio), node);
            else
                menu.AddDisabledItem(new GUIContent(@"Convert To/Audio"));
            if (!node.IsRootOrFolder)
            {
                menu.AddItem(new GUIContent(@"Convert To/Random"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Random), node);
                menu.AddItem(new GUIContent(@"Convert To/Sequence"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Sequence), node);
                menu.AddItem(new GUIContent(@"Convert To/Multi"), false,
                    (obj) => AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Multi), node);
                //menu.AddItem(new GUIContent(@"Convert To/Track"), false, (obj) =>       AudioNodeWorker.ConvertNodeType(node, AudioNodeType.Track), node);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(@"Convert To/Random"));
                menu.AddDisabledItem(new GUIContent(@"Convert To/Sequence"));
                menu.AddDisabledItem(new GUIContent(@"Convert To/Multi"));
                //menu.AddDisabledItem(new GUIContent(@"Add Parent/Track"));
            }

            #endregion

            menu.AddSeparator("");

            #region Delete

            if (node.Type != AudioNodeType.Root)
                menu.AddItem(new GUIContent("Delete"), false, obj =>
                {
                    treeDrawer.SelectedNode = TreeWalker.GetPreviousVisibleNode(treeDrawer.SelectedNode);
                    AudioNodeWorker.DeleteNode(node);
                }, node);
            else
                menu.AddDisabledItem(new GUIContent("Delete"));

            #endregion

            menu.ShowAsContext();
        }

        private void CreateChild(InAudioNode parent, AudioNodeType type)
        {
            UndoHelper.RecordObjectFull(new UnityEngine.Object[] { parent, parent.GetBank().LazyBankFetch },
                "Create Audio Node");
            var newNode = AudioNodeWorker.CreateChild(parent, type);

            if (type == AudioNodeType.Audio)
            {
                AudioBankWorker.AddNodeToBank(newNode, null);
            }
        }

        protected override bool OnNodeDraw(InAudioNode node, bool isSelected)
        {
            return GenericTreeNodeDrawer.Draw(node, isSelected);
        }

        internal void FindAudio(Func<InAudioNode, bool> filter)
        {
            searchingFor = "Finding nodes in bank";
            lowercaseSearchingFor = "Finding nodes in bank";
            treeDrawer.Filter(filter);
        }

        protected override InAudioNode Root()
        {
            return InAudioInstanceFinder.DataManager.AudioTree;
        }

        protected override GUIPrefs GUIData
        {
            get { return InAudioInstanceFinder.InAudioGuiUserPrefs.AudioGUIData; }
        }
    }
}