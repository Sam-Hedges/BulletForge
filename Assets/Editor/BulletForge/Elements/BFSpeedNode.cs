using BulletForge.Data;
using BulletForge.Data.Save;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    
    /// <summary>
    /// Creates a speed for a bullet
    /// </summary>
    public class BFSpeedNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Speed;
            
            // Initialize the Node Connections
            BFConnectionSaveData connectionData = new BFConnectionSaveData();
            Connections.Add(connectionData);
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Speed", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // Output Container
            this.CreateIOPort("Speed", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Connections);
        }
    }
}
