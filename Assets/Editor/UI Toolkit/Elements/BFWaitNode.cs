using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    
    public class BFWaitNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Wait;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            CreateIOPort("Duration", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            CreateIOPort("Times", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            // Output Container
            CreateIOPort("Action Ref", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
    }
}
