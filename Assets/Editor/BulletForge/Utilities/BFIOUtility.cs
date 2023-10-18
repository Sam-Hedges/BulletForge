using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BulletForge.Utilities
{
    using Data.Save;
    using Windows;
    
    /// <summary>
    /// 
    /// </summary>
    public static class BFIOUtility
    {
        private static BFSaveUtility saveUtility;
        private static BFLoadUtility loadUtility;
        
        public static void Initialize(BFGraphView bfGraphView, string graphName)
        {
            saveUtility = new BFSaveUtility(bfGraphView, graphName, $"Assets/Runtime/BulletForge/Patterns/{graphName}");
            loadUtility = new BFLoadUtility(bfGraphView, graphName);
        }
        
        /// <summary>
        /// 
        /// </summary>
        private static void Save()
        {
            saveUtility.Save();
        }
        
        /// <summary>
        /// 
        /// </summary>
        private static void Load()
        {
            loadUtility.Load();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<BFNodeSaveData> CloneNodes(List<BFNodeSaveData> nodes)
        {
            List<BFNodeSaveData> nodeList = new List<BFNodeSaveData>();

            foreach (BFNodeSaveData node in nodeList)
            {
                BFNodeSaveData patternData = new BFNodeSaveData()
                {
                    ID = node.ID
                };

                nodes.Add(patternData);
            }

            return nodeList;
        }
    }
}