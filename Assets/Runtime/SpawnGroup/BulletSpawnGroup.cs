using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BulletForge
{
    [System.Serializable]
    public class BulletSpawnGroup : SpawnGroup
    {
        [SerializeReference]
        public GameObject bulletPrefab;

        public float bulletSpeed;

        private IBullet spawnedBullet;

        public BulletSpawnGroup()
        {
            executeImmediate = true;
        }

        protected override bool ShouldBeDestroyed()
        {
            if (!isSimulation)
            {
                return repetitionCount >= repetition && children.Count <= 0 && spawnedBullet == null;
            }
            else
            {
                return repetitionCount >= repetition && children.Count <= 0 && timer >= 5;
            }
        }

        protected override void UpdatePosition(float deltaTime)
        {
            spawnedBullet?.UpdateLocalPosition(position - prePosition, rotation - preRotation);
        }

        protected override void Spawn()
        {
            if (bulletPrefab != null && !isSimulation)
            {
                spawnedBullet = bulletPrefab.GetComponent<IBullet>()?.SpawnBullet(this);
                if (spawnedBullet == null)
                {
                    Debug.LogWarning(string.Format("Bullet prefab {0} in Patern {1} does not have a IBullet component", bulletPrefab.name, patern.name));
                    return;
                }
                spawnedBullet.SetPosition(position);
                spawnedBullet.SetRotation(rotation);
                spawnedBullet.SetSpeed(bulletSpeed);
            }
        }

        public void RemoveBullet()
        {
            spawnedBullet = null;
        }

        public override void DrawSimulation(MeshGenerationContext mgc, Vector3 offset, float zoom)
        {
            base.DrawSimulation(mgc, offset, zoom);

            Vector3 forward = new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), -Mathf.Sin(rotation * Mathf.Deg2Rad));
            Vector3 side = Vector2.Perpendicular(forward);
            Vector3 pos = new Vector2(position.x, -position.y) + new Vector2(forward.x, forward.y) * bulletSpeed * timer;

            List<Vertex> vertices = new()
            {
                new Vertex()
                {
                    position = (pos + forward * 0.2f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + side * 0.1f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + -forward * 0.1f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + -side * 0.1f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
            };

            List<ushort> indices = new List<ushort>() { 0, 1, 3, 3, 1, 2 };

            var mesh = mgc.Allocate(vertices.Count, indices.Count);
            mesh.SetAllVertices(vertices.ToArray());
            mesh.SetAllIndices(indices.ToArray());
        }
    }
}
