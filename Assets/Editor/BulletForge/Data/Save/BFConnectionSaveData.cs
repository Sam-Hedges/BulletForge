using System;
using UnityEngine;

namespace BulletForge.Data.Save
{
    using Elements;
    
    /// <summary>
    /// Points to the next node, used to save the connection between nodes
    /// </summary>
    [Serializable]
    public class BFConnectionSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }
        [field: SerializeField] public BFNode Text { get; set; }
    }
}