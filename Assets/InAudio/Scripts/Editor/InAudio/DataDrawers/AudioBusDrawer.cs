using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class AudioBusDrawer
    {
        private static int beDuckedByID;

        public static void Draw(InAudioBus bus)
        {
            EditorGUILayout.BeginVertical();

            UndoHelper.GUIUndo(bus, "Name Change", ref bus.Name, () => 
                EditorGUILayout.TextField("Name", bus.Name));

            EditorGUIHelper.DrawID(bus.GUID);

            EditorGUILayout.Separator();

            if (!Application.isPlaying)
            {
                UndoHelper.GUIUndo(bus, "Mute Bus", ref bus.Mute, () =>
                    EditorGUILayout.Toggle("Initial Mute", bus.Mute));
            }
            else
            {
                UndoHelper.GUIUndo(bus, "Mute Bus (Runtime)", ref bus.RuntimeMute, () =>
                    EditorGUILayout.Toggle("Mute (Runtime)", bus.RuntimeMute));
            }

            EditorGUILayout.Separator();

            UndoHelper.GUIUndo(bus, "Volume Change", ref bus.Volume, () =>
                EditorGUILayout.Slider("Master Volume", bus.Volume, 0.0f, 1.0f));

            if (!Application.isPlaying)
                UndoHelper.GUIUndo(bus, "Runtime Volume Change", ref bus.SelfVolume, () =>
                    EditorGUILayout.Slider("Initial Runtime Volume", bus.SelfVolume, 0.0f, 1.0f));
            else
            {
                UndoHelper.GUIUndo(bus, "Runtime Volume Change", ref bus.RuntimeSelfVolume, () =>
                    EditorGUILayout.Slider("Runtime Volume", bus.RuntimeSelfVolume, 0.0f, 1.0f));
            }

            EditorGUILayout.Separator(); 

            GUI.enabled = false; 
            if (Application.isPlaying)
            {
                EditorGUILayout.Slider("Ducking", bus.LastDuckedVolume, -1.0f, 0.0f);
                EditorGUILayout.Separator();
                
                EditorGUILayout.Slider("Final Hierarchy Volume", Mathf.Clamp(bus.FinalVolume, 0 , 1), 0, 1.0f);

            }
            else
            {
                float volume = bus.CombinedVolume;
                if (bus.RuntimeMute)
                    volume = 0;
                EditorGUILayout.Slider("Final Hierarchy Volume", volume, 0, 1.0f);
            }

            GUI.enabled = true;

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            
            //Not implemented yet
            #region Ducking

            

            EditorGUILayout.Separator(); EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Hold Control down and drag a bus here.");
            EditorGUILayout.LabelField("Cannot add parents or children.");

            GUILayout.Button("Drag bus here", GUILayout.Width(200));

            var dragging = DragAndDrop.objectReferences;
            OnDragging.OnDraggingObject<InAudioBus>(dragging, GUILayoutUtility.GetLastRect(),
                buses => AudioBusWorker.CanBeDuckedBy(bus, buses[0]),
                buses =>
                {
                    UndoHelper.RecordObject(bus, "Add");
                    for (int i = 0; i < buses.Length; i++)
                    {
                        DuckingData data = new DuckingData();
                        data.DuckedBy = buses[i];
                        bus.DuckedBy.Add(data);
                    }

                }
                );

            EditorGUILayout.LabelField("");
            Rect labelArea = GUILayoutUtility.GetLastRect();

            labelArea.width = labelArea.width/4.0f - 5;
            Rect workArea = labelArea;
            
            for (int i = 0; i < bus.DuckedBy.Count; i++)
            {
                workArea.width = labelArea.width;
                workArea.y += workArea.height + 4;
                workArea.x = labelArea.x;
                
                DuckingData data = bus.DuckedBy[i];
                
                EditorGUI.SelectableLabel(workArea, data.DuckedBy.Name);
                Rect area1 = workArea;
                workArea.x += workArea.width;

                UndoHelper.GUIUndo(bus, "Duck Amount Change", () =>
                    EditorGUI.Slider(workArea, data.VolumeDucking, -1.0f, 0.0f),
                    v => data.VolumeDucking = v);
                Rect area2 = workArea;
                workArea.x += workArea.width;

                UndoHelper.GUIUndo(bus, "Attack Time", () =>
                    EditorGUI.Slider(workArea, data.AttackTime, 0.0f, 10.0f),
                    v => data.AttackTime = v);
                Rect area3 = workArea;
                workArea.x += workArea.width;

                UndoHelper.GUIUndo(bus, "Release Time", () =>
                    EditorGUI.Slider(workArea, data.ReleaseTime, 0.0f, 10.0f),
                    v => data.ReleaseTime = v);
                Rect area4 = workArea;
                workArea.x += workArea.width;
                workArea.width = 20;
                if (GUI.Button(workArea, "X"))
                {
                    UndoHelper.RecordObjectFull(bus, "X");
                    bus.DuckedBy.RemoveAt(i);
                    i--;
                }

                if (i == 0) //Workaround to avoid a gui layout mismatch
                {
                    area1.y -= 20;
                    area2.y -= 20;
                    area3.y -= 20;
                    area4.y -= 20;
                    EditorGUI.LabelField(area1, "Ducked By");
                    EditorGUI.LabelField(area2,"Volume Duck Amount");
                    EditorGUI.LabelField(area3, "Attack Time");
                    EditorGUI.LabelField(area4, "Release Time");
                }
            }
            
            #endregion


            /*   GUILayout.Label("Nodes Playing In This Specific Bus");

                lastArea.x += 20;
                lastArea.y = lastArea.y + lastArea.height + 2;
                lastArea.height = 17;
                List<RuntimePlayer> players = node.GetRuntimePlayers();
                for (int i = 0; i < players.Count; i++)
                {
                    GUI.Label(lastArea, players[i].NodePlaying.Name);

                    lastArea.y += 20;
                }
            */
            EditorGUILayout.EndVertical();
        }
    }

}