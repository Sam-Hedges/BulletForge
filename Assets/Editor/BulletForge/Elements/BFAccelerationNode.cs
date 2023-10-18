using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    using Data.Save;
    
    /// <summary>
    /// Applies an acceleration to a bullet.
    /// </summary>
    public class BFAccelerationNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Acceleration;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Direction", inputContainer, Orientation.Horizontal, Direction.Input);
            this.CreateIOPort("Speed", inputContainer, Orientation.Horizontal, Direction.Input);
            this.CreateIOPort("Duration", inputContainer, Orientation.Horizontal, Direction.Input);
            
            // Output Container
            this.CreateIOPort("Acceleration", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
        }
    }
}
