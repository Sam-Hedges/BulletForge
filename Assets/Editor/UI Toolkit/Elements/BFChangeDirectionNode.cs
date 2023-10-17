using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    
    public class BFChangeDirectionNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.ChangeDirection;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            Port directionPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            directionPort.portName = "Input";
            inputContainer.Add(directionPort);
            
            Port speedPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            speedPort.portName = "Input";
            inputContainer.Add(speedPort);
            
            // No Output Container
        }
    }
}
