using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.Data.Save
{
    public class BFGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<BFGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<BFNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new List<BFGroupSaveData>();
            Nodes = new List<BFNodeSaveData>();
        }
    }
}