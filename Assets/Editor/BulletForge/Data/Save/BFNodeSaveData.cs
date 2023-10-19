using System;
using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.Data.Save
{
    using Enumerations;
    
    /// <summary>
    /// A data class used to save store the data of a node
    /// </summary>
    [Serializable]
    public class BFNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<BFConnectionSaveData> Connections { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public ENodeType NodeType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}