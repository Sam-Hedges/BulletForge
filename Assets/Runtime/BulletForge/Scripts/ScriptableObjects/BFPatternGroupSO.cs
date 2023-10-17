using UnityEngine;

namespace BulletForge.ScriptableObjects
{
    public class BFPatternGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }
}