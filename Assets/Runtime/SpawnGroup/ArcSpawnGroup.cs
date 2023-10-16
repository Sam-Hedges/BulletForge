using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletForge
{
    [System.Serializable]
    public class ArcSpawnGroup : SpawnGroup
    {
        public float arc;
        public int count;
        public float radius;

        protected override void Spawn()
        {
            for (int i = 0; i < count; i++)
            {
                float a = rotation * Mathf.Deg2Rad;
                if (count > 1)
                {
                    a = (rotation - arc / 2 + i * arc / (count - 1f)) * Mathf.Deg2Rad;
                }

                if (spawn != null)
                {
                    SpawnGroup s = SpawnSP(spawn);
                    Vector2 pos = position + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
                    s.Start(pos, a * Mathf.Rad2Deg);
                }
            }
        }
    }
}
