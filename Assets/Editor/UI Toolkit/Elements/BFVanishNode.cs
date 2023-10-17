using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    
    public class BFVanishNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Vanish;
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            CreateIOPort("Bullet Ref", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            // No Output Container
        }
    }
}
