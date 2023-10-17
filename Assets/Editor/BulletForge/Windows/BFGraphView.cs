using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace BulletForge.Windows
{
    using Elements;
    using Enumerations;
    
    /// <summary>
    /// Governs the layout of the graph view that contains the nodes
    /// </summary>
    public class BFGraphView : GraphView
    {
        // Called when the graph view is created
        public BFGraphView()
        {
            BFGraphViewManipulators graphViewManipulators = new BFGraphViewManipulators(this);
            
            AddGridBackground();
            
            AddStyles();
        }
        
        #region Overrides
        
        /// <summary>
        ///   <para>Get all ports compatible with given port</para>
        /// </summary>
        /// <param name="startPort">Start port to validate against</param>
        /// <param name="nodeAdapter">Node adapter</param>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            
            ports.ForEach(port => {
                // If the port is the same as the start port, then skip
                // If the port is on the same node as the start port, then skip
                // If the port is the same direction as the start port, then skip
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction) {
                    compatiblePorts.Add(port);
                }
            });
            
            return compatiblePorts;
        }
        
        #endregion

        #region Visual Methods
        
        /// <summary>
        /// Adds a grid background to the graph view
        /// </summary>
        private void AddGridBackground()
        {
            // Create a grid background
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        
        /// <summary>
        /// Applies the visual parameters within the stylesheet to the graph view
        /// </summary>
        private void AddStyles()
        {
            StyleSheet graphViewStyleSheet = EditorGUIUtility.Load("BulletForge/BFGraphViewStyles.uss") as StyleSheet;
            StyleSheet nodeStyleSheet = EditorGUIUtility.Load("BulletForge/BFNodeStyles.uss") as StyleSheet;

            styleSheets.Add(graphViewStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }
        
        #endregion
    }
}
