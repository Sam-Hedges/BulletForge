using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    using Data.Save;
    
    /// <summary>
    /// Sends the logic to make a bullet vanish
    /// </summary>
    public class BFVanishNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Vanish;
            
            // Initialize the Node Connections
            BFConnectionSaveData connectionData = new BFConnectionSaveData();
            Connections.Add(connectionData);
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Bullet Ref", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // No Output Container
        }
    }
}
