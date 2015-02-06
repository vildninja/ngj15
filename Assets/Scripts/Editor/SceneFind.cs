﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace GameCore.UEditor
{
    public class SceneFind : Editor
    {
        private static string[] scenes;

        [MenuItem("Open Scene/Update Scene Names", false, 0)]
        private static void UpdateNames()
        {
            scenes = ReadNames();

            string full = Application.dataPath + "/Scripts/Editor";

            StreamWriter stream = File.AppendText(full + "/OpenSceneNames.cs");
            

            string classStart = "//Generated by SceneFind.cs \n" +
                                "using UnityEditor; \n" +
                                "namespace GameCore.UEditor { \n"+
                                "public class EditorMapOpener : Editor { \n"; 

            string endClassContent = "}\n}";

            string classContent = "";

            for (int i = 0; i < scenes.Length; i++)
            {
                string path = scenes[i] + ".unity";
                string sceneName = scenes[i].Substring(scenes[i].LastIndexOf('/') + 1); //Get scene name

                string menuItem = "[UnityEditor.MenuItem(\"Open Scene/" + ObjectNames.NicifyVariableName(sceneName) +
                                  "\", false, " + 20 + ")]";
                string function = "private static void " + sceneName + "() {";
                string content = "\tOpenIf(\"" + path + "\");";
                const string functionEnd = "}";

                classContent += Combine(menuItem, function, content, functionEnd);
            }

            string openIf = Combine(
                "private static void OpenIf(string level) {",
                "\tif (EditorApplication.SaveCurrentSceneIfUserWantsTo()){",
                "\t\tEditorApplication.OpenScene(level);",
                "\t}",
                "}");

            stream.Write(classStart + classContent + openIf + endClassContent);
            stream.Close();
            AssetDatabase.Refresh();
        }

        private static string Combine(params string[] lines)
        {
            string final = "";
            for (int i = 0; i < lines.Length; i++)
            {
                final += "\t" + lines[i] + "\n";
            }
            return final;
        }

        private static string[] ReadNames()
        {
            List<string> temp = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {

                if (scene.enabled)
                {
                    string name = scene.path; //.Substring(scene.path.LastIndexOf('/') + 1);
                    name = name.Substring(0, name.Length - 6);
                    temp.Add(name);
                }

            }
            return temp.ToArray();
        }


    }

    public class Tuple<T, U>
    {
        public T One;
        public U Two;

        public Tuple(T one, U two)
        {
            One = one;
            Two = two;
        }
    }
}