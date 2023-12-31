using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    using Data.Save;
    
    /// <summary>
    /// Sends the logic to fire a bullet
    /// </summary>
    public class BFFireNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Fire;
            
            // Initialize the Node Connections
            BFConnectionSaveData connectionData = new BFConnectionSaveData();
            Connections.Add(connectionData);
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Direction", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            this.CreateIOPort("Speed", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            this.CreateIOPort("Bullet Ref", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // No Output Container
        }
    }
}
