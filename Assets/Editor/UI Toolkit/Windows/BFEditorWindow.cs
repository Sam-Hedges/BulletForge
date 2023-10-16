using UnityEditor;
using UnityEngine.UIElements;

namespace BulletForge.Windows
{
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

            AddStyles();
        }
        
        /// <summary>
        /// Adds a GraphView to the window
        /// </summary>
        private void AddGraphView()
        {
            // Create a new GraphView
            BFGraphView graphView = new BFGraphView();
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
    }
}