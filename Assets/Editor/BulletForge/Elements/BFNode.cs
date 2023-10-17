using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BulletForge.Elements
{
    using Enumerations;
    
    /// <summary>
    /// A base class for all nodes in the graph view to inherit from
    /// </summary>
    public class BFNode : Node
    {
        public ENodeType NodeType { get; set; }
        
        /// <summary>
        /// Initializes the node parameters and position
        /// </summary>
        /// <param name="position">The position the node will be created at</param>
        public virtual void Initialize(Vector2 position)
        {
            SetPosition(new Rect(position, Vector2.zero));
            
            // Add Styles to the Node Containers
            mainContainer.AddToClassList("bf-node__main-container");
            extensionContainer.AddToClassList("bf-node__extension-container");
        }
        
        /// <summary>
        /// Draws the node to the graph view
        /// </summary>
        public virtual void Draw()
        {
            // Create Text Element for the Title Container
            TextElement nodeNameTextElement = new TextElement()
            {
                text = NodeType.ToString()
            };
            
            // Add Styles to the Node Title
            nodeNameTextElement.AddToClassList("bf-node__textfield");
            nodeNameTextElement.AddToClassList("bf-node__filename-textfield");
            nodeNameTextElement.AddToClassList("bf-node__textfield__hidden");
            
            // Insert the Node Title into the container
            titleContainer.Insert(0, nodeNameTextElement);
        }
        
        /// <summary>
        /// Creates a port for the node and add it to a container
        /// </summary>
        /// <param name="portName">Display name of the port</param>
        /// <param name="container">The container of the node the port will be placed into</param>
        /// <param name="orientation">Whether the port is vertical or horizontal</param>
        /// <param name="direction">Whether the port is left or right</param>
        /// <param name="capacity">How many connections can be made to the port</param>
        /// <param name="type">Whether the port an Input or Output</param>
        public void CreateIOPort(string portName, VisualElement container, Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            Port directionPort = InstantiatePort(orientation, direction, capacity, type);
            directionPort.portName = portName;
            container.Add(directionPort);
        }
    }
}
