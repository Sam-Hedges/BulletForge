using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

namespace BB
{
    public class SpawnGroupNode : Node
    {
        public string GUID;

        public SpawnGroup spawnGroup;

        public bool entryPoint = false;

        [System.Serializable]
        public struct SerializableNode
        {
            [SerializeReference]
            public SpawnGroup spawnGroup;
            public Vector2 position;

            public SerializableNode(SpawnGroup spawnGroup, Vector2 position)
            {
                this.spawnGroup = spawnGroup;
                this.position = position;
            }
        }

        public SerializableNode GetSerializableNode()
        {
            return new SerializableNode(spawnGroup, GetPosition().min);
        }


        public VisualElement ExtensionContainer()
        {
            Type type = spawnGroup.GetType();

            var container = new VisualElement();
            container.AddToClassList("container");

            var fields = type.GetFields().ToList();
            var inheritedFields = type.BaseType.GetFields().ToList();

            List<string> filter = new List<string>() { "executeImmediate", "delay", "repetition", "interval" };
            fields.RemoveAll(y => inheritedFields.Any(x => x.Name == y.Name) || !y.IsPublic);
            inheritedFields.RemoveAll(y => filter.Any(x => x == y.Name) || !y.IsPublic);

            foreach (var field in fields)
            {
                VisualElement v = CreateDataField(field, this);
                if (v != null)
                {
                    container.Add(v);
                }
            }
            if (fields.Count > 0)
            {
                var separator = new VisualElement();
                separator.AddToClassList("separator");
                container.Add(separator);
            }
            foreach (var field in inheritedFields)
            {
                VisualElement v = CreateDataField(field, this);
                if (v != null)
                {
                    container.Add(v);
                }
            }

            if (spawnGroup is not BulletSpawnGroup)
            {
                if (fields.Count > 0)
                {
                    var separator = new VisualElement();
                    separator.AddToClassList("separator");
                    container.Add(separator);
                }

                VisualElement immediate = new VisualElement();
                Toggle toggle = new Toggle("Execute Immediate") { value = spawnGroup.executeImmediate };
                toggle.RegisterValueChangedCallback(evt =>
                {
                    spawnGroup.executeImmediate = evt.newValue;
                    if (evt.newValue)
                    {
                        immediate.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        immediate.style.display = DisplayStyle.Flex;
                    }
                });
                FloatField delay = new FloatField("Delay") { value = spawnGroup.delay };
                delay.RegisterValueChangedCallback(evt => { spawnGroup.delay = evt.newValue; });
                IntegerField repetition = new IntegerField("Repetition") { value = spawnGroup.repetition };
                repetition.RegisterValueChangedCallback(evt => { spawnGroup.repetition = evt.newValue; });
                FloatField interval = new FloatField("Interval") { value = spawnGroup.interval };
                interval.RegisterValueChangedCallback(evt => { spawnGroup.interval = evt.newValue; });
                immediate.Add(delay);
                immediate.Add(repetition);
                immediate.Add(interval);

                if (toggle.value)
                {
                    immediate.style.display = DisplayStyle.None;
                }
                else
                {
                    immediate.style.display = DisplayStyle.Flex;
                }

                container.Add(toggle);
                container.Add(immediate);
            }

            return container;
        }

        private VisualElement CreateDataField(System.Reflection.FieldInfo field, SpawnGroupNode node)
        {
            if (field.FieldType == typeof(Vector2))
            {
                Vector2Field v2 = new Vector2Field(field.Name)
                {
                    value = (Vector2)field.GetValue(node.spawnGroup)
                };
                v2.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return v2;
            }
            else if (field.FieldType == typeof(float))
            {
                FloatField f = new FloatField(field.Name)
                {
                    value = (float)field.GetValue(node.spawnGroup)
                };
                f.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return f;
            }
            else if (field.FieldType == typeof(int))
            {
                IntegerField i = new IntegerField(field.Name)
                {
                    value = (int)field.GetValue(node.spawnGroup)
                };
                i.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return i;
            }
            else if (field.FieldType == typeof(bool))
            {
                Toggle t = new Toggle(field.Name)
                {
                    value = (bool)field.GetValue(node.spawnGroup)
                };
                t.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return t;
            }
            else if (field.FieldType == typeof(SimulationSpace))
            {
                EnumField e = new EnumField(field.Name, (SimulationSpace)field.GetValue(node.spawnGroup));
                e.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return e;
            }
            else if (field.FieldType == typeof(GameObject) || field.FieldType.IsSubclassOf(typeof(GameObject)))
            {
                ObjectField o = new ObjectField(field.Name);
                o.objectType = typeof(GameObject);
                o.value = (GameObject)field.GetValue(node.spawnGroup);
                o.allowSceneObjects = false;
                o.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return o;
            }
            else if (field.FieldType == typeof(Color))
            {
                ColorField c = new ColorField(field.Name);
                c.value = (Color)field.GetValue(node.spawnGroup);
                c.RegisterValueChangedCallback(evt =>
                {
                    field.SetValue(node.spawnGroup, evt.newValue);
                });
                return c;
            }
            else
            {
                return null;
            }
        }
    }

    [System.Serializable]
    public class JsonableListWrapper<T>
    {
        public List<T> list;
        public JsonableListWrapper(List<T> list) => this.list = list;
    }
}
