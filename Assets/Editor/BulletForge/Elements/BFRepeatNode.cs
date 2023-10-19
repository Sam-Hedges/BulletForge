using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    using Data.Save;
    
    /// <summary>
    /// Repeats the following actions a number of times
    /// </summary>
    public class BFRepeatNode : BFNode
    {
        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            
            NodeType = ENodeType.Repeat;
            
            // Initialize the Node Connections
            BFConnectionSaveData connectionData = new BFConnectionSaveData();
            Connections.Add(connectionData);
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            this.CreateIOPort("Action Ref", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            this.CreateIOPort("Times", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            
            // Output Container
            this.CreateIOPort("Action Ref", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, Connections);
        }
    }
}
