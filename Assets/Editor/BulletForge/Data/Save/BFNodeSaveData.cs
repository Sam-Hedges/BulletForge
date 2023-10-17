using System;
using System.Collections.Generic;
using UnityEngine;

namespace BulletForge.Data.Save
{
    using Enumerations;

    [Serializable]
    public class BFNodeSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<BFPatternSaveData> Pattern { get; set; }
        [field: SerializeField] public string GroupID { get; set; }
        [field: SerializeField] public ENodeType NodeType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}