using System;
using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.ReorderableList;
using InAudioSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class AudioEventDrawer
{
    private static InAudioEventNode lastEvent;
    private static AudioEventAction audioEventAction = null;
    private static Vector2 scrollPos;
    //private static Rect drawArea;
    private static AudioEventAction toRemove = null;
    private static EventActionListAdapter ListAdapter ;

    private static GUIStyle leftStyle = new GUIStyle(GUI.skin.label);

    private static GameObject eventObjectTarget;


    public static bool Draw(InAudioEventNode audioevent)
    {
        if (ListAdapter == null)
        {
            ListAdapter = new EventActionListAdapter();
            ListAdapter.DrawEvent = DrawItem;
            ListAdapter.ClickedInArea = ClickedInArea;
        }

        if (lastEvent != audioevent)
        {
            ListAdapter.Event = audioevent;

            audioEventAction = null;
            if (audioevent.ActionList.Count > 0)
            {
                audioEventAction = audioevent.ActionList[0];
            }
        }
        EditorGUILayout.BeginVertical();

        lastEvent = audioevent;
        UndoHelper.GUIUndo(audioevent, "Name Change", ref audioevent.Name, () => 
            EditorGUILayout.TextField("Name", audioevent.Name));
        
        bool repaint = false;
      
        if (audioevent.Type == EventNodeType.Event)
        {
            EditorGUIHelper.DrawID(audioevent.GUID);

            if (Application.isPlaying)
            {
                eventObjectTarget = EditorGUILayout.ObjectField("Event preview target", eventObjectTarget, typeof(GameObject), true) as GameObject;
                if (eventObjectTarget != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Post event"))
                    {
                        InAudio.PostEvent(eventObjectTarget, audioevent);
                    }
                    if (GUILayout.Button("Stop All Sounds on Object"))
                    {
                        InAudio.StopAll(eventObjectTarget);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Separator();
            }

            

            UndoHelper.GUIUndo(audioevent, "Delay", ref audioevent.Delay, () =>
                Mathf.Max(EditorGUILayout.FloatField("Delay", audioevent.Delay), 0));
          
            NewEventArea(audioevent);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);

                repaint = DrawContent();

            EditorGUILayout.EndScrollView();
            DrawSelected(audioEventAction);
        }

        EditorGUILayout.EndVertical(); 

        if (toRemove != null)
        {
            UndoHelper.DoInGroup(() =>
            {
                //Remove the required piece
                int index = audioevent.ActionList.FindIndex(toRemove);
                AudioEventWorker.DeleteActionAtIndex(audioevent, index);
                
            });
            toRemove = null;

        }
        else //Remove all actions that does not excist. 
        {
            audioevent.ActionList.RemoveAll(p => p == null);
        }
        return repaint;
    }

    private static void ClickedInArea(int i)
    {
        audioEventAction = lastEvent.ActionList[i];
    }

    private static void DrawSelected(AudioEventAction eventAction)
    {
        if (eventAction != null)
        {
            Rect thisArea = EditorGUILayout.BeginVertical(GUILayout.Height(120));
            EditorGUILayout.LabelField("");
            var buttonArea = thisArea;
            buttonArea.height = 16;

            GUI.skin.label.alignment = TextAnchor.UpperLeft;

            UndoHelper.GUIUndo(eventAction, "Event Action Delay", ref eventAction.Delay, () =>
                Mathf.Max(EditorGUI.FloatField(buttonArea, "Seconds Delay", eventAction.Delay), 0));
            
            buttonArea.y += 33;

            var busAction = eventAction as InEventBusAction;
            var busMuteAction = eventAction as InEventBusMuteAction;
            var bankLoadingAction = eventAction as InEventBankLoadingAction;
            var audioAction = eventAction as InEventAudioAction;
            if (audioAction != null)
            {
                if (audioAction.EventActionType == EventActionTypes.Play || audioAction.EventActionType == EventActionTypes.Stop || audioAction.EventActionType == EventActionTypes.StopAll)
                {
                    UndoHelper.GUIUndo(audioAction, "Fade Time", ref audioAction.Fadetime,
                        () => Mathf.Max(0, EditorGUILayout.FloatField("Fade Time", audioAction.Fadetime)));
                    UndoHelper.GUIUndo(audioAction, "Fade Type", ref audioAction.TweenType,
                        () => (LeanTweenType) EditorGUILayout.EnumPopup("Fade Type", audioAction.TweenType));
                    if (audioAction.TweenType == LeanTweenType.animationCurve)
                    {
                        EditorGUILayout.HelpBox("Animation curve type is not supported", MessageType.Warning);
                    }
                }
            }
            else
                if (busAction != null)
            {
                UndoHelper.GUIUndo(busAction, "Bus Action Volume", ref busAction.Volume, () =>
                {
                    if (busAction.VolumeMode == InEventBusAction.VolumeSetMode.Relative)
                        return EditorGUI.Slider(buttonArea, "Relative Volume", busAction.Volume, -1.0f, 1.0f);
                    else
                        return EditorGUI.Slider(buttonArea, "Target Volume", busAction.Volume, 0.0f, 1.0f);    
                });

                buttonArea.y += 21;
                UndoHelper.GUIUndo(busAction, "Bus Action Volume Mode", ref busAction.VolumeMode, () =>
                    (InEventBusAction.VolumeSetMode)EditorGUI.EnumPopup(buttonArea, "Volume Mode", busAction.VolumeMode));

                buttonArea.y += 26;
                UndoHelper.GUIUndo(busAction, "Bus Action Fade Duration", ref busAction.Duration, () =>
                    Mathf.Max(EditorGUI.FloatField(buttonArea, "Fade Duration", busAction.Duration), 0));

                buttonArea.y += 21;
                UndoHelper.GUIUndo(busAction, "Bus Action Fade Curve", ref busAction.FadeCurve, () =>
                    (FadeCurveType)EditorGUI.EnumPopup(buttonArea, "Fade Curve", busAction.FadeCurve));
            }  else if (busMuteAction != null)
            {
                UndoHelper.GUIUndo(busMuteAction, "Mute", ref busMuteAction.Action, () =>
                    (InEventBusMuteAction.MuteAction)EditorGUI.EnumPopup(buttonArea, "Action", busMuteAction.Action));
            }
            else if (bankLoadingAction != null)
            {
                UndoHelper.GUIUndo(bankLoadingAction, "Bank Loading Action", ref bankLoadingAction.LoadingAction, () =>
                    (BankHookActionType)EditorGUI.EnumPopup(buttonArea, "Load Action", bankLoadingAction.LoadingAction));
            }
            EditorGUILayout.EndVertical();
        }
    }

    private static void NewEventArea(InAudioEventNode audioevent)
    {
        var defaultAlignment = GUI.skin.label.alignment;

        EditorGUILayout.BeginHorizontal();

        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndHorizontal();
        Rect lastArea = GUILayoutUtility.GetLastRect();
        lastArea.height *= 1.5f;

        if (GUI.Button(lastArea, "Click or drag here to add event action"))
        {
            ShowCreationContext(audioevent);
        }
        
        var dragging = DragAndDrop.objectReferences;
        OnDragging.OnDraggingObject(dragging, lastArea,
            objects => AudioEventWorker.CanDropObjects(audioevent, dragging),
            objects => AudioEventWorker.OnDrop(audioevent, dragging));
        
        GUI.skin.label.alignment = defaultAlignment;
    }

    private static bool DrawContent()
    {
        ReorderableListGUI.ListField(ListAdapter, ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons);
    
        return false;
    }

    private static void DrawItem(Rect position, int i)
    {
        var item = lastEvent.ActionList[i];
        leftStyle.alignment = TextAnchor.MiddleLeft;
        if(item == audioEventAction)
            DrawBackground(position);

        Rect typePos = position;
        typePos.width = 110;
        if (item != null)
        {
            
            if (GUI.Button(typePos, EventActionExtension.GetList().Find(p => p.Value == (int)item.EventActionType).Name))
            {
                ShowChangeContext(lastEvent, item);
                Event.current.Use();
            }
        }
        else
            GUI.Label(position, "Missing data", leftStyle);


        typePos.x += 130;
        if (item != null && item.EventActionType != EventActionTypes.StopAll)
        {
            Rect area = typePos;
            area.width = position.width - 220;
            GUI.Label(area, item.ObjectName);
        }
        HandleDragging(item, typePos);

        position.x = position.x + position.width - 75;
        position.width = 45;

        if (item != null && item.EventActionType != EventActionTypes.StopAll)
        {
            if (GUI.Button(position, "Find"))
            {
                SearchHelper.SearchFor(item);
            }
        }

        position.x += 50;
        position.width = 20;

        if (audioEventAction == item)
        {
            DrawBackground(position);
        }

        if (GUI.Button(position, "X"))
        {
            toRemove = item;
        }
    }

    private static void HandleDragging(AudioEventAction currentAction, Rect dragArea)
    {
        if (currentAction != null)
        {
            
            if (currentAction is InEventAudioAction)
            {
                InAudioNode dragged = OnDragging.DraggingObject<InAudioNode>(dragArea, node => node.IsPlayable);
                if (dragged != null)
                {
                    UndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
            else if (currentAction is InEventBankLoadingAction)
            {
                InAudioBankLink dragged = OnDragging.DraggingObject<InAudioBankLink>(dragArea,
                    bank => bank.Type == AudioBankTypes.Link);

                if (dragged != null)
                {
                    UndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
            else if (currentAction is InEventBusAction)
            {
                InAudioBus dragged = OnDragging.DraggingObject<InAudioBus>(dragArea, bus => true);
                
                if (dragged != null)
                {
                    UndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
            else if (currentAction is InEventBusMuteAction)
            {
                InAudioBus dragged = OnDragging.DraggingObject<InAudioBus>(dragArea, bus => true);
                if (dragged != null)
                {
                    UndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }

            else if (currentAction is InEventBusStopAction)
            {
                InAudioBus dragged = OnDragging.DraggingObject<InAudioBus>(dragArea, bus => true);
                if (dragged != null)
                {
                    UndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
        }
    }


    private static void DrawBackground(Rect area)
    {
        GUI.depth += 10;
        GUI.DrawTexture(area, EditorResources.Background);
        GUI.depth -= 10;
    }

    private static void ShowChangeContext(InAudioEventNode audioEvent, AudioEventAction action)
    {
        var menu = new GenericMenu();

        List<EventActionExtension.ActionMeta> actionList = EventActionExtension.GetList();
        foreach (EventActionExtension.ActionMeta currentType in actionList)
        {
            var enumType = currentType.ActionType;
            menu.AddItem(
                new GUIContent(currentType.Name),
                false,
                f => ChangeAction(audioEvent, action, enumType),
                currentType.ActionType
                );
        }
        
        menu.ShowAsContext();
    }

    private static void ChangeAction(InAudioEventNode audioEvent, AudioEventAction action, EventActionTypes newEnumType)
    {
        for (int i = 0; i < audioEvent.ActionList.Count; ++i)
        {
            if (audioEvent.ActionList[i] == action)
            {
                Type oldType = AudioEventWorker.ActionEnumToType(action.EventActionType);
                Type newType = AudioEventWorker.ActionEnumToType(newEnumType);


                if (oldType != newType)
                {
                    UndoHelper.DoInGroup(() => AudioEventWorker.ReplaceActionDestructiveAt(audioEvent, newEnumType, i));
                }
                else
                {
                    UndoHelper.RecordObject(action, "Change Event Action Type");
                    action.EventActionType = newEnumType;
                }    
                
                break;
            }
        }
    }

    private static void ShowCreationContext(InAudioEventNode audioevent)
    {
        var menu = new GenericMenu();

        List<EventActionExtension.ActionMeta> actionList = EventActionExtension.GetList();
        foreach (EventActionExtension.ActionMeta currentType in actionList)
        {
            Type newType = AudioEventWorker.ActionEnumToType(currentType.ActionType);
            var enumType = currentType.ActionType;
            menu.AddItem(new GUIContent(currentType.Name), false, f =>
                AudioEventWorker.AddEventAction(audioevent, newType, enumType), currentType);
        }
        menu.ShowAsContext();
    }
}
}