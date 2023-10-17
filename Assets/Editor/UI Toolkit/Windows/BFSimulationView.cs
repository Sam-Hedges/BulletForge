using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Internal;
using Object = UnityEngine.Object;

using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.UIElements.StyleSheets;

namespace BulletForge.Windows
{
    /// <summary>
    /// Preview of the pattern, simulated in the editor
    /// </summary>
    public class BFSimulationView : VisualElement
    {
        /*  UNITY STUFF
        Image m_PreviewTextureView;

        public Image previewTextureView
        {
            get { return m_PreviewTextureView; }
        }
        

        bool m_RecalculateLayout;



        VisualElement m_Preview;
        Label m_Title;

        public VisualElement preview
        {
            get { return m_Preview; }
        }
        
        
        public BFSimulationView()
        {
            style.overflow = Overflow.Hidden;

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/MasterPreviewView"));


            var topContainer = new VisualElement() { name = "top" };
            {
                m_Title = new Label() { name = "title" };
                m_Title.text = "Main Preview";

                topContainer.Add(m_Title);
            }
            Add(topContainer);

            m_Preview = new VisualElement { name = "middle" };
            {
                m_PreviewTextureView = CreatePreview(Texture2D.blackTexture);
                preview.Add(m_PreviewTextureView);
                preview.AddManipulator(new Scrollable(OnScroll));
            }
            Add(preview);

            m_RecalculateLayout = false;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        Image CreatePreview(Texture texture)
        {
            if (m_PreviewRenderHandle?.texture != null)
            {
                texture = m_PreviewRenderHandle.texture;
            }

            var image = new Image { name = "preview", image = texture, scaleMode = ScaleMode.ScaleAndCrop };
            image.AddManipulator(new Draggable(OnMouseDragPreviewMesh, true));
            image.AddManipulator((IManipulator)Activator.CreateInstance(s_ContextualMenuManipulator, (Action<ContextualMenuPopulateEvent>)BuildContextualMenu));
            return image;
        }

        void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            foreach (var primitiveTypeName in Enum.GetNames(typeof(PrimitiveType)))
            {
                if (m_DoNotShowPrimitives.Contains(primitiveTypeName))
                    continue;
                evt.menu.AppendAction(primitiveTypeName, e => ChangePrimitiveMesh(primitiveTypeName), DropdownMenuAction.AlwaysEnabled);
            }

            evt.menu.AppendAction("Sprite", e => ChangeMeshSprite(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Custom Mesh", e => ChangeMeshCustom(), DropdownMenuAction.AlwaysEnabled);
        }
        
        
        private static EditorWindow Get()
        {
            PropertyInfo P = s_ObjectSelector.GetProperty("get", BindingFlags.Public | BindingFlags.Static);
            return P.GetValue(null, null) as EditorWindow;
        }
        */
    }
}