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
    /// Adds manipulators to the graph view
    /// </summary>
    public class BFGraphViewManipulators
    {
        private GraphView graphView;
        
        public BFGraphViewManipulators(GraphView graphView)
        {
            this.graphView = graphView;
            
            AddManipulators();
        }
        
        /// <summary>
        /// Allows for control over the graph view
        /// </summary>
        private void AddManipulators()
        {
            graphView.SetupZoom(0.1f, 10f); 
            
            graphView.AddManipulator(new ContentDragger());
            graphView.AddManipulator(new SelectionDragger());
            graphView.AddManipulator(new RectangleSelector());
            
            // Add a contextual rc menu to create each type of node in the graph view
            foreach (ENodeType nodeType in Enum.GetValues(typeof(ENodeType))) {
                graphView.AddManipulator(CreateNodeContextualMenu($"Add Node ({nodeType})", nodeType));
            }
            
            graphView.AddManipulator(CreateGroupContextualMenu());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => graphView.AddElement(CreateGroup("Group", actionEvent.eventInfo.localMousePosition)))
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
            
            graphView.AddElement(group);

            foreach (GraphElement selectedElement in graphView.selection)
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
                menuEvent => menuEvent.menu.AppendAction(title, actionEvent => graphView.AddElement(CreateNode(nodeType, actionEvent.eventInfo.localMousePosition)))
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
        }
}
