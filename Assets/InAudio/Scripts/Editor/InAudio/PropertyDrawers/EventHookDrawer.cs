using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    //[CanEditMultipleObjects]// Does not work currently, work in progress
    [CustomEditor(typeof(InAudioEventHook))]
    public class EventHookDrawer : Editor
    {
        private SerializedObject hookObj;
        //private SerializedObject hookTargets;
        public void OnEnable()
        {
            hookObj = new SerializedObject(targets);

            //hookTargets = new SerializedObject(targets);

        }

        public override void OnInspectorGUI()
        {
            hookObj.Update();
            EditorGUI.BeginChangeCheck();

            DrawProperty("onEnable");
            DrawProperty("onStart");
            DrawProperty("onDisable");
            DrawProperty("onDestroy");          

            if (!HasCollision())
                EditorGUILayout.HelpBox("No colliders found on this object.", MessageType.Info, true);

            DrawProperty("CollisionList");

            if (EditorGUI.EndChangeCheck())
            {
                hookObj.ApplyModifiedProperties();
            }
        }

        private bool HasCollision()
        {
            var hook = target as MonoBehaviour;

            if (hook.GetComponent<Collider>())
                return true;
#if !UNITY_4_1 && !UNITY_4_2
            if (hook.GetComponent<Collider2D>())
                return true;
#endif
            return false;
        }

        /// <summary>
        /// Draws the AudioEventList by name
        /// </summary>
        /// <param name="name"></param>
        public void DrawProperty(string name)
        {
            var eventList = hookObj.FindProperty(name);
            EditorGUILayout.PropertyField(eventList);
        }
    }


}
