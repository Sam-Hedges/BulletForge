using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

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
        
        #region Overrides
        
        /// <summary>
        ///   <para>Get all ports compatible with given port</para>
        /// </summary>
        /// <param name="startPort">Start port to validate against</param>
        /// <param name="nodeAdapter">Node adapter</param>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            
            ports.ForEach(port => {
                // If the port is the same as the start port, then skip
                // If the port is on the same node as the start port, then skip
                // If the port is the same direction as the start port, then skip
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction) {
                    compatiblePorts.Add(port);
                }
            });
            
            return compatiblePorts;
        }
        
        #endregion

        #region Manipulator Methods

        /// <summary>
        /// Allows for control over the graph view
        /// </summary>
        private void AddManipulators()
        {
            SetupZoom(0.1f, 10f); 
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            // Add a contextual rc menu to create each type of node in the graph view
            foreach (ENodeType nodeType in Enum.GetValues(typeof(ENodeType))) {
                this.AddManipulator(CreateNodeContextualMenu($"Add Node ({nodeType})", nodeType));
            }
            
            this.AddManipulator(CreateGroupContextualMenu());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("Group", actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }
        
        /// <summary>
        /// Creates a group and adds it to the graph view
        /// </summary>
        /// <param name="position">The position to create the Group</param>
        /// <returns></returns>
        public BFGroup CreateGroup(string title, Vector2 position)
        {
            BFGroup group = new BFGroup(title, position);
            
            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is BFNode))
                {
                    continue;
                }

                BFNode node = (BFNode) selectedElement;

                group.AddElement(node);
            }

            return group;
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
        
        #endregion

        #region Visual Methods
        
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
            StyleSheet graphViewStyleSheet = EditorGUIUtility.Load("BulletForge/BFGraphViewStyles.uss") as StyleSheet;
            StyleSheet nodeStyleSheet = EditorGUIUtility.Load("BulletForge/BFNodeStyles.uss") as StyleSheet;

            styleSheets.Add(graphViewStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }
        
        #endregion
    }
}
