using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BulletForge.Windows
{
    using Elements;
    using Enumerations;
    using Utilities;
    
    /// <summary>
    /// Adds manipulators to the graph view
    /// </summary>
    public class BFGraphViewManipulators
    {
        private BFEditorWindow editorWindow;
        private GraphView graphView;
        
        public BFGraphViewManipulators(GraphView bfGraphView, BFEditorWindow bfEditorWindow)
        {
            graphView = bfGraphView;
            editorWindow = bfEditorWindow;
            
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
                graphView.AddManipulator(CreateContextualMenu($"Add Node ({nodeType})", nodeType.ToString()));
            }
            
            graphView.AddManipulator(CreateContextualMenu("Add Group", "Group"));
        }
        
        /// <summary>
        /// Gets the mouse position in the graph view
        /// </summary>
        /// <returns></returns>
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }
            
            Vector2 localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);
            
            return localMousePosition;
        }
        
        /// <summary>
        /// Creates a contextual menu to add a node of a specific type
        /// </summary>
        /// <param name="contextMenuTitle">The Call to Action text within the menu</param>
        /// <param name="name">ENodeType.ToString() for a Node / Any string title for a group</param>
        /// <returns></returns>
        private IManipulator CreateContextualMenu(string contextMenuTitle, string name)
        {
            ContextualMenuManipulator contextualMenuManipulator;
            
            // Attempt to parse the name to ENodeType
            bool isParsed = Enum.TryParse(name, out ENodeType nodeType);
            
            // If the name is a valid ENodeType, then create a node
            if (isParsed)
            {
                contextualMenuManipulator = new ContextualMenuManipulator(
                    menuEvent => menuEvent.menu.AppendAction(contextMenuTitle,
                        actionEvent => graphView.AddElement(CreateNode(nodeType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );
            }
            else
            {
                contextualMenuManipulator = new ContextualMenuManipulator(
                    menuEvent => menuEvent.menu.AppendAction(contextMenuTitle,
                        actionEvent => graphView.AddElement(CreateGroup(name, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );
            }

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

            // Add all selected nodes to the group
            foreach (GraphElement selectedElement in graphView.selection)
            {
                if (selectedElement is BFNode)
                {
                    BFNode node = (BFNode)selectedElement;
                    group.AddElement(node);
                }
            }

            return group;
        }
        
        
        /// <summary>
        /// Creates a node and adds it to the graph view
        /// </summary>
        /// <param name="nodeType">The type of node to create</param>
        /// <param name="position">The position to create the node</param>
        /// <returns></returns>
        public BFNode CreateNode(ENodeType nodeType, Vector2 position)
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
