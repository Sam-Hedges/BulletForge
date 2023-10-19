using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    
    /// <summary>
    /// Wait for a certain amount of time then continues to the following actions
    /// </summary>
    public class BFWaitNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Wait;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Duration", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            this.CreateIOPort("Times", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // Output Container
            this.CreateIOPort("Action Ref", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Connections);
        }
    }
}
