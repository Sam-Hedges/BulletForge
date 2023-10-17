using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    public class BFNode : Node
    {
        public ENodeName NodeName { get; set; }
        
        public ENodeType NodeType { get; set; }
        
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
                value = NodeName.ToString()
            };

            titleContainer.Insert(0, nodeNameTextElement);
            
            // Input Container
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Input";
            
            inputContainer.Add(inputPort);
        }
    }
}
