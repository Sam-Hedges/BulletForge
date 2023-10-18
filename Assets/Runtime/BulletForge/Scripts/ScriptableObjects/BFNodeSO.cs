using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge.IncomingChanges;
using UnityEngine;

namespace BulletForge.ScriptableObjects
{
    using Data;
    using Enumerations;
    
    /// <summary>
    /// 
    /// </summary>
    public class BFNodeSO : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<BFConnectionData> Connections { get; set; }
        [field: SerializeField] public ENodeType NodeType { get; set; }
        [field: SerializeField] public bool IsStartingNode { get; set; }

        public void Initialize(string id, List<BFConnectionData> connections, ENodeType nodeType, bool isStartingNode)
        {
            ID = id;
            Connections = connections;
            NodeType = nodeType;
            IsStartingNode = isStartingNode;
        }
    }
}