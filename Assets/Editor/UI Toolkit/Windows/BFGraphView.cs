using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace BulletForge.Windows
{
    public class BFGraphView : GraphView
    {
        public BFGraphView()
        {
            AddManipulators();
            
            AddGridBackground();

            AddStyles();
        }

        private void AddManipulators()
        {
            SetupZoom(0.1f, 10f); 
            this.AddManipulator(new ContentDragger());
        }

        private void AddGridBackground()
        {
            // Create a grid background
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        
        private void AddStyles()
        {
            StyleSheet styleSheet = EditorGUIUtility.Load("BulletForge/BFGraphViewStyles.uss") as StyleSheet;

            styleSheets.Add(styleSheet);
        }
    }
}
