using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    
    public class BFChangeSpeedNode : BFNode
    {
        public override void Initialize(string nodeName, BFGraphView bfGraphView, Vector2 position) {
            base.Initialize(nodeName, bfGraphView, position);
            
            NodeType = ENodeType.Acceleration;
            
            BFPatternSaveData choiceData = new BFPatternSaveData()
            {
                Text = "Next Dialogue"
            };

            Pattern.Add(choiceData);
        }

        public override void Draw() {
            base.Draw();
            
            // Input Container
            Port directionPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            directionPort.portName = "Input";
            inputContainer.Add(directionPort);
            
            Port speedPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            speedPort.portName = "Input";
            inputContainer.Add(speedPort);
            
            // No Output Container
        }
    }
}
