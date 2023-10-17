using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BulletForge.Windows
{
    using System;
    using Utilities;
    
    /// <summary>
    /// Contains the graph view and other elements that make up the pattern graph window
    /// </summary>
    public class BFEditorWindow : EditorWindow
    {
        private BFGraphView graphView;

        private readonly string defaultFileName = "PatternFileName";

        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;
        
        [MenuItem("BulletForge/Pattern Graph")]
        public static void Open()
        { 
            // Get existing open window or if none, make a new one
            GetWindow<BFEditorWindow>("Pattern Graph"); 
        }
        
        /// <summary>
        /// Called when the window is opened.
        /// </summary>
        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }
        
        /// <summary>
        /// Adds a GraphView to the window
        /// </summary>
        private void AddGraphView()
        {
            // Create a new GraphView
            BFGraphView graphView = new BFGraphView(this);
            graphView.StretchToParentSize(); // Make the GraphView fill the entire window
            rootVisualElement.Add(graphView);
        }
        
        /// <summary>
        /// Applies the visual parameters within the stylesheet to the window
        /// </summary>
        private void AddStyles()
        {
            StyleSheet styleSheet = EditorGUIUtility.Load("BulletForge/BFVariables.uss") as StyleSheet;

            rootVisualElement.styleSheets.Add(styleSheet);
        }
        
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = BFElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = BFElementUtility.CreateButton("Save", () => Save());

            Button loadButton = BFElementUtility.CreateButton("Load", () => Load());
            Button clearButton = BFElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = BFElementUtility.CreateButton("Reset", () => ResetGraph());
            

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);

            toolbar.AddStyleSheets("DialogueSystem/BFToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

                return;
            }

            BFIOUtility.Initialize(graphView, fileNameTextField.value);
            BFIOUtility.Save();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();

            BFIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            BFIOUtility.Load();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(defaultFileName);
        }
        

        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
    }
}