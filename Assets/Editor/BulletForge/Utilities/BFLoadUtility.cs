using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Utilities
{
    using Data.Save;
    using Elements;
    using ScriptableObjects;
    using Windows;
    
    /// <summary>
    /// 
    /// </summary>
    public class BFLoadUtility
    {
        private BFGraphView graphView;
        private BFGraphViewManipulators graphViewManipulators;
        
        // Used for saving the graph
        private string graphFileName;
        
        private static Dictionary<string, BFGroup> loadedGroups;
        private static Dictionary<string, BFNode> loadedNodes;
        
        public BFLoadUtility(BFGraphView bfGraphView, string fileName)
        {
            graphView = bfGraphView;
            graphViewManipulators = bfGraphView.graphViewManipulators;
            
            graphFileName = fileName;
            
            loadedGroups = new Dictionary<string, BFGroup>();
            loadedNodes = new Dictionary<string, BFNode>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            BFGraphSaveDataSO graphData = BFIOUtility.LoadAsset<BFGraphSaveDataSO>("Assets/Editor/PatternSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/PatternSystem/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            BFEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groups"></param>
        private void LoadGroups(List<BFGroupSaveData> groups)
        {
            foreach (BFGroupSaveData groupData in groups)
            {
                BFGroup group = graphViewManipulators.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        private void LoadNodes(List<BFNodeSaveData> nodes)
        {
            foreach (BFNodeSaveData nodeData in nodes)
            {
                List<BFNodeSaveData> nodeList = BFIOUtility.CloneNodes(nodes);

                BFNode node = graphViewManipulators.CreateNode(nodeData.NodeType, nodeData.Position);

                node.ID = nodeData.ID;
                node.Data = nodeList;

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                BFGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, BFNode> loadedNode in loadedNodes)
            {
                foreach (Port Port in loadedNode.Value.outputContainer.Children())
                {
                    BFNodeSaveData patternData = (BFNodeSaveData) Port.userData;

                    if (string.IsNullOrEmpty(patternData.ID))
                    {
                        continue;
                    }

                    BFNode nextNode = loadedNodes[patternData.ID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = Port.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }
        
    }
}
