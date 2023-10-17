using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    public class BFNode : Node
    {
        public ENodeType NodeType { get; set; }
        
        public EDirectionType DirectionType { get; set; }
        
        public EPatternType PatternType { get; set; }
        
        public ERunStatus RunStatus { get; set; }

        public virtual void Initialize(Vector2 position)
        {
            SetPosition(new Rect(position, Vector2.zero));
        }

        public virtual void Draw()
        {
            // Title Container
            TextField nodeNameTextElement = new TextField()
            {
                value = NodeType.ToString()
            };

            titleContainer.Insert(0, nodeNameTextElement);
        }

        public void CreateIOPort(string portName, VisualElement container, Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            Port directionPort = InstantiatePort(orientation, direction, capacity, type);
            directionPort.portName = portName;
            container.Add(directionPort);
        }
    }
}
