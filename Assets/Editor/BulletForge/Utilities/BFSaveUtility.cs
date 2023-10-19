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
    using Data;
    
    /// <summary>
    /// 
    /// </summary>
    public class BFSaveUtility
    {
        #region Variables
        
        private BFGraphView graphView;
        
        // Used for saving the graph
        private string graphFileName;
        private string containerFolderPath;
        
        private List<BFNode> nodes;
        private List<BFGroup> groups;
        
        private static Dictionary<string, BFGroupSO> createdGroups;
        private static Dictionary<string, BFNodeSO> createdNodes;
        
        #endregion
        
        public BFSaveUtility(BFGraphView bfGraphView, string fileName)
        {
            graphView = bfGraphView;
            
            graphFileName = fileName;
            containerFolderPath = $"Assets/Runtime/BulletForge/Patterns/{graphFileName}";
            
            nodes = new List<BFNode>();
            groups = new List<BFGroup>();
            
            createdGroups = new Dictionary<string, BFGroupSO>();
            createdNodes = new Dictionary<string, BFNodeSO>();
        }
        
        #region Save Methods
        
        /// <summary>
        /// Saves the graph to a ScriptableObject
        /// </summary>
        public void Save()
        {
            CreateDefaultFolders();

            GetElementsFromGraphView();
            
            // Create the ScriptableObjects
            BFGraphSaveDataSO graphData = CreateAsset<BFGraphSaveDataSO>("Assets/Editor/BulletForge/Graphs", $"{graphFileName}Graph");
            graphData.Initialize(graphFileName);

            BFPatternContainerSO patternContainer = CreateAsset<BFPatternContainerSO>(containerFolderPath, graphFileName);
            patternContainer.Initialize(graphFileName);

            SaveGroups(graphData, patternContainer);
            SaveNodes(graphData, patternContainer);

            SaveAsset(graphData);
            SaveAsset(patternContainer);
        }
        
        /// <summary>
        /// Manages how the groups are saved to a ScriptableObject
        /// </summary>
        /// <param name="graphData">The scriptable object used to save the graph</param>
        /// <param name="patternContainer">The main scriptable object that contains all the nodes and groups</param>
        private void SaveGroups(BFGraphSaveDataSO graphData, BFPatternContainerSO patternContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (BFGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, patternContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }
        
        /// <summary>
        /// Saves the groups to a ScriptableObject
        /// </summary>
        /// <param name="group">The group being saved</param>
        /// <param name="graphData">The scriptable object being saved to</param>
        private void SaveGroupToGraph(BFGroup group, BFGraphSaveDataSO graphData)
        {
            // Create a new group save data
            BFGroupSaveData groupData = new BFGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };
            
            // Add the group save data to the graph data's list of groups
            graphData.Groups.Add(groupData);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="patternContainer"></param>
        private void SaveGroupToScriptableObject(BFGroup group, BFPatternContainerSO patternContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Patterns");

            BFGroupSO patternGroup = CreateAsset<BFGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            patternGroup.Initialize(groupName);

            createdGroups.Add(group.ID, patternGroup);

            patternContainer.Groups.Add(patternGroup, new List<BFNodeSO>());

            SaveAsset(patternGroup);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphData"></param>
        /// <param name="patternContainer"></param>
        private void SaveNodes(BFGraphSaveDataSO graphData, BFPatternContainerSO patternContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeIDs = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeIDs = new List<string>();

            foreach (BFNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, patternContainer);

                if (node.Group != null)
                {
                    groupedNodeIDs.AddItem(node.Group.title, node.ID);

                    continue;
                }

                ungroupedNodeIDs.Add(node.ID);
            }

            UpdateNodeConnections();

            UpdateOldGroupedNodes(groupedNodeIDs, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeIDs, graphData);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="graphData"></param>
        private void SaveNodeToGraph(BFNode node, BFGraphSaveDataSO graphData)
        {
            List<BFConnectionSaveData> nodeConnections = BFIOUtility.CloneNodes(node.Connections);

            BFNodeSaveData nodeData = new BFNodeSaveData()
            {
                ID = node.ID,
                Connections = nodeConnections,
                GroupID = node.Group?.ID,
                NodeType = node.NodeType,
                Position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="patternContainer"></param>
        private void SaveNodeToScriptableObject(BFNode node, BFPatternContainerSO patternContainer)
        {
            BFNodeSO pattern;

            if (node.Group != null)
            {
                pattern = CreateAsset<BFNodeSO>($"{containerFolderPath}/Groups/{node.Group.title}/Patterns", node.NodeType.ToString());

                patternContainer.Groups.AddItem(createdGroups[node.Group.ID], pattern);
            }
            else
            {
                pattern = CreateAsset<BFNodeSO>($"{containerFolderPath}/Global/Patterns", node.NodeType.ToString());

                patternContainer.UngroupedNodes.Add(pattern);
            }

            pattern.Initialize(
                node.ID,
                ConvertNodesToConnection(node.Connections),
                node.NodeType,
                node.IsStartingNode()
            );

            createdNodes.Add(node.ID, pattern);

            SaveAsset(pattern);
        }
        
        /// <summary>
        /// Saves the asset to the project
        /// </summary>
        public void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private List<BFConnectionData> ConvertNodesToConnection(List<BFConnectionSaveData> nodes)
        {
            List<BFConnectionData> pattern = new List<BFConnectionData>();

            foreach (BFConnectionSaveData node in nodes)
            {
                BFConnectionData Data = new BFConnectionData()
                {
                     NodeID = node.NodeID
                };

                pattern.Add(Data);
            }

            return pattern;
        }
        
        #endregion

        #region Update Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentGroupIDs"></param>
        /// <param name="graphData"></param>
        private void UpdateOldGroups(List<string> currentGroupIDs, BFGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupIDs != null && graphData.OldGroupIDs.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupIDs.Except(currentGroupIDs).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupIDs = new List<string>(currentGroupIDs);
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void UpdateNodeConnections()
        {
            foreach (BFNode node in nodes)
            {
                BFNodeSO pattern = createdNodes[node.ID];

                for (int Index = 0; Index < node.Connections.Count; ++Index)
                {
                    BFConnectionSaveData nodePattern = node.Connections[Index];

                    if (string.IsNullOrEmpty(nodePattern.NodeID))
                    {
                        continue;
                    }

                    pattern.Connections[Index].NextNode = createdNodes[nodePattern.NodeID];

                    SaveAsset(pattern);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentGroupedNodeNames"></param>
        /// <param name="graphData"></param>
        private void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, BFGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeIDs != null && graphData.OldGroupedNodeIDs.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeIDs)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Patterns", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeIDs = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUngroupedNodeNames"></param>
        /// <param name="graphData"></param>
        private void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, BFGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeIDs != null && graphData.OldUngroupedNodeIDs.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeIDs.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Patterns", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeIDs = new List<string>(currentUngroupedNodeNames);
        }
        
        #endregion
        
        #region Creation Methods

        /// <summary>
        /// Creates the file structure for the pattern
        /// </summary>
        private void CreateDefaultFolders()
        {
            CreateFolder("Assets", "Editor");
            CreateFolder("Assets/Editor", "BulletForge");
            CreateFolder("Assets/Editor/BulletForge", "Graphs");

            CreateFolder("Assets", "Runtime");
            CreateFolder("Assets/Runtime", "BulletForge");
            CreateFolder("Assets/Runtime/BulletForge", "Patterns");

            CreateFolder("Assets/Runtime/BulletForge/Patterns", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Nodes");
        }
        
        /// <summary>
        /// Checks if the folder exists and creates it if it doesn't
        /// </summary>
        /// <param name="parentFolderPath">The path to the parent folder, starting with "Assets/"</param>
        /// <param name="newFolderName">The name of the new folder</param>
        public void CreateFolder(string parentFolderPath, string newFolderName)
        {
            // If the folder already exists, don't create it
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }
            
            // Create the folder
            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }
        
        /// <summary>
        /// Creates an Asset of the specified type
        /// </summary>
        /// <param name="path">The file path of where to save the graph</param>
        /// <param name="assetName">The name of the file once saved</param>
        /// <typeparam name="T">The type of the asset to create</typeparam>
        public T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            // Create the full path to the asset
            string fullPath = $"{path}/{assetName}.asset";
            
            // Load the asset from the path
            T asset = BFIOUtility.LoadAsset<T>(path, assetName);
            
            // If the asset doesn't exists, create it
            if (asset == null)
            {
                // Creates a new instance of the specified type
                asset = ScriptableObject.CreateInstance<T>();
                
                // Creates a new native Unity asset
                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }
        
        #endregion

        #region Removal Methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assetName"></param>
        public void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }
        
        #endregion
        
        #region Fetch Methods
        
        /// <summary>
        /// Initializes the lists of nodes and groups
        /// </summary>
        private void GetElementsFromGraphView()
        {
            Type groupType = typeof(BFGroup);

            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is BFNode node)
                {
                    nodes.Add(node);
                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    BFGroup group = graphElement as BFGroup;
                    groups.Add(group);
                    return;
                }
            });
        }
        #endregion
    }
}
