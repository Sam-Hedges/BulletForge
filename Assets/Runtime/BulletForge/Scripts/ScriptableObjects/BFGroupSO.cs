using UnityEngine;

namespace BulletForge.ScriptableObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class BFGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupID { get; set; }

        public void Initialize(string id)
        {
            GroupID = id;
        }
    }
}