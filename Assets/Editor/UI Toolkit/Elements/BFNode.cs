using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace BulletForge.Elements
{
    using Enumerations;
    public class BFNode : Node
    {
        public ENodeName NodeName { get; set; }
        
        public ENodeType NodeType { get; set; }
        
        public EPatternType PatternType { get; set; }
        
        public ERunStatus RunStatus { get; set; }

        public void Initialize()
        {
            
        }

        public void Draw()
        {
            // Title Container
            TextField nodeNameTextElement = new TextField()
            {
                value = NodeName.ToString()
            };

            titleContainer.Insert(0, nodeNameTextElement);
            
            // Input Container
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Input";
            
            inputContainer.Add(inputPort);
        }
    }
}
