using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    
    public class BFSpeedNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Speed;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            CreateIOPort("Speed", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            // Output Container
            CreateIOPort("Speed", outputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }
    }
}
