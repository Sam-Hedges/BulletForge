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
    using Utilities;
    
    /// <summary>
    /// Governs the layout of the graph view that contains the nodes
    /// </summary>
    public class BFGraphView : GraphView
    {
        private BFSearchWindow searchWindow;
        public BFGraphViewManipulators graphViewManipulators;
        
        // Called when the graph view is created
        public BFGraphView(BFEditorWindow bfEditorWindow)
        {
            graphViewManipulators = new BFGraphViewManipulators(this, bfEditorWindow);
                
            AddSearchWindow(graphViewManipulators);
            
            AddGridBackground();
            
            AddStyles();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphViewManipulators"></param>
        private void AddSearchWindow(BFGraphViewManipulators graphViewManipulators)
        {
            if (searchWindow == null) {
                searchWindow = ScriptableObject.CreateInstance<BFSearchWindow>();
                searchWindow.Initialize(this, graphViewManipulators);
            }
            
            // Opens the search window at the mouse position
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
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
            this.AddStyleSheets(
                "BulletForge/BFGraphViewStyles.uss", 
                "BulletForge/BFNodeStyles.uss"
                );
        }
        
        #endregion
    }
}
