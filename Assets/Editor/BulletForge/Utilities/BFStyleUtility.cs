using UnityEditor;
using UnityEngine.UIElements;

namespace BulletForge.Utilities
{
    
    /// <summary>
    /// Used to apply styles to UIElements
    /// </summary>
    public static class BFStyleUtility
    {
        /// <summary>
        /// Applies the visual parameters within a stylesheet to a UI element
        /// </summary>
        /// <param name="element">The Elements the Style Sheets will be applied to</param>
        /// <param name="classNames">The file paths from Editor Default Resources</param>
        /// <returns></returns>
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
        
        /// <summary>
        /// Applies the visual parameters within a stylesheet to the visual element
        /// </summary>
        /// <param name="element">The Elements the Style Sheets will be applied to</param>
        /// <param name="styleSheetNames">The file paths from Editor Default Resources</param>
        /// <returns></returns>
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(styleSheetName);

                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}