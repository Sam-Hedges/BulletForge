using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    
    public class BFDirectionNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Direction;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Type", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            this.CreateIOPort("Vector2", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // Output Container
            this.CreateIOPort("Direction", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
        }
    }
}
