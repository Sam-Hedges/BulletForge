using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    using Data.Save;
    
    /// <summary>
    /// Gives a bullet a direction.
    /// </summary>
    public class BFDirectionNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Direction;
            
            // Initialize the Node Connections
            BFConnectionSaveData connectionData = new BFConnectionSaveData();
            Connections.Add(connectionData);
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Type", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            this.CreateIOPort("Vector2", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // Output Container
            this.CreateIOPort("Direction", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Connections);
        }
    }
}
