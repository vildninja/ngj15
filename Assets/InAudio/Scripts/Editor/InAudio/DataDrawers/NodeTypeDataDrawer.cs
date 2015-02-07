using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class NodeTypeDataDrawer
{
    public static Vector2 scrollView;
    public static InPlayer preview;

    //private static GameObject go;
    public static void Draw(InAudioNode node)
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play Node"))
            {
                var link = node.GetBank();
                if (link != null && link.IsLoaded)
                {
                    if (preview != null)
                    {
                        preview.Stop();
                    }
                    preview = InAudio.Play(InAudioInstanceFinder.Instance.gameObject, node);
                    preview.Pan = 0.0f;
                    preview.OnCompleted = (go, audioNode) => preview = null;
                }
                else
                {
                    Debug.Log("InAudio: Cannot preview node as its beloning bank is not loaded");
                }
            }
            if (GUILayout.Button("Stop Playing Node") && preview != null)
            {
                preview.Stop();
            }
            EditorGUILayout.EndHorizontal();
        }

        InAudioNodeData baseData = (InAudioNodeData)node.NodeData;
        EditorGUILayout.Separator();
        
        DrawSelectedArea(node, baseData);

        Seperators(2);

        if (baseData.SelectedArea == 0)
        {
            EditorGUIHelper.DrawID(node.GUID);

            #region Volume

            UndoHelper.GUIUndo(baseData, "Random Volume", ref baseData.RandomVolume, () =>
                EditorGUILayout.Toggle("Random Volume", baseData.RandomVolume));

            if (!baseData.RandomVolume)
            {
                UndoHelper.GUIUndo(baseData, "Volume", () =>
                    EditorGUILayout.Slider("Volume", baseData.MinVolume, 0, 1),
                    v =>
                    {
                        baseData.MinVolume = v;
                        if (baseData.MinVolume > baseData.MaxVolume)
                            baseData.MaxVolume = baseData.MinVolume + 0.1f;
                    });
            }
            else
            {
                UndoHelper.GUIUndo(baseData, "Random Volume", ref baseData.MinVolume, ref baseData.MaxVolume, (out float minVolume, out float maxVolume) =>
                {
                    EditorGUILayout.MinMaxSlider(new GUIContent("Volume"), ref baseData.MinVolume, ref baseData.MaxVolume, 0, 1);
                    minVolume = Mathf.Clamp(EditorGUILayout.FloatField("Min volume", baseData.MinVolume), 0, baseData.MaxVolume);
                    maxVolume = Mathf.Clamp(EditorGUILayout.FloatField("Max volume", baseData.MaxVolume), baseData.MinVolume, 1);
                });
            }
            #endregion

            Seperators(2);

            #region Parent pitch

            float minPitch = 0.001f;
            float maxPitch = 3;
            UndoHelper.GUIUndo(baseData, "Random Pitch", ref baseData.RandomPitch, () =>
                EditorGUILayout.Toggle("Random Pitch", baseData.RandomPitch));

            if (!baseData.RandomPitch)
            {
                UndoHelper.GUIUndo(baseData, "Pitch", () =>
                    EditorGUILayout.Slider("Pitch", baseData.MinPitch, minPitch, maxPitch),
                    v => {
                        baseData.MinPitch = v;
                        if (baseData.MinPitch > baseData.MaxPitch)
                            baseData.MaxPitch = baseData.MinPitch + 0.1f;
                        baseData.MaxPitch = Mathf.Clamp(baseData.MaxPitch, minPitch, 3.0f);
                    });
            }
            else
            {
                UndoHelper.GUIUndo(baseData, "Random Pitch", 
                    ref baseData.MinPitch, ref baseData.MaxPitch, 
                    (out float v1, out float v2) => {
                        EditorGUILayout.MinMaxSlider(new GUIContent("Pitch"), ref baseData.MinPitch, ref baseData.MaxPitch, minPitch, maxPitch);
                        v1 = Mathf.Clamp(EditorGUILayout.FloatField("Min pitch", baseData.MinPitch), minPitch, baseData.MaxPitch);
                        v2 = Mathf.Clamp(EditorGUILayout.FloatField("Max pitch", baseData.MaxPitch), baseData.MinPitch, maxPitch);
                });
            }

            #endregion

            Seperators(2);

            #region Delay

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            UndoHelper.GUIUndo(baseData, "Randomize Delay", ref baseData.RandomizeDelay, () => 
                EditorGUILayout.Toggle("Randomize Delay", baseData.RandomizeDelay));
            if (baseData.RandomizeDelay)
            {
                UndoHelper.GUIUndo(baseData, "Delay Change", ref baseData.InitialDelayMin, ref baseData.InitialDelayMax,
                    (out float v1, out float v2) =>
                    {
                        v1 = Mathf.Clamp(EditorGUILayout.FloatField("Min delay", baseData.InitialDelayMin), 0, baseData.InitialDelayMax);
                        v2 = Mathf.Clamp(EditorGUILayout.FloatField("Max delay", baseData.InitialDelayMax), baseData.InitialDelayMin, float.MaxValue);        
                    });
                
            }
            else
            {
                UndoHelper.GUIUndo(baseData, "Delay", ref baseData.InitialDelayMin, () =>
                {
                    float delay = Mathf.Max(EditorGUILayout.FloatField("Initial delay", baseData.InitialDelayMin), 0);
                    if (delay > baseData.InitialDelayMax)
                        baseData.InitialDelayMax = baseData.InitialDelayMin + 0.001f;
                    return delay;
                });
                
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            #endregion

            Seperators(2);

            #region Audio bus

            DataDrawerHelper.DrawBus(node);

            #endregion

            Seperators(2);

            #region Loops

            GUILayout.BeginVertical();

            UndoHelper.GUIUndo(baseData, "Use looping", ref baseData.Loop, () => EditorGUILayout.Toggle("Loop", baseData.Loop));
            if (baseData.Loop)
            {
                GUI.enabled = baseData.Loop;

                UndoHelper.GUIUndo(baseData, "Loop Infinite", ref baseData.LoopInfinite, 
                    () => EditorGUILayout.Toggle("Loop Infinite", baseData.LoopInfinite));
                if (baseData.Loop)
                    GUI.enabled = !baseData.LoopInfinite;

                UndoHelper.GUIUndo(baseData, "Loop Randomize", ref baseData.RandomizeLoops,
                    () => EditorGUILayout.Toggle("Randomize Loop Count", baseData.RandomizeLoops));

                if (!baseData.RandomizeLoops)
                {
                    UndoHelper.GUIUndo(baseData, "Loop Count", 
                        ref baseData.MinIterations, () => (byte) Mathf.Clamp(EditorGUILayout.IntField("Loop Count", baseData.MinIterations), 0, 255));
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    UndoHelper.GUIUndo(baseData, "Loop Count", ref baseData.MinIterations, ref baseData.MaxIterations,
                        (out byte v1, out byte v2) =>
                        {
                            v1 = (byte)Mathf.Clamp(EditorGUILayout.IntField("Min Loop Count", baseData.MinIterations), 0, 255);
                            v2 = (byte)Mathf.Clamp(EditorGUILayout.IntField("Max Loop Count", baseData.MaxIterations), 0, 255);

                            //Clamp to 0-255 and so that min/max doesn't overlap
                            v2 = (byte)Mathf.Clamp(v2, v1, 255);
                            v1 = (byte)Mathf.Clamp(v1, 0, v2);        
                        });

                    GUILayout.EndHorizontal();
                }

            }

            GUI.enabled = true;

            GUILayout.EndVertical();

            #endregion

            Seperators(2);

            #region Instance limiting
            UndoHelper.GUIUndo(baseData, "Limit Instances (Global)", ref baseData.LimitInstances, () => EditorGUILayout.Toggle("Limit Instances", baseData.LimitInstances));
            GUI.enabled = baseData.LimitInstances;
            if (baseData.LimitInstances)
            {
                UndoHelper.GUIUndo(baseData, "Max Instance Cont", ref baseData.MaxInstances, () => Math.Max(EditorGUILayout.IntField("Max Instance Count", baseData.MaxInstances), 0));
                UndoHelper.GUIUndo(baseData, "Stealing Type", ref baseData.InstanceStealingTypes, () => (InstanceStealingTypes)EditorGUILayout.EnumPopup("Stealing Type", baseData.InstanceStealingTypes));            
            }
            GUI.enabled = true;

            #endregion

            Seperators(2);

            #region Priority
            UndoHelper.GUIUndo(baseData, "Priority", ref baseData.Priority, () => EditorGUILayout.IntSlider("Priority", baseData.Priority, 0, 255));
            #endregion

            Seperators(2);

            #region Sample offset
            UndoHelper.GUIUndo(baseData, "Random Sample Offset", ref baseData.RandomSecondsOffset, () =>
               EditorGUILayout.Toggle("Random Sample Offset", baseData.RandomSecondsOffset));

            if (baseData.RandomSecondsOffset)
            {
                UndoHelper.GUIUndo(baseData, "First item offset", ref baseData.MinSecondsOffset, ref baseData.MaxSecondsOffset,
                    (out float v1, out float v2) =>
                    {
                        v1 = Mathf.Clamp(EditorGUILayout.FloatField("Min offset", baseData.MinSecondsOffset), 0, baseData.MaxSecondsOffset);
                        v2 = Mathf.Clamp(EditorGUILayout.FloatField("Max offset", baseData.MaxSecondsOffset), baseData.MinSecondsOffset, float.MaxValue);
                    });

            }
            else
            {
                UndoHelper.GUIUndo(baseData, "Delay", ref baseData.MinSecondsOffset, () =>
                {
                    var delay = Mathf.Max(EditorGUILayout.FloatField("First clip offset", baseData.MinSecondsOffset), 0);
                    if (delay > baseData.MaxSecondsOffset)
                        baseData.MaxSecondsOffset = baseData.MinSecondsOffset + 1;
                    return delay;
                });

            }

            if (node.Type == AudioNodeType.Audio)
            {
                var nodeData = node.NodeData as InAudioData;
                if (nodeData.EditorClip != null)
                {
                    float length = nodeData.EditorClip.ExactLength();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Clip length");
                    EditorGUILayout.SelectableLabel(length.ToString(), GUILayout.Height(20));
                    EditorGUILayout.EndHorizontal();
                    if (baseData.RandomSecondsOffset && (baseData.MinSecondsOffset > length || baseData.MaxSecondsOffset > length))
                    {
                        EditorGUILayout.HelpBox("Offset exceeds sound clip length", MessageType.Warning);
                    }
                    else if (baseData.MinSecondsOffset > length)
                    {
                        EditorGUILayout.HelpBox("Offset exceeds sound clip length", MessageType.Warning);
                    }
                }
            }
            #endregion
        }
        else
        {
            #region Attenuation

            if (!node.Parent.IsRootOrFolder)
                UndoHelper.GUIUndo(baseData, "Override Parent", ref baseData.OverrideAttenuation, () => GUILayout.Toggle(baseData.OverrideAttenuation, "Override Parent"));
            GUI.enabled = baseData.OverrideAttenuation;
            if (node.Parent.IsRootOrFolder)
                GUI.enabled = true;

            bool dirty = false;
            UndoHelper.GUIUndo(node, "Rolloff Mode", ref baseData.RolloffMode, () =>
            {
                dirty = true;
                return (AudioRolloffMode) EditorGUILayout.EnumPopup("Volume Rolloff", baseData.RolloffMode);

            });

            UndoHelper.GUIUndo(baseData, "Set Rolloff Distance", ref baseData.MinDistance, ref baseData.MaxDistance,
                (out float v1, out float v2) =>
                {
                    if (baseData.RolloffMode != AudioRolloffMode.Custom)
                    {
                        v1 = EditorGUILayout.FloatField("Min Distance", baseData.MinDistance);
                    }
                    else
                        v1 = baseData.MinDistance;
                    v2 = EditorGUILayout.FloatField("Max Distance", baseData.MaxDistance);
                    v1 = Mathf.Max(v1, 0.00001f);
                    v2 = Mathf.Max(v2, v1 + 0.01f);

                    dirty = true;
                });

            

            if (baseData.RolloffMode == AudioRolloffMode.Custom)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Rolloff");

                UndoHelper.GUIUndo(baseData, "Set Rolloff Curve", ref baseData.FalloffCurve, () => EditorGUILayout.CurveField(baseData.FalloffCurve, GUILayout.Height(200), GUILayout.Width(200)));
                //baseData.FalloffCurve = EditorGUILayout.CurveField(baseData.FalloffCurve, GUILayout.Height(200), GUILayout.Width(200));
                EditorGUILayout.EndHorizontal();

                var keys = baseData.FalloffCurve.keys;
                int keyCount = keys.Length;
                for (int i = 0; i < keyCount; i++)
                {
                    Keyframe key = keys[i];

                    key.time = Mathf.Clamp(key.time, 0.0f, 1.0f);
                    key.value = Mathf.Clamp(key.value, 0.0f, 1.0f);
                    try
                    {
                        baseData.FalloffCurve.MoveKey(i, key);
                    }
                    catch (Exception)
                    {
                    }
                    
                }
                if (GUILayout.Button("Reset curve", GUILayout.Width(150)))
                {
                    UndoHelper.RecordObject(baseData, "Reset Curve");
                    baseData.FalloffCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.1f, 1), new Keyframe(1, 0));
                }
                
                if (Selection.activeObject == null)
                {
                    EditorGUILayout.HelpBox("Please select any game object in the scene.\nIt fixes a bug in Unity Editor API",
                    MessageType.Info, true);
                }

                EditorGUILayout.HelpBox(
                    "Unity does not support setting custom rolloff via scripts. This will perform slower than a log/linear rolloff curve",
                    MessageType.Warning, true);
                GUI.enabled = false;
            }


            if (dirty && Application.isPlaying)
            {
                UpdateDateRuntime.Update(node.GetBus(), node);
            }

            #endregion 

            GUI.enabled = true;
        }
    }

    private static void Seperators(int layoutNumbers)
    {
        for (int i = 0; i < layoutNumbers; i++)
        {
            EditorGUILayout.Separator();
        }
    }

    public static void DrawSelectedArea(InAudioNode node, InAudioNodeData baseData)
    {
        UndoHelper.GUIUndo(node, "Name Change", ref baseData.SelectedArea, () =>
            GUILayout.Toolbar(baseData.SelectedArea, new[] { "Audio", "Attenuation" }));     
    }

    public static void DrawName(InAudioNode node)
    {
        UndoHelper.GUIUndo(node, "Name Change", ref node.Name, () =>
            EditorGUILayout.TextField("Name", node.Name));
    }
}
}