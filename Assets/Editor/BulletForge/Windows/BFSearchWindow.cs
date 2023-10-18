using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Windows
{
    using Elements;
    using Enumerations;
    
    /// <summary>
    /// Creates the search window for the graph view
    /// </summary>
    public class BFSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BFGraphView graphView;
        private BFGraphViewManipulators graphViewManipulators;
        private Texture2D indentationIcon;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bfGraphView"></param>
        public void Initialize(BFGraphView bfGraphView, BFGraphViewManipulators bfGraphViewManipulators)
        {
            graphView = bfGraphView;
            graphViewManipulators = bfGraphViewManipulators;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }
        
        /// <summary>
        /// Creates the search tree for the search window
        /// </summary>
        /// <param name="context">Includes parameters for configuring the search window</param>
        /// <returns></returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"))
            };

            // Define groups and their entries
            var groups = new Dictionary<string, ENodeType[]>
            {
                { "Sequence Nodes", new[] { ENodeType.Fire, ENodeType.Vanish, ENodeType.Wait, ENodeType.Repeat } },
                { "Dynamic Nodes", new[] { ENodeType.Speed, ENodeType.Acceleration, ENodeType.Direction } },
                { "Modifier Nodes", new[] { ENodeType.ChangeSpeed, ENodeType.ChangeDirection } },
                { "Groups", null }  // This is a special case
            }; // ADD NEW NODES HERE <-------------------------------------------------------------
            
            // Add the groups and their entries to the search tree
            foreach (var pair in groups)
            {
                searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(pair.Key), 1));

                if (pair.Value != null)
                {
                    foreach (var nodeType in pair.Value)
                    {
                        searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(nodeType.ToString(), indentationIcon))
                        {
                            userData = nodeType,
                            level = 2
                        });
                    }
                }
                else  // Handle the special "Groups" case
                {
                    searchTreeEntries.Add(new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                    {
                        userData = new Group(),
                        level = 2
                    });
                }
            }

            return searchTreeEntries;
        }
        
        /// <summary>
        /// Returns true if the search window entry was selected
        /// </summary>
        /// <param name="searchTreeEntry">A search tree entry</param>
        /// <param name="context">Includes parameters for configuring the search window</param>
        /// <returns></returns>
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphViewManipulators.GetLocalMousePosition(context.screenMousePosition, true);
            
            // If the search tree entry is a group, then create a group
            if (searchTreeEntry.userData is Group)
            {
                graphViewManipulators.CreateGroup("Group", localMousePosition);
                return true;
            }
            
            // If the search tree entry is a node, then create a node
            if (searchTreeEntry.userData is ENodeType nodeType)
            {
                BFNode node = graphViewManipulators.CreateNode(nodeType, localMousePosition) as BFNode;
                graphView.AddElement(node);
                return true;
            }

            return false;
        }
    }
}