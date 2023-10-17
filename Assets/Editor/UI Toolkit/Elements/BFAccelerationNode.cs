using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    
    public class BFAccelerationNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Acceleration;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            CreateIOPort("Direction", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            CreateIOPort("Speed", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            CreateIOPort("Duration", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            // Output Container
            CreateIOPort("Acceleration", outputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }
    }
}
