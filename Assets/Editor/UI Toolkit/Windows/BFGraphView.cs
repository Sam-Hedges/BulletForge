using BulletForge.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Windows
{
    using Elements;
    
    /// <summary>
    /// Governs the layout of the graph view that contains the nodes
    /// </summary>
    public class BFGraphView : GraphView
    {
        // Called when the graph view is created
        public BFGraphView()
        {
            AddManipulators();
            
            AddGridBackground();
    
            AddStyles();
        }
        
        /// <summary>
        /// Creates a node and adds it to the graph view
        /// </summary>
        private BFNode CreateNode(Vector2 position)
        {
            BFNode node = new BFNode();
            node.Initialize(position);
            node.Draw();
            AddElement(node);
            return node;
        }
        
        /// <summary>s
        /// Allows for control over the graph view
        /// </summary>
        private void AddManipulators()
        {
            SetupZoom(0.1f, 10f); 
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextualMenu()); 
        }

        private IManipulator CreateNodeContextualMenu() 
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition)))
                );

            return contextualMenuManipulator;
        }

        /// <summary>
        /// Adds a grid background to the graph view
        /// </summary>
        private void AddGridBackground()
        {
            // Create a grid background
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        
        /// <summary>
        /// Applies the visual parameters within the stylesheet to the graph view
        /// </summary>
        private void AddStyles()
        {
            StyleSheet styleSheet = EditorGUIUtility.Load("BulletForge/BFGraphViewStyles.uss") as StyleSheet;

            styleSheets.Add(styleSheet);
        }
    }
}
