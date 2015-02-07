using System;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
//[CanEditMultipleObjects] 
[CustomPropertyDrawer(typeof(InAudioEvent))]
public class InAudioEventDrawer : PropertyDrawer  
{
    private const float LineHeight = 22;
    private const float DragHeight = 20;
    private GUIStyle eventTypeStyle;

    private InEventName fieldName;

    private InEventName FieldName
    {
        get
        {
            if (fieldName == null)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof (InEventName), false);
                fieldName = attributes.FirstOrDefault() as InEventName;
            }
            return fieldName;
        }
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        SerializedProperty array = prop.FindPropertyRelative("Events");
        if (array == null)
            return 100;
        float extraHeight = array.arraySize * LineHeight + 34;
        if (!prop.isExpanded)
            extraHeight = 0;
        if (prop.hasMultipleDifferentValues)
            extraHeight = 0;
        return base.GetPropertyHeight(prop, label) + extraHeight;
    }

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        var labelPos = pos;
        labelPos.y += 2;
        Color backgroundColor = GUI.color;

        TextAnchor defaultAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        var labelStyle = GUI.skin.GetStyle("label");

        if (eventTypeStyle == null)
            eventTypeStyle = new GUIStyle(GUI.skin.GetStyle("label"));

        SerializedProperty array = prop.FindPropertyRelative("Events");

        labelPos.height = 18;
        eventTypeStyle.fontStyle = FontStyle.Bold;

        EditorStyles.foldout.fontStyle = FontStyle.Bold;
        Rect nameRect = labelPos;

        if(FieldName == null)
            prop.isExpanded = EditorGUI.Foldout(nameRect, prop.isExpanded, "Event " + ObjectNames.NicifyVariableName( fieldInfo.Name ), true);
        else
            prop.isExpanded = EditorGUI.Foldout(nameRect, prop.isExpanded, "Event " + FieldName.EventType, true);
        
        EditorStyles.foldout.fontStyle = FontStyle.Normal;

        if (prop.isExpanded)
        {
            if (prop.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox("Cannot edit with multiple different values.", MessageType.Info);
            }
            else
            { 
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                for (int i = 0; i < array.arraySize; ++i)
                {
                    labelPos.y += LineHeight;
                    labelPos.height = 20;
                    var audioEvent = array.GetArrayElementAtIndex(i).FindPropertyRelative("Event").objectReferenceValue as InAudioEventNode;

                    if (audioEvent != null)
                        GUI.Label(labelPos, audioEvent.GetName, labelStyle);
                    else
                        GUI.Label(labelPos, "Missing event", labelStyle);

                    Rect buttonPos = labelPos;
                    buttonPos.x = pos.width - 200; //Align to right side
                    buttonPos.width = 100;
                    if (audioEvent == null)
                        GUI.enabled = false;

                    SerializedProperty postAtProperty = array.GetArrayElementAtIndex(i).FindPropertyRelative("PostAt");
                    buttonPos.y += 1.5f;
                    var currentValue =
                        Convert.ToInt32(EditorGUI.EnumPopup(buttonPos,
                            (EventHookListData.PostEventAt) postAtProperty.enumValueIndex));
                    buttonPos.y -= 1.5f;
                    if (currentValue != postAtProperty.enumValueIndex)
                        GUI.changed = true;
                    postAtProperty.enumValueIndex = currentValue;

                    buttonPos.x += 108;
                    buttonPos.width = 50;
                    if (GUI.Button(buttonPos, "Find"))
                    {
                        EditorWindow.GetWindow<EventWindow>().Find(audioEvent);
                    }
                    GUI.enabled = true;
                    buttonPos.x = pos.width - 38;
                    buttonPos.width = 35;
                    if (GUI.Button(buttonPos, "X"))
                    {
                        array.DeleteArrayElementAtIndex(i);
                        GUI.changed = true;

                    }
                }
            }

            if (!prop.hasMultipleDifferentValues)
            {
                labelPos.y += DragHeight + 4;
                labelPos.height = DragHeight;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.color = backgroundColor;

                if (FieldName == null)
                    GUI.Button(labelPos,
                        "Drag event here to add " + ObjectNames.NicifyVariableName(fieldInfo.Name) + " event");
                else
                {
                    GUI.Button(labelPos,
                        "Drag event here to add " + FieldName.EventType + " event");
                }
                
                if (labelPos.Contains(Event.current.mousePosition))
                {
                    DrawerHelper.HandleEventDrag(array);
                }
            }
        }
        GUI.color = backgroundColor;

        labelPos.height += 4;
        GUI.skin.label.alignment = defaultAnchor;
        
    }
}

}
