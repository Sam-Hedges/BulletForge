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
        /// Adds a toolbar to the top of the window
        /// </summary>
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            
            TextField fileNameTextField = BFElementUtility.CreateTextField("BulletPatternFileName", "File Name:");
            
            Button saveButton = BFElementUtility.CreateButton("Save");
            
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            
            rootVisualElement.Add(toolbar);
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