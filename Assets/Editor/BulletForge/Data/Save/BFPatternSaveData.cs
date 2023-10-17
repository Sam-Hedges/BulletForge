using System;
using UnityEngine;

namespace BulletForge.Data.Save
{
    [Serializable]
    public class BFPatternSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeID { get; set; }
    }
}