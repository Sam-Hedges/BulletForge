using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;


namespace BulletForge.Windows
{
    using Elements;
    using Enumerations;
    
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
        
        /// <summary>s
        /// Allows for control over the graph view
        /// </summary>
        private void AddManipulators()
        {
            SetupZoom(0.1f, 10f); 
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            // Add a contextual menu to add each type of node
            foreach (ENodeType nodeType in Enum.GetValues(typeof(ENodeType))) {
                this.AddManipulator(CreateNodeContextualMenu($"Add Node ({nodeType})", nodeType));
            }
        }
        
        /// <summary>
        /// Creates a contextual menu to add a node of a specific type
        /// </summary>
        /// <param name="title">The text that is displayed in the contextual menu</param>
        /// <param name="nodeType">The type of node to display</param>
        /// <returns></returns>
        private IManipulator CreateNodeContextualMenu(string title, ENodeType nodeType) 
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(title, actionEvent => AddElement(CreateNode(nodeType, actionEvent.eventInfo.localMousePosition)))
                );

            return contextualMenuManipulator;
        }
        
        /// <summary>
        /// Creates a node and adds it to the graph view
        /// </summary>
        /// <param name="nodeType">The type of node to create</param>
        /// <param name="position">The position to create the node</param>
        /// <returns></returns>
        private BFNode CreateNode(ENodeType nodeType, Vector2 position)
        {
            Type type = Type.GetType($"BulletForge.Elements.BF{nodeType}Node");
            
            if (type == null) {
                Debug.LogError($"BFGraphView.CreateNode: Type BF{nodeType}Node not found");
                return new BFNode();
            }
            
            BFNode node = Activator.CreateInstance(type) as BFNode;
            
            node.Initialize(position);
            node.Draw();
            
            return node;
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
