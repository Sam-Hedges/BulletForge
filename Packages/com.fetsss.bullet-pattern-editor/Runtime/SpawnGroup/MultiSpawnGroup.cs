using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace BB
{
    [System.Serializable]
    public class MultiSpawnGroup : SpawnGroup
    {
        [SerializeReference]
        public List<SpawnGroup> spawns = new List<SpawnGroup>() { null };


        public MultiSpawnGroup()
        {
            executeImmediate = true;
        }

        /*
                public SpawnGroup GetSpawn(int index)
                {
                    return patern.GetSpawnGroup(spawns[index]);
                }
                */

        protected override void Spawn()
        {
            for (int i = 0; i < spawns.Count; i++)
            {
                if (spawns[i] != null)
                {
                    SpawnGroup s = SpawnSP(spawns[i]);
                    s.Start(position, rotation);
                }
            }
        }
    }
}