using System;
using System.Collections.Generic;
using System.IO;
using GameLogic.UI.UXTools.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.Editor.UI
{
    public static class UXToolsEditorUtilities
    {
        private static readonly string[] RequiredFolders =
        {
            "Assets/GameScripts/HotFix/GameLogic/UI",
            "Assets/GameScripts/HotFix/GameLogic/UI/UXTools",
            "Assets/GameScripts/HotFix/GameLogic/UI/CodeGen",
            "Assets/GameScripts/HotFix/GameLogic/Editor/UI",
        };

        [MenuItem("UXTools/Setup Selected")]
        public static void SetupSelected()
        {
            var selection = Selection.gameObjects;
            if (selection == null || selection.Length == 0)
            {
                throw new InvalidOperationException("No GameObjects selected.");
            }

            foreach (var go in selection)
            {
                SetupGameObject(go);
            }
        }

        [MenuItem("UXTools/Validate Project Structure")]
        public static void ValidateProjectStructure()
        {
            var missing = new List<string>();
            for (int i = 0; i < RequiredFolders.Length; i++)
            {
                if (!Directory.Exists(RequiredFolders[i]))
                {
                    missing.Add(RequiredFolders[i]);
                }
            }

            if (missing.Count == 0)
            {
                EditorUtility.DisplayDialog("UXTools", "Project structure looks good.", "OK");
                return;
            }

            var message = "Missing folders:\n" + string.Join("\n", missing);
            var create = EditorUtility.DisplayDialog("UXTools", message, "Create", "Cancel");
            if (!create)
            {
                throw new InvalidOperationException("Project structure validation failed.");
            }

            for (int i = 0; i < missing.Count; i++)
            {
                Directory.CreateDirectory(missing[i]);
            }

            AssetDatabase.Refresh();
        }

        private static void SetupGameObject(GameObject go)
        {
            if (go == null)
            {
                throw new ArgumentNullException(nameof(go));
            }

            var image = go.GetComponent<Image>();
            if (image != null && go.GetComponent<UXImage>() == null)
            {
                Undo.AddComponent<UXImage>(go);
            }

            var text = go.GetComponent<Text>();
            if (text != null && go.GetComponent<UXText>() == null)
            {
                Undo.AddComponent<UXText>(go);
            }

            var toggle = go.GetComponent<Toggle>();
            if (toggle != null && go.GetComponent<UXToggle>() == null)
            {
                Undo.AddComponent<UXToggle>(go);
            }
        }
    }
}
