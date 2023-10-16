using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace BB
{
    public class SpawnGroupGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<SpawnGroupGraphView, GraphView.UxmlTraits> { }

        public Pattern currentPatern;

        private string windowUss = "3160aa02438d3964980fedf74ef3c941";

        public SpawnGroupGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            serializeGraphElements += CutCopyOperation;
            unserializeAndPaste += PasteOperation;

            var ussPath = AssetDatabase.GUIDToAssetPath(windowUss);
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);

            styleSheets.Add(styleSheet);

            this.AddElement(GenerateRootNode());
        }

        private Port GeneratePort(SpawnGroupNode node, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(int));
        }

        private SpawnGroupNode GenerateRootNode()
        {
            var node = new SpawnGroupNode
            {
                title = "Root",
                GUID = Guid.NewGuid().ToString(),
                entryPoint = true,
                spawnGroup = new SpawnGroup(),
            };

            node.spawnGroup.SetPatern(currentPatern);

            Port generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Spawn";
            node.outputContainer.Add(generatedPort);

            node.capabilities -= Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 100, 100, 150));

            return node;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            Vector2 localMousePos = evt.localMousePosition;
            Vector2 actualGraphPosition = viewTransform.matrix.inverse.MultiplyPoint(localMousePos);
            var types = TypeCache.GetTypesDerivedFrom<SpawnGroup>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"{type.Name}", (a) =>
                {
                    SpawnGroup sp = (SpawnGroup)Activator.CreateInstance(type);
                    sp.SetPatern(currentPatern);
                    CreateNode(sp, actualGraphPosition);
                });
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != portView.direction)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private SpawnGroupNode CreateNode(SpawnGroup spawnGroup, Vector2 position)
        {
            Type type = spawnGroup.GetType();

            var node = new SpawnGroupNode
            {
                title = type.Name,
                GUID = Guid.NewGuid().ToString(),
                entryPoint = false,
                spawnGroup = spawnGroup,
            };

            VisualElement container = node.ExtensionContainer();
            node.extensionContainer.Add(container);

            if (type.IsSubclassOf(typeof(MultiSpawnGroup)) || type == typeof(MultiSpawnGroup))
            {
                int v = ((MultiSpawnGroup)node.spawnGroup).spawns.Count;
                for (int i = 0; i < v; i++)
                {
                    Port outputPort = GeneratePort(node, Direction.Output);
                    outputPort.portName = "Spawn";
                    node.outputContainer.Add(outputPort);
                }

                IntegerField spawnCount = new IntegerField("Spawns Count")
                {
                    value = v
                };

                spawnCount.RegisterCallback<ChangeEvent<int>>(evt =>
                {
                    int value = 1;
                    if (evt.newValue >= 1) value = evt.newValue;
                    ((MultiSpawnGroup)node.spawnGroup).spawns = new List<SpawnGroup>(new SpawnGroup[value]);
                    var ports = node.outputContainer.Query<Port>().ToList();
                    if (value > ports.Count)
                    {
                        for (int i = 0; i < value - ports.Count; i++)
                        {
                            Port outputPort = GeneratePort(node, Direction.Output);
                            outputPort.portName = "Spawn";
                            node.outputContainer.Add(outputPort);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ports.Count - value; i++)
                        {
                            Port outputPort = ports[ports.Count - i - 1];
                            DeleteElements(outputPort.connections);
                            if (node.outputContainer.Contains(outputPort))
                            {
                                node.outputContainer.Remove(outputPort);
                            }
                        }
                    }
                });

                container.Insert(0, spawnCount);
            }
            else if (type.IsSubclassOf(typeof(BulletSpawnGroup)) || type == typeof(BulletSpawnGroup))
            {
                //no output port
            }
            else
            {
                Port outputPort = GeneratePort(node, Direction.Output);
                outputPort.portName = "Spawn";
                node.outputContainer.Add(outputPort);
            }
            Port inputPort = GeneratePort(node, Direction.Input);
            inputPort.portName = "";
            node.inputContainer.Add(inputPort);

            // this ensure that the node is always collapsible
            Port hiddenPort = GeneratePort(node, Direction.Input);
            hiddenPort.style.display = DisplayStyle.None;
            node.inputContainer.Add(hiddenPort);

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(position.x, position.y, 1000, 1000));

            this.AddElement(node);

            return node;
        }

        private string CutCopyOperation(IEnumerable<GraphElement> elements)
        {
            List<SpawnGroupNode.SerializableNode> nodes = new List<SpawnGroupNode.SerializableNode>();

            foreach (GraphElement ge in elements)
            {
                if (ge is SpawnGroupNode)
                {
                    if (!(ge as SpawnGroupNode).entryPoint)
                    {
                        nodes.Add((ge as SpawnGroupNode).GetSerializableNode());
                    }
                }
            }

            string data = JsonUtility.ToJson(new JsonableListWrapper<SpawnGroupNode.SerializableNode>(nodes));
            return data;
        }

        private void PasteOperation(string operationName, string data)
        {
            try
            {
                var nodes = JsonUtility.FromJson<JsonableListWrapper<SpawnGroupNode.SerializableNode>>(data).list;

                ClearSelection();

                foreach (var node in nodes)
                {
                    SpawnGroup sp = SpawnGroup.ShallowClone(node.spawnGroup);
                    sp.SetPatern(currentPatern);
                    SpawnGroupNode n = CreateNode(sp, node.position + new Vector2(50, 50));
                    AddToSelection(n);
                }
            }
            catch
            {
            }
        }

        public (SpawnGroup, List<SpawnGroup>) SaveGraph()
        {
            List<SpawnGroup> spawnGroups = new List<SpawnGroup>();

            SpawnGroup GetSpawnGroup(SpawnGroupNode spn)
            {
                if (spn == null) return null;
                var outputPorts = spn.outputContainer.Query<Port>().ToList();
                SpawnGroup sp = SpawnGroup.ShallowClone(spn.spawnGroup);

                if (sp is MultiSpawnGroup)
                {
                    MultiSpawnGroup msp = sp as MultiSpawnGroup;
                    msp.spawns = new List<SpawnGroup>(new SpawnGroup[outputPorts.Count]);
                    for (int i = 0; i < outputPorts.Count; i++)
                    {
                        if (outputPorts[i].connected)
                        {
                            var c = outputPorts[i].connections.First();
                            if (c.input.node != spn)
                            {
                                msp.spawns[i] = GetSpawnGroup(c.input.node as SpawnGroupNode);
                            }
                            else if (c.output.node != spn)
                            {
                                msp.spawns[i] = GetSpawnGroup(c.output.node as SpawnGroupNode);
                            }
                        }
                    }
                }
                else
                {
                    if (outputPorts.Count > 0 && outputPorts[0].connected)
                    {
                        var c = outputPorts[0].connections.First();
                        if (c.input.node != spn)
                        {
                            sp.spawn = GetSpawnGroup(c.input.node as SpawnGroupNode);
                        }
                        else if (c.output.node != spn)
                        {
                            sp.spawn = GetSpawnGroup(c.output.node as SpawnGroupNode);
                        }
                    }
                }

                spawnGroups.Add(sp);
                return sp;
            }

            var allNodes = nodes;

            Node root = nodes.First(x => x is SpawnGroupNode node && node.entryPoint);

            return (GetSpawnGroup(root as SpawnGroupNode), spawnGroups);
        }

        public void LoadGraph(Pattern patern)
        {

            Vector2 offset = new Vector2(400, 300);
            int currentyOffset = 0;

            SpawnGroupNode SpawnSpawnGroup(SpawnGroup sp, Vector2 pos)
            {
                if (sp == null) return null;

                SpawnGroupNode spn = CreateNode(sp, pos);

                if (sp is MultiSpawnGroup)
                {
                    MultiSpawnGroup msp = sp as MultiSpawnGroup;
                    List<Port> outputPorts = spn.outputContainer.Query<Port>().ToList();
                    for (int i = 0; i < Mathf.Min(msp.spawns.Count, outputPorts.Count); i++)
                    {
                        SpawnGroupNode n = SpawnSpawnGroup(msp.spawns[i], pos + new Vector2(offset.x, offset.y * currentyOffset));
                        currentyOffset += 1;
                        if (n != null)
                        {
                            Port p = n.inputContainer.Query<Port>();
                            Edge e = outputPorts[i].ConnectTo(p);
                            this.AddElement(e);
                        }
                    }
                }
                else
                {
                    List<Port> outputPorts = spn.outputContainer.Query<Port>().ToList();
                    if (outputPorts.Count > 0)
                    {
                        SpawnGroupNode n = SpawnSpawnGroup(sp.spawn, pos + new Vector2(offset.x, 0));
                        if (n != null)
                        {
                            Port p = n.inputContainer.Query<Port>();
                            Edge e = outputPorts[0].ConnectTo(p);
                            this.AddElement(e);
                        }
                    }
                }

                return spn;
            }

            currentPatern = patern;
            if (patern.root == null)
            {
                patern.root = new SpawnGroup();
            }
            patern.root.SetPatern(patern);

            SpawnGroupNode root = nodes.First(x => x is SpawnGroupNode node && node.entryPoint) as SpawnGroupNode;
            root.spawnGroup = patern.root;
            SpawnGroupNode node = SpawnSpawnGroup(patern.root.spawn, new Vector2(100, 100) + new Vector2(150, 0));
            if (node != null)
            {
                Edge e = root.outputContainer.Q<Port>().ConnectTo(node.inputContainer.Q<Port>());
                this.AddElement(e);
            }
        }
    }
}