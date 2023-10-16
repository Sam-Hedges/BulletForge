using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace BB
{
    public enum SimulationSpace
    {
        Local,
        Global,
    }

    [System.Serializable]
    public class SpawnGroup
    {
        private Guid ID = Guid.NewGuid();

        [SerializeReference]
        public SpawnGroup spawn;

        public SimulationSpace simulationSpace = SimulationSpace.Global;

        public Vector2 positionOffset;
        public float rotationOffset;

        public bool executeImmediate = false;
        public float delay;
        public int repetition = 1;
        public float interval;

        [HideInInspector]
        protected Pattern patern;

        [SerializeReference]
        public SpawnGroup parent;

        protected float timer;

        [System.NonSerialized]
        protected List<SpawnGroup> children;

        protected Vector2 prePosition;
        public Vector2 position { get; protected set; }
        protected float preRotation;
        protected float rotation;

        protected int repetitionCount;

        public Color simulationColor = Color.white;
        protected bool isSimulation = false;

        public void Start(Vector2 pos, float rot)
        {
            Vector2 r = new Vector2(Mathf.Cos(rot * Mathf.Deg2Rad), -Mathf.Sin(rot * Mathf.Deg2Rad));
            position = pos + new Vector2(positionOffset.x * r.x + positionOffset.y * r.y, -positionOffset.x * r.y + positionOffset.y * r.x);
            //position = pos + positionOffset;
            rotation = rot + rotationOffset;
            prePosition = position;
            preRotation = rotation;
            children = new List<SpawnGroup>();
            timer = 0;
        }

        public void Update(float deltaTime)
        {
            timer += deltaTime;

            UpdatePosition(deltaTime);

            for (int i = children.Count - 1; i >= 0; i--)
            {
                children[i].UpdateLocalPosition(position - prePosition, rotation - preRotation);
                children[i].Update(deltaTime);
            }

            if ((timer >= repetitionCount * interval + delay || executeImmediate) && repetitionCount < repetition)
            {
                Spawn();
                repetitionCount += 1;

                if (executeImmediate)
                {
                    repetitionCount = repetition + 1;
                }
            }

            if (ShouldBeDestroyed())
            {
                if (parent == null)
                {
                    patern.RemoveSpawnGroup(this);
                }
                else
                {
                    parent.RemoveSP(this);
                }
            }

            prePosition = position;
            preRotation = rotation;
        }

        /// <summary>
        /// Called everytime the patterns updates.
        /// You can change the position or anything in here.
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void UpdatePosition(float deltaTime)
        {

        }

        /// <summary>
        /// Returns whether the SpawnGroup should be destroyed yet.
        /// Can be used to tie the lifetime of the SpawnGroup to another object like a bullet.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldBeDestroyed()
        {
            return repetitionCount >= repetition && children.Count <= 0;
        }

        public void UpdateLocalPosition(Vector2 deltaPos, float deltaRot)
        {
            position += deltaPos;
            rotation += deltaRot;
            if (parent != null)
            {
                Vector2 d = position - parent.position;
                Vector2 rot = new Vector2(Mathf.Cos(deltaRot * Mathf.Deg2Rad), -Mathf.Sin(deltaRot * Mathf.Deg2Rad));
                position = parent.position + new Vector2(d.x * rot.x + d.y * rot.y, -d.x * rot.y + d.y * rot.x);
            }
        }

        /// <summary>
        /// Called when the SpapwnGroup supposed to spawn the child SpawnGroup.
        /// Spawns the child SpawnGroup in a way that is specified.
        /// </summary>
        protected virtual void Spawn()
        {
            if (spawn != null)
            {
                SpawnGroup s = SpawnSP(spawn);
                s.Start(position, rotation);
            }
        }

        protected SpawnGroup SpawnSP(SpawnGroup s)
        {
            if (s.simulationSpace == SimulationSpace.Local)
            {
                SpawnGroup sp = ShallowClone(s);
                sp.patern = patern;
                sp.parent = this;
                children.Add(sp);
                patern.AddSpawnGroup(sp);
                return sp;
            }
            else if (s.simulationSpace == SimulationSpace.Global)
            {
                SpawnGroup sp = ShallowClone(s);
                sp.patern = patern;
                sp.parent = null;
                patern.AddSpawnGroup(sp);
                return sp;
            }
            else
            {
                return null;
            }
        }

        public void RemoveSP(SpawnGroup s)
        {
            if (children.Contains(s))
            {
                children.Remove(s);
                patern.RemoveSpawnGroup(s);
            }
        }

        public void SetPatern(Pattern patern)
        {
            this.patern = patern;
        }

        public static SpawnGroup ShallowClone(SpawnGroup spawnGroup)
        {
            return (SpawnGroup)spawnGroup.MemberwiseClone();
        }

        public void SetAsSimulation()
        {
            isSimulation = true;
        }

        /// <summary>
        /// Draw the SpawnGroup on the Simulation View.
        /// You shouldn't touch this if you don't know what you are doing.
        /// </summary>
        /// <param name="mgc">MeshGenerationContext of the simulationView</param>
        /// <param name="offset">offset from the center of the origin</param>
        /// <param name="zoom">divide by this</param>
        public virtual void DrawSimulation(MeshGenerationContext mgc, Vector3 offset, float zoom)
        {
            Vector3 forward = new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), -Mathf.Sin(rotation * Mathf.Deg2Rad));
            Vector3 side = Vector2.Perpendicular(forward);
            Vector3 pos = new Vector3(position.x, -position.y);

            List<Vertex> vertices = new()
            {
                new Vertex()
                {
                    position = (pos + forward * 0.2f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + forward * 0.16f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + side * 0.1f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + side * 0.08f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + -forward * 0.1f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + -forward * 0.08f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + -side * 0.1f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                },
                new Vertex()
                {
                    position = (pos + -side * 0.08f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = simulationColor,
                }
            };

            List<ushort> indices = new List<ushort>() { 2, 1, 0, 3, 1, 2, 4, 3, 2, 5, 3, 4, 6, 5, 4, 7, 5, 6, 0, 7, 6, 1, 7, 0 };

            var mesh = mgc.Allocate(vertices.Count, indices.Count);
            mesh.SetAllVertices(vertices.ToArray());
            mesh.SetAllIndices(indices.ToArray());

            /*
            for (int i = 0; i < children.Count; i++)
            {
                children[i].DrawSimulation(mgc, offset, zoom);
            }
            */
        }
    }
}
