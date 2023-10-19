using System;
using System.Collections.Generic;
using BulletForge.Data.Save;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BulletForge.Utilities
{
    using Elements;

    /// <summary>
    /// Used to easily implement UIElements
    /// </summary>
    public static class BFElementUtility
    {
        
        /// <summary>
        /// Creates a Button UI Element
        /// </summary>
        /// <param name="text">The Button Text</param>
        /// <param name="onClick">The action triggered when the button is clicked</param>
        /// <returns></returns>
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };

            return button;
        }
        
        /// <summary>
        /// Creates a Foldout UI Element
        /// </summary>
        /// <param name="title">The Header of the Foldout</param>
        /// <param name="collapsed">The state of the Foldout's toggle</param>
        /// <returns></returns>
        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }
        
        /// <summary>
        /// Creates a Node UI Element
        /// </summary>
        /// <param name="node">The current Node creating the element</param>
        /// <param name="container">The container to add the the port to</param>
        /// <param name="portName">The display text</param>
        /// <param name="orientation">Graph element orientation</param>
        /// <param name="direction">Port direction (input or output)</param>
        /// <param name="capacity">Specifies how many edges a port can have connected</param>
        /// <returns></returns>
        public static void CreateIOPort(this BFNode node, string portName, VisualElement container, Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single, List<BFConnectionSaveData> connections = null)
        {
            if (connections == null)
            {
                Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
                port.portName = portName;
                container.Add(port);
            }

            if (connections != null)
                foreach (var connection in connections)
                {
                    Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
                    port.portName = portName;
                    port.userData = connection;
                    container.Add(port);
                }
        }
        
        /// <summary>
        /// Creates a Text UI Element
        /// </summary>
        /// <param name="value">The text to be displayed</param>
        /// <returns></returns>
        public static TextElement CreateTextElement(string value = null)
        {
            TextElement textElement = new TextElement()
            {
                text = value,
            };

            return textElement;
        }
        
        /// <summary>
        /// Creates a Editable Text Field UI Element
        /// </summary>
        /// <param name="value">The text to be displayed</param>
        /// <param name="label">The text of the label that will appear beside the field</param>
        /// <param name="onValueChanged">The callback event triggered when the text is changed</param>
        /// <returns></returns>
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        /// <summary>
        /// Creates a Editable Text Field UI Element within a scrollable container
        /// </summary>
        /// <param name="value">The text to be displayed</param>
        /// <param name="label">The text of the label that will appear beside the field</param>
        /// <param name="onValueChanged">The callback event triggered when the text is changed</param>
        /// <returns></returns>
        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);

            textArea.multiline = true;

            return textArea;
        }
    }
}