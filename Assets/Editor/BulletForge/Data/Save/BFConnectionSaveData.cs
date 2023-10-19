using System;
using UnityEngine;

namespace BulletForge.Data.Save
{
    using Elements;
    
    /// <summary>
    /// A data class that points to the next node, used to save the connection between nodes
    /// </summary>
    [Serializable]
    public class BFConnectionSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }
    }
}