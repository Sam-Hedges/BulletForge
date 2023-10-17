using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Utilities
{
    using Data;
    using Data.Save;
    using Elements;
    using ScriptableObjects;
    using Windows;

    public static class BFIOUtility
    {
        private static BFGraphView graphView;

        private static string graphFileName;
        private static string containerFolderPath;

        private static List<BFNode> nodes;
        private static List<BFGroup> groups;

        private static Dictionary<string, BFPatternGroupSO> createdPatternGroups;
        private static Dictionary<string, BFPatternSO> createdPatterns;

        private static Dictionary<string, BFGroup> loadedGroups;
        private static Dictionary<string, BFNode> loadedNodes;

        public static void Initialize(BFGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/BulletForge/Patterns/{graphName}";

            nodes = new List<BFNode>();
            groups = new List<BFGroup>();

            createdPatternGroups = new Dictionary<string, BFPatternGroupSO>();
            createdPatterns = new Dictionary<string, BFPatternSO>();

            loadedGroups = new Dictionary<string, BFGroup>();
            loadedNodes = new Dictionary<string, BFNode>();
        }

        public static void Save()
        {
            CreateDefaultFolders();

            GetElementsFromGraphView();

            BFGraphSaveDataSO graphData = CreateAsset<BFGraphSaveDataSO>("Assets/Editor/BulletForge/Graphs", $"{graphFileName}Graph");

            graphData.Initialize(graphFileName);

            BFPatternContainerSO patternContainer = CreateAsset<BFPatternContainerSO>(containerFolderPath, graphFileName);

            patternContainer.Initialize(graphFileName);

            SaveGroups(graphData, patternContainer);
            SaveNodes(graphData, patternContainer);

            SaveAsset(graphData);
            SaveAsset(patternContainer);
        }

        private static void SaveGroups(BFGraphSaveDataSO graphData, BFPatternContainerSO patternContainer)
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

        private static void SaveGroupToGraph(BFGroup group, BFGraphSaveDataSO graphData)
        {
            BFGroupSaveData groupData = new BFGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(BFGroup group, BFPatternContainerSO patternContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Patterns");

            BFPatternGroupSO patternGroup = CreateAsset<BFPatternGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            patternGroup.Initialize(groupName);

            createdPatternGroups.Add(group.ID, patternGroup);

            patternContainer.PatternGroups.Add(patternGroup, new List<BFPatternSO>());

            SaveAsset(patternGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, BFGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void SaveNodes(BFGraphSaveDataSO graphData, BFPatternContainerSO patternContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();

            foreach (BFNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, patternContainer);

                if (node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.NodeType.ToString());

                    continue;
                }

                ungroupedNodeNames.Add(node.PatternType.ToString());
            }

            UpdatePatternConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(BFNode node, BFGraphSaveDataSO graphData)
        {
            List<BFPatternSaveData> nodes = CloneNodes(node.Pattern);

            BFNodeSaveData nodeData = new BFNodeSaveData()
            {
                ID = node.ID,
                Text = node.Text,
                GroupID = node.Group?.ID,
                NodeType = node.NodeType,
                Position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(BFNode node, BFPatternContainerSO patternContainer)
        {
            BFPatternSO pattern;

            if (node.Group != null)
            {
                pattern = CreateAsset<BFPatternSO>($"{containerFolderPath}/Groups/{node.Group.title}/Patterns", node.NodeType.ToString());

                patternContainer.PatternGroups.AddItem(createdPatternGroups[node.Group.ID], pattern);
            }
            else
            {
                pattern = CreateAsset<BFPatternSO>($"{containerFolderPath}/Global/Patterns", node.NodeType.ToString());

                patternContainer.UngroupedPatterns.Add(pattern);
            }

            pattern.Initialize(
                node.NodeType.ToString(),
                node.Text,
                ConvertNodesToPattern(node.Pattern),
                node.PatternType,
                node.IsStartingNode()
            );

            createdPatterns.Add(node.ID, pattern);

            SaveAsset(pattern);
        }

        private static List<BFPatternData> ConvertNodesToPattern(List<BFPatternSaveData> nodes)
        {
            List<BFPatternData> pattern = new List<BFPatternData>();

            foreach (BFPatternSaveData node in nodes)
            {
                BFPatternData Data = new BFPatternData()
                {
                    Text = node.Text
                };

                pattern.Add(Data);
            }

            return pattern;
        }

        private static void UpdatePatternConnections()
        {
            foreach (BFNode node in nodes)
            {
                BFPatternSO pattern = createdPatterns[node.ID];

                for (int Index = 0; Index < node.Pattern.Count; ++Index)
                {
                    BFPatternSaveData nodePattern = node.Pattern[Index];

                    if (string.IsNullOrEmpty(nodePattern.NodeID))
                    {
                        continue;
                    }

                    pattern.Pattern[Index].NextPattern = createdPatterns[nodePattern.NodeID];

                    SaveAsset(pattern);
                }
            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, BFGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
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

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, BFGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Patterns", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        public static void Load()
        {
            BFGraphSaveDataSO graphData = LoadAsset<BFGraphSaveDataSO>("Assets/Editor/PatternSystem/Graphs", graphFileName);

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

        private static void LoadGroups(List<BFGroupSaveData> groups)
        {
            foreach (BFGroupSaveData groupData in groups)
            {
                BFGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<BFNodeSaveData> nodes)
        {
            foreach (BFNodeSaveData nodeData in nodes)
            {
                List<BFPatternSaveData> nodeList = CloneNodes(nodeData.Pattern);

                BFNode node = graphView.CreateNode(nodeData.Name, nodeData.NodeType, nodeData.Position, false);

                node.ID = nodeData.ID;
                node.Pattern = nodeList;
                node.Text = nodeData.Text;

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

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, BFNode> loadedNode in loadedNodes)
            {
                foreach (Port Port in loadedNode.Value.outputContainer.Children())
                {
                    BFPatternSaveData patternData = (BFPatternSaveData) Port.userData;

                    if (string.IsNullOrEmpty(patternData.NodeID))
                    {
                        continue;
                    }

                    BFNode nextNode = loadedNodes[patternData.NodeID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = Port.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }

        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets/Editor/PatternSystem", "Graphs");

            CreateFolder("Assets", "PatternSystem");
            CreateFolder("Assets/PatternSystem", "Patterns");

            CreateFolder("Assets/PatternSystem/Patterns", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Patterns");
        }

        private static void GetElementsFromGraphView()
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
                    BFGroup group = (BFGroup) graphElement;

                    groups.Add(group);

                    return;
                }
            });
        }

        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static List<BFPatternSaveData> CloneNodes(List<BFPatternSaveData> nodes)
        {
            List<BFPatternSaveData> nodeList = new List<BFPatternSaveData>();

            foreach (BFPatternSaveData node in nodeList)
            {
                BFPatternSaveData patternData = new BFPatternSaveData()
                {
                    Text = node.Text,
                    NodeID = node.NodeID
                };

                nodes.Add(patternData);
            }

            return nodeList;
        }
    }
}