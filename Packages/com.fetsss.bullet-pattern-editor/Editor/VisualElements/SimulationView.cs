using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace BB
{
    public class SpawnGroupSimulationView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SpawnGroupSimulationView, VisualElement.UxmlTraits> { }

        public Pattern currentPatern;

        private Color lineColor = new Color(0.8f, 0.8f, 0.8f, 1);

        private float unit = 50;
        private float minZoom = 0.5f;
        private float maxZoom = 5f;

        private Vector2 offset = Vector2.zero;
        private float zoom = 1;

        private bool dragging = false;

        private bool isPause = true;
        private float currentTime;

        public SpawnGroupSimulationView()
        {
            generateVisualContent += GenerateVisual;
            style.overflow = Overflow.Hidden;

            RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 2)
                {
                    dragging = true;
                }
            });

            RegisterCallback<MouseUpEvent>(evt =>
            {
                if (evt.button == 2)
                {
                    dragging = false;
                }
            });

            RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (dragging)
                {
                    offset += evt.mouseDelta;
                }
            });

            RegisterCallback<WheelEvent>(evt =>
            {
                zoom = Mathf.Clamp(zoom * Mathf.Pow(1.1f, evt.delta.normalized.y), minZoom, maxZoom);
            });

            currentTime = (float)EditorApplication.timeSinceStartup;
        }

        public void SetPatern(Pattern patern)
        {
            currentPatern = patern;
            currentPatern?.Start();
        }

        public void OnGUI()
        {
            float delta = (float)EditorApplication.timeSinceStartup - currentTime;
            if (delta > 0.03f)
            {
                currentTime = (float)EditorApplication.timeSinceStartup;
                if (!isPause)
                {
                    currentPatern.UpdatePaternInSimulation(delta);
                }
            }
            MarkDirtyRepaint();
        }

        public void Start()
        {
            currentTime = (float)EditorApplication.timeSinceStartup;
            isPause = false;
        }

        public void Pause()
        {
            isPause = true;
        }

        public void Reset()
        {
            currentPatern?.ResetPatern();
            isPause = true;
        }

        void GenerateVisual(MeshGenerationContext mgc)
        {

            Vector2 size = new Vector2(resolvedStyle.width, resolvedStyle.height);
            Vector2 center = transform.position + new Vector3(size.x, size.y) / 2;

            Vector2Int min = new Vector2Int(Mathf.CeilToInt((-offset.x - size.x / 2) / unit * zoom), Mathf.CeilToInt((-offset.y - size.y / 2) / unit * zoom));
            Vector2Int max = new Vector2Int(Mathf.FloorToInt((-offset.x + size.x / 2) / unit * zoom), Mathf.FloorToInt((-offset.y + size.y / 2) / unit * zoom));

            for (int x = min.x; x <= max.x; x++)
            {
                float thickness = 0.5f / zoom;
                if (x == 0) thickness *= 3;
                DrawLine(new Vector2(x * unit / zoom + center.x + offset.x, center.y - size.y / 2f),
                new Vector2(x * unit / zoom + center.x + offset.x, center.y + size.y / 2f),
                thickness,
                 lineColor,
                 mgc);
            }
            for (int y = min.y; y <= max.y; y++)
            {
                float thickness = 0.5f / zoom;
                if (y == 0) thickness *= 3;
                DrawLine(new Vector2(center.x - size.x / 2f, y * unit / zoom + center.y + offset.y),
                new Vector2(center.x + size.x / 2f, y * unit / zoom + center.y + offset.y),
                thickness,
                 lineColor,
                 mgc);
            }

            DrawPlayer(mgc, center + offset, zoom / unit);

            currentPatern?.DrawSimulation(mgc, center + offset, zoom / unit);
        }

        void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, MeshGenerationContext mgc)
        {

            List<Vertex> vertices = new List<Vertex>();
            List<ushort> indices = new List<ushort>() { 0, 1, 2, 3, 4, 5 };


            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x);
            float offsetX = thickness / 2 * Mathf.Sin(angle);
            float offsetY = thickness / 2 * Mathf.Cos(angle);

            vertices.Add(new Vertex()
            {
                position = new Vector3(start.x + offsetX, start.y - offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(end.x + offsetX, end.y - offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(end.x - offsetX, end.y + offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(end.x - offsetX, end.y + offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(start.x - offsetX, start.y + offsetY, Vertex.nearZ),
                tint = color
            });
            vertices.Add(new Vertex()
            {
                position = new Vector3(start.x + offsetX, start.y - offsetY, Vertex.nearZ),
                tint = color
            });


            var mesh = mgc.Allocate(vertices.Count, indices.Count);
            mesh.SetAllVertices(vertices.ToArray());
            mesh.SetAllIndices(indices.ToArray());
        }

        public void DrawPlayer(MeshGenerationContext mgc, Vector3 offset, float zoom)
        {

            Vector3 pos = new Vector2(0, 3);

            List<Vertex> vertices = new()
            {
                new Vertex()
                {
                    position = (pos + Vector3.up * 0.3f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = Color.red,
                },
                new Vertex()
                {
                    position = (pos + Vector3.left * 0.3f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = Color.red,
                },
                new Vertex()
                {
                    position = (pos + Vector3.down * 0.3f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = Color.red,
                },
                new Vertex()
                {
                    position = (pos + Vector3.right * 0.3f) / zoom + offset + new Vector3(0, 0, Vertex.nearZ),
                    tint = Color.red,
                },
            };

            List<ushort> indices = new List<ushort>() { 0, 1, 3, 3, 1, 2 };

            var mesh = mgc.Allocate(vertices.Count, indices.Count);
            mesh.SetAllVertices(vertices.ToArray());
            mesh.SetAllIndices(indices.ToArray());
        }
    }
}
