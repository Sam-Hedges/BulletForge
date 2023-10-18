using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.ScriptableObjects
{
    /// <summary>
    /// The main scriptable object that contains all the nodes and groups
    /// </summary>
    public class BFPatternContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SerializableDictionary<BFGroupSO, List<BFNodeSO>> Groups { get; set; }
        [field: SerializeField] public List<BFNodeSO> UngroupedNodes { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new SerializableDictionary<BFGroupSO, List<BFNodeSO>>();
            UngroupedNodes = new List<BFNodeSO>();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetGroupIDs()
        {
            List<string> groupIDs = new List<string>();

            foreach (BFGroupSO group in Groups.Keys)
            {
                groupIDs.Add(group.GroupID);
            }

            return groupIDs;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="startingNodesOnly"></param>
        /// <returns></returns>
        public List<string> GetGroupedNodeIDs(BFGroupSO group, bool startingNodesOnly)
        {
            List<BFNodeSO> groupedNodes = Groups[group];

            List<string> groupedNodeIDs = new List<string>();

            foreach (BFNodeSO node in groupedNodes)
            {
                if (startingNodesOnly && !node.IsStartingNode)
                {
                    continue;
                }

                groupedNodeIDs.Add(node.ID);
            }

            return groupedNodeIDs;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingNodesOnly"></param>
        /// <returns></returns>
        public List<string> GetUngroupedNodeIDs(bool startingNodesOnly)
        {
            List<string> ungroupedNodeIDs = new List<string>();

            foreach (BFNodeSO node in UngroupedNodes)
            {
                if (startingNodesOnly && !node.IsStartingNode)
                {
                    continue;
                }

                ungroupedNodeIDs.Add(node.ID);
            }

            return ungroupedNodeIDs;
        }
    }
}