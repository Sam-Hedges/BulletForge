using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    
    public class BFRepeatNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Repeat;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            CreateIOPort("Action Ref", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            CreateIOPort("Times", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            // Output Container
            CreateIOPort("Action Ref", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
    }
}
