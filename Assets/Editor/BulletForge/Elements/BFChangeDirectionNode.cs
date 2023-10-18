using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    
    /// <summary>
    /// Changes the direction of a bullet.
    /// </summary>
    public class BFChangeDirectionNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.ChangeDirection;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Bullet Ref", inputContainer, Orientation.Horizontal, Direction.Input);
            this.CreateIOPort("New Direction", inputContainer, Orientation.Horizontal, Direction.Input);
            
            // No Output Container
        }
    }
}
