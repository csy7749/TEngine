using System;
using System.Collections.Generic;
using System.IO;
using GameLogic.UI.CodeGen;
using UnityEditor;
using UnityEngine;

namespace GameLogic.Editor.UI
{
    [CustomEditor(typeof(UIControlData))]
    public sealed class UIControlDataInspector : UnityEditor.Editor
    {
        private const string OutputFolderPrefKey = "UI_CODEGEN_OUTPUT";
        private const string GenerateViewModelPrefKey = "UI_CODEGEN_VIEWMODEL";
        private const string TargetPrefKey = "UI_CODEGEN_TARGET";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            EditorGUILayout.Space();

            DrawCollectionSection();
            EditorGUILayout.Space();
            DrawGenerationSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCollectionSection()
        {
            EditorGUILayout.LabelField("Component Collection", EditorStyles.boldLabel);
            if (GUILayout.Button("Collect Components"))
            {
                CollectComponents();
            }
        }

        private void CollectComponents()
        {
            var data = (UIControlData)target;
            var components = UIComponentCollector.Collect(data.gameObject);
            data.ApplyComponents(components);
            EditorUtility.SetDirty(data);
        }

        private void DrawGenerationSection()
        {
            EditorGUILayout.LabelField("Code Generation", EditorStyles.boldLabel);

            var outputDirectory = EditorPrefs.GetString(OutputFolderPrefKey, Application.dataPath);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Folder", GUILayout.Width(100));
            EditorGUILayout.TextField(outputDirectory);
            if (GUILayout.Button("Select", GUILayout.Width(80)))
            {
                outputDirectory = SelectOutputDirectory(outputDirectory);
                EditorPrefs.SetString(OutputFolderPrefKey, outputDirectory);
            }
            EditorGUILayout.EndHorizontal();

            var target = (UIGenerationTarget)EditorPrefs.GetInt(TargetPrefKey, (int)UIGenerationTarget.Window);
            target = (UIGenerationTarget)EditorGUILayout.EnumPopup("Target", target);
            EditorPrefs.SetInt(TargetPrefKey, (int)target);

            var generateViewModel = EditorPrefs.GetBool(GenerateViewModelPrefKey, true);
            generateViewModel = EditorGUILayout.Toggle("Generate ViewModel", generateViewModel);
            EditorPrefs.SetBool(GenerateViewModelPrefKey, generateViewModel);

            if (GUILayout.Button("Generate Code"))
            {
                GenerateCode(outputDirectory, target, generateViewModel);
            }
        }

        private static string SelectOutputDirectory(string current)
        {
            var selected = EditorUtility.OpenFolderPanel("Select Output Folder", current, string.Empty);
            if (string.IsNullOrWhiteSpace(selected))
            {
                return current;
            }

            return selected;
        }

        private void GenerateCode(string outputDirectory, UIGenerationTarget target, bool generateViewModel)
        {
            var data = (UIControlData)target;
            var options = new UICodeGenerationOptions(outputDirectory, target, generateViewModel);
            var results = UICodeGenerator.Generate(data, options);
            UICodePreviewWindow.ShowPreview(results);
        }
    }

    public sealed class UICodePreviewWindow : EditorWindow
    {
        private List<UICodeGenerationResult> _results = new List<UICodeGenerationResult>();
        private int _selectedIndex;
        private Vector2 _scroll;

        public static void ShowPreview(IReadOnlyList<UICodeGenerationResult> results)
        {
            if (results == null || results.Count == 0)
            {
                throw new InvalidOperationException("No code generation results to preview.");
            }

            var window = GetWindow<UICodePreviewWindow>(true, "Code Preview");
            window.Initialize(results);
            window.Show();
        }

        private void Initialize(IReadOnlyList<UICodeGenerationResult> results)
        {
            _results = new List<UICodeGenerationResult>(results);
            _selectedIndex = 0;
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawPreview();
            DrawActions();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < _results.Count; i++)
            {
                var label = Path.GetFileName(_results[i].Path);
                if (GUILayout.Toggle(_selectedIndex == i, label, EditorStyles.toolbarButton))
                {
                    _selectedIndex = i;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPreview()
        {
            var content = _results[_selectedIndex].Content;
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.TextArea(content, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void DrawActions()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Files"))
            {
                WriteFiles(_results);
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void WriteFiles(IReadOnlyList<UICodeGenerationResult> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                var path = results[i].Path;
                if (File.Exists(path))
                {
                    var overwrite = EditorUtility.DisplayDialog(
                        "Overwrite File",
                        $"File already exists:\n{path}\n\nOverwrite?",
                        "Overwrite",
                        "Cancel");
                    if (!overwrite)
                    {
                        throw new InvalidOperationException("Code generation canceled due to overwrite refusal.");
                    }
                }

                File.WriteAllText(path, results[i].Content);
            }

            AssetDatabase.Refresh();
        }
    }
}

