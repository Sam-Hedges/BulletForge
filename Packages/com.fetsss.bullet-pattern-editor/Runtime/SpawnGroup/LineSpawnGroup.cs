using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    public class LineSpawnGroup : SpawnGroup
    {
        public int count;
        public float minLength;
        public float maxLength;

        protected override void Spawn()
        {
            for (int i = 0; i < count; i++)
            {
                float a = minLength;
                if (count > 1)
                {
                    a = Mathf.Lerp(minLength, maxLength, (float)i / (count - 1f));
                }

                if (spawn != null)
                {
                    SpawnGroup s = SpawnSP(spawn);
                    Vector2 pos = position + new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad)) * a;
                    s.Start(pos, rotation);
                }
            }
        }
    }
}
