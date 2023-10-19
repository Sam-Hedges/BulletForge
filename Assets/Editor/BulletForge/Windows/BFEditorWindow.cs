using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BulletForge.Windows
{
    using Utilities;
    
    /// <summary>
    /// Contains the graph view and other elements that make up the pattern graph window
    /// </summary>
    public class BFEditorWindow : EditorWindow
    {
        private static BFGraphView graphView;
        private static TextField fileNameTextField;
        private static Button saveButton;
        
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
            graphView = new BFGraphView(this);
            graphView.StretchToParentSize(); // Make the GraphView fill the entire window
            rootVisualElement.Add(graphView);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFileName"></param>
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }
        
        /// <summary>
        /// Adds a toolbar to the top of the window
        /// </summary>
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            
            fileNameTextField = BFElementUtility.CreateTextField("BulletPatternFileName", "File Name:", callback =>
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
            //graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName("BulletPatternFileName");
        }
        
        /// <summary>
        /// Applies the visual parameters within the stylesheet to the window
        /// </summary>
        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("BulletForge/BFVariables.uss");
            rootVisualElement.AddStyleSheets("BulletForge/BFToolbarStyles.uss");
        }
    }
}