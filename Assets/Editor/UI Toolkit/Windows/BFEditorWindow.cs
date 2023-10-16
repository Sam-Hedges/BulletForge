using UnityEditor;
using UnityEngine.UIElements;

namespace BulletForge.Windows
{
    public class BFEditorWindow : EditorWindow
    {
        [MenuItem("BulletForge/Pattern Graph")]
        public static void Open()
        { 
            // Get existing open window or if none, make a new one
            GetWindow<BFEditorWindow>("Pattern Graph"); 
        }
        
        // This method is called when the window is opened.
        private void CreateGUI()
        {
            AddGraphView();

            AddStyles();
        }
        
        private void AddGraphView()
        {
            // Create a new GraphView
            BFGraphView graphView = new BFGraphView();
            graphView.StretchToParentSize(); // Make the GraphView fill the entire window
            rootVisualElement.Add(graphView);
        }
        
        private void AddStyles()
        {
            StyleSheet styleSheet = EditorGUIUtility.Load("BulletForge/BFVariables.uss") as StyleSheet;

            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}