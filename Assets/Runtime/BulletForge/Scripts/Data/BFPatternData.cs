using System;
using UnityEngine;

namespace BulletForge.Data
{
    using ScriptableObjects;

    [Serializable]
    public class BFPatternData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public BFPatternSO NextPattern { get; set; }
    }
}