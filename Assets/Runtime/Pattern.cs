using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace BulletForge
{
    [CreateAssetMenu(fileName = "Pattern", menuName = "BulletForge/Pattern")]
    public class Pattern : ScriptableObject
    {
        [System.NonSerialized]
        private List<SpawnGroup> spawnGroups = new List<SpawnGroup>();

        [HideInInspector]
        public SpawnGroup root;

        [HideInInspector]
        public List<SpawnGroup> allSpawnGroups = new List<SpawnGroup>();

        public Vector2 position;
        private Vector2 prePosition;

        public float rotation;
        private float preRotation;

        public void Start()
        {
            ResetPatern();
        }

        public void ResetPatern()
        {
            spawnGroups.Clear();
            root.SetPatern(this);
            spawnGroups.Add(SpawnGroup.ShallowClone(root));
            spawnGroups[0].Start(Vector2.zero, 0);
        }

        public void UpdatePatern(float deltaTime)
        {
            for (int i = spawnGroups.Count - 1; i >= 0; i--)
            {
                if (spawnGroups[i].parent == null)
                {
                    spawnGroups[i].UpdateLocalPosition(position - prePosition, -rotation + preRotation);
                    spawnGroups[i].Update(deltaTime);
                }
            }

            prePosition = position;
            preRotation = rotation;
        }

        public void UpdatePaternInSimulation(float deltaTime)
        {
            for (int i = spawnGroups.Count - 1; i >= 0; i--)
            {
                spawnGroups[i].SetAsSimulation();
                if (spawnGroups[i].parent == null)
                {
                    spawnGroups[i].Update(deltaTime);
                }
            }
        }

        public bool IsDone()
        {
            return spawnGroups.Count == 0;
        }

        public void AddSpawnGroup(SpawnGroup spawnGroup)
        {
            spawnGroups.Add(spawnGroup);
        }

        public void RemoveSpawnGroup(SpawnGroup spawnGroup)
        {
            if (spawnGroups.Contains(spawnGroup))
            {
                spawnGroups.Remove(spawnGroup);
            }
        }

        public void DrawSimulation(MeshGenerationContext mgc, Vector3 offset, float zoom)
        {
            for (int i = spawnGroups.Count - 1; i >= 0; i--)
            {
                spawnGroups[i].DrawSimulation(mgc, offset, zoom);
            }
        }
    }
}
