using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.Data.Save
{
    /// <summary>
    /// The scriptable object used to save the graph
    /// </summary>
    public class BFGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<BFGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<BFNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupIDs { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeIDs { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeIDs { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new List<BFGroupSaveData>();
            Nodes = new List<BFNodeSaveData>();
        }
    }
}