using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;

    public class BFSpeedNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Speed;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Speed", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // Output Container
            this.CreateIOPort("Speed", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
        }
    }
}
