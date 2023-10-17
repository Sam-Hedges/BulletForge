using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    
    public class BFDirectionNode : BFNode
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
            CreateIOPort("Type", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            CreateIOPort("Vector2", inputContainer, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            // Output Container
            CreateIOPort("Direction", outputContainer, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
    }
}
