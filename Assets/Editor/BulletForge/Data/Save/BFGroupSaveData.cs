using System;
using UnityEngine;

namespace BulletForge.Data.Save
{
    [Serializable]
    public class BFGroupSaveData
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}