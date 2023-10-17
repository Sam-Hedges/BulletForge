using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;


namespace BulletForge.Windows
{
    using Data.Error;
    using Data.Save;
    using Elements;
    using Enumerations;
    using Utilities;
    
    /// <summary>
    /// Governs the layout of the graph view that contains the nodes
    /// </summary>
    public class BFGraphView : GraphView
    {
        
        private BFEditorWindow editorWindow;
        private BFSearchWindow searchWindow;

        private SerializableDictionary<string, BFNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, BFGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, BFNodeErrorData>> groupedNodes;

        private int nameErrorsAmount;

        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }

            set
            {
                nameErrorsAmount = value;

                if (nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        
        // Called when the graph view is created
        public BFGraphView(BFEditorWindow bfEditorWindow)
        {
            editorWindow = bfEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, BFNodeErrorData>();
            groups = new SerializableDictionary<string, BFGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, BFNodeErrorData>>();

            AddManipulators();
            AddGridBackground();
            AddSearchWindow();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
        }

        #region Overrides
        
        /// <summary>
        /// Gets the ports that are compatible with the start port
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            
            ports.ForEach(port => {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction) {
                    compatiblePorts.Add(port);
                }
            });
            
            return compatiblePorts;
        }
        
        #endregion

        #region Manipulator Methods

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

            AddGroup(group);

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
            
            node.Initialize(nodeType.ToString(), this, position);
            node.Draw();
            
            return node;
        }
        public BFNode CreateNode(string nodeName, ENodeType nodeType, Vector2 position, bool shouldDraw = true)
        {
            Type type = Type.GetType($"BF.Elements.BF{nodeType}Node");
            
            if (type == null) {
                Debug.LogError($"BFGraphView.CreateNode: Type BF{nodeType}Node not found");
                return new BFNode();
            }
            
            BFNode node = Activator.CreateInstance(type) as BFNode;


            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

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
        
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(BFGroup);
                Type edgeType = typeof(Edge);

                List<BFGroup> groupsToDelete = new List<BFGroup>();
                List<BFNode> nodesToDelete = new List<BFNode>();
                List<Edge> edgesToDelete = new List<Edge>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is BFNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge) selectedElement;

                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if (selectedElement.GetType() != groupType)
                    {
                        continue;
                    }

                    BFGroup group = (BFGroup) selectedElement;

                    groupsToDelete.Add(group);
                }

                foreach (BFGroup groupToDelete in groupsToDelete)
                {
                    List<BFNode> groupNodes = new List<BFNode>();

                    foreach (GraphElement groupElement in groupToDelete.containedElements)
                    {
                        if (!(groupElement is BFNode))
                        {
                            continue;
                        }

                        BFNode groupNode = (BFNode) groupElement;

                        groupNodes.Add(groupNode);
                    }

                    groupToDelete.RemoveElements(groupNodes);

                    RemoveGroup(groupToDelete);

                    RemoveElement(groupToDelete);
                }

                DeleteElements(edgesToDelete);

                foreach (BFNode nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete.Group != null)
                    {
                        nodeToDelete.Group.RemoveElement(nodeToDelete);
                    }

                    RemoveUngroupedNode(nodeToDelete);

                    nodeToDelete.DisconnectAllPorts();

                    RemoveElement(nodeToDelete);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is BFNode))
                    {
                        continue;
                    }

                    BFGroup dsGroup = (BFGroup) group;
                    BFNode node = (BFNode) element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, dsGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is BFNode))
                    {
                        continue;
                    }

                    BFGroup dsGroup = (BFGroup) group;
                    BFNode node = (BFNode) element;

                    RemoveGroupedNode(node, dsGroup);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                BFGroup dsGroup = (BFGroup) group;

                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        --NameErrorsAmount;
                    }
                }

                RemoveGroup(dsGroup);

                dsGroup.OldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        BFNode nextNode = (BFNode) edge.input.node;

                        BFPatternSaveData choiceData = (BFPatternSaveData) edge.output.userData;

                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;

                        BFPatternSaveData choiceData = (BFPatternSaveData) edge.output.userData;

                        choiceData.NodeID = "";
                    }
                }

                return changes;
            };
        }

        public void AddUngroupedNode(BFNode node)
        {
            string nodeName = node.PatternType.ToString().ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                BFNodeErrorData nodeErrorData = new BFNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }

            List<BFNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNode(BFNode node)
        {
            string nodeName = node.PatternType.ToString().ToLower();

            List<BFNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);

            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                ungroupedNodesList[0].ResetStyle();

                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }

        private void AddGroup(BFGroup group)
        {
            string groupName = group.title.ToLower();

            if (!groups.ContainsKey(groupName))
            {
                BFGroupErrorData groupErrorData = new BFGroupErrorData();

                groupErrorData.Groups.Add(group);

                groups.Add(groupName, groupErrorData);

                return;
            }

            List<BFGroup> groupsList = groups[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.Color;

            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++NameErrorsAmount;

                groupsList[0].SetErrorStyle(errorColor);
            }
        }

        private void RemoveGroup(BFGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<BFGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);

            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --NameErrorsAmount;

                groupsList[0].ResetStyle();

                return;
            }

            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }

        public void AddGroupedNode(BFNode node, BFGroup group)
        {
            string nodeName = node.PatternType.ToString().ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, BFNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                BFNodeErrorData nodeErrorData = new BFNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<BFNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveGroupedNode(BFNode node, BFGroup group)
        {
            string nodeName = node.PatternType.ToString().ToLower();

            node.Group = null;

            List<BFNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }
        

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<BFSearchWindow>();
            }

            searchWindow.Initialize(this);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }
        

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));

            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();

            NameErrorsAmount = 0;
        }
        
    }
}
