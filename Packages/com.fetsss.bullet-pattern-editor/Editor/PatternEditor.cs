using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BB
{
    [CustomEditor(typeof(Pattern))]
    public class PatternEditor : Editor
    {
        private Pattern targetInfo;

        private void OnEnable()
        {
            if (targetInfo == null)
            {
                targetInfo = target as Pattern;
            }
        }

        public override void OnInspectorGUI()
        {
            // Make a button that calls a static method to open the editor window,
            // passing in the scriptable object information from which the button was pressed
            if (GUILayout.Button("Open Editor Window"))
            {
                PatternEditorWindow.ShowMyEditor(targetInfo);
            }

            // Remember to display the other GUI from the object if you want to see all its normal properties
            base.OnInspectorGUI();
        }
    }
}

