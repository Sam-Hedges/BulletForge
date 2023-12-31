using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BulletForge.Elements
{
    using Enumerations;
    using Utilities;
    using Data.Save;
    
    /// <summary>
    /// A base class for all nodes in the graph view to inherit from
    /// </summary>
    public class BFNode : Node
    {
        public string ID { get; set; }
        public List<BFConnectionSaveData> Connections { get; set; }
        public ENodeType NodeType { get; set; }
        
        public BFGroup Group { get; set; }
        
        /// <summary>
        /// Initializes the node parameters and position
        /// </summary>
        /// <param name="position">The position the node will be created at</param>
        public virtual void Initialize(Vector2 position)
        {
            // Set the Node NodeID
            ID = Guid.NewGuid().ToString();
            
            // Initialize the Node Connections
            Connections = new List<BFConnectionSaveData>();
            
            // Set the Node Position
            SetPosition(new Rect(position, Vector2.zero));
            
            // Add Styles to the Node Containers
            mainContainer.AddClasses("bf-node__main-container");
            extensionContainer.AddClasses("bf-node__extension-container");
        }
        
        /// <summary>
        /// Draws the node to the graph view
        /// </summary>
        public virtual void Draw()
        {
            // Create Text Element for the Title Container
            TextElement nodeNameTextElement = BFElementUtility.CreateTextElement(NodeType.ToString());
            
            // Add Styles to the Node Title
            nodeNameTextElement.AddClasses(
                "bf-node__textfield",
                "bf-node__filename-textfield",
                "bf-node__textfield__hidden"
                );
            
            // Insert the Node Title into the container
            titleContainer.Insert(0, nodeNameTextElement);
        }
        
        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }
    }
}
