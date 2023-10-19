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
            saveUtility = new BFSaveUtility(bfGraphView, graphName);
            loadUtility = new BFLoadUtility(bfGraphView, graphName);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static void Save()
        {
            saveUtility.Save();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static void Load()
        {
            loadUtility.Load();
        }
        
        /// <summary>
        /// Loads an Asset of the specified type
        /// </summary>
        /// <param name="path">The file path of where to save the graph</param>
        /// <param name="assetName">The name of the file once saved</param>
        /// <typeparam name="T">The type of asset being loaded</typeparam>
        /// <returns></returns>
        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            // Create the full path to the asset
            string fullPath = $"{path}/{assetName}.asset";
            
            // Load the asset at the specified path
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<BFConnectionSaveData> CloneNodes(List<BFConnectionSaveData> nodes)
        {
            List<BFConnectionSaveData> nodeList = new List<BFConnectionSaveData>();

            foreach (BFConnectionSaveData node in nodeList)
            {
                BFConnectionSaveData nodeData = new BFConnectionSaveData()
                {
                    NodeID = node.NodeID
                };

                nodeList.Add(nodeData);
            }

            return nodeList;
        }
    }
}