using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletForge
{
    public class CircleSpawnGroup : SpawnGroup
    {
        public int count;
        public float radius;

        protected override void Spawn()
        {
            for (int i = 0; i < count; i++)
            {
                float a = rotation * Mathf.Deg2Rad;
                if (count > 1)
                {
                    a = (rotation - 180 + i * 360.0f / count) * Mathf.Deg2Rad;
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
