using System;
using UnityEngine;

namespace BulletForge.Data
{
    using ScriptableObjects;
    
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BFConnectionData
    {
        [field: SerializeField] public string NodeID { get; set; }
        [field: SerializeField] public BFNodeSO NextNode { get; set; }
    }
}