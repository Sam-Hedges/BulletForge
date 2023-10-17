using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Elements
{
    
    /// <summary>
    /// A group that contains other nodes for organization
    /// </summary>
    public class BFGroup : Group
    {
        public string ID { get; set; }
        public string OldTitle { get; set; }
        
        /// <summary>
        /// The Group Constructor
        /// </summary>
        /// <param name="groupTitle">The Header Title for the group</param>
        /// <param name="position">Where the group will be created</param>
        public BFGroup(string groupTitle, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();

            title = groupTitle;
            OldTitle = groupTitle;

            SetPosition(new Rect(position, Vector2.zero));
        }
    }
}