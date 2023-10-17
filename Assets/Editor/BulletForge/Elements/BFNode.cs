using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    
    public class BFNode : Node
    {
        public string ID { get; set; }
        public List<BFPatternSaveData> Pattern { get; set; }
        public string Text { get; set; }
        public BFGroup Group { get; set; }

        protected BFGraphView graphView;
        private Color defaultBackgroundColor;
        
        public ENodeType NodeType { get; set; }
        
        public EDirectionType DirectionType { get; set; }
        
        public EPatternType PatternType { get; set; }
        
        public ERunStatus RunStatus { get; set; }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }
        
        public virtual void Initialize(string nodeName, BFGraphView bfGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            
            Pattern = new List<BFPatternSaveData>();
            Text = "Node text.";

            SetPosition(new Rect(position, Vector2.zero));

            graphView = bfGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("bf-node__main-container");
            extensionContainer.AddToClassList("bf-node__extension-container");
        }

        public virtual void Draw()
        {
            // Title Container
            TextElement nodeNameTextElement = new TextElement()
            {
                text = NodeType.ToString()
            };
            
            nodeNameTextElement.AddToClassList("bf-node__textfield");
            nodeNameTextElement.AddToClassList("bf-node__filename-textfield");
            nodeNameTextElement.AddToClassList("bf-node__textfield__hidden");

            titleContainer.Insert(0, nodeNameTextElement);
        }

        public void CreateIOPort(string portName, VisualElement container, Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            Port directionPort = InstantiatePort(orientation, direction, capacity, type);
            directionPort.portName = portName;
            container.Add(directionPort);
        }
        
        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
    }
}
