using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BulletForge.Windows
{
    using Elements;
    using Enumerations;

    public class BFSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BFGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(BFGraphView bfGraphView)
        {
            graphView = bfGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeGroupEntry(new GUIContent("Sequence Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Fire", indentationIcon))
                {
                    userData = ENodeType.Fire,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Vanish", indentationIcon))
                {
                    userData = ENodeType.Vanish,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Wait", indentationIcon))
                {
                    userData = ENodeType.Wait,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Repeat", indentationIcon))
                {
                    userData = ENodeType.Repeat,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Dynamic Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Speed", indentationIcon))
                {
                    userData = ENodeType.Speed,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Acceleration", indentationIcon))
                {
                    userData = ENodeType.Acceleration,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Direction", indentationIcon))
                {
                    userData = ENodeType.Direction,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Modifier Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Change Speed", indentationIcon))
                {
                    userData = ENodeType.ChangeSpeed,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Change Direction", indentationIcon))
                {
                    userData = ENodeType.ChangeDirection,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Groups"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 2
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case ENodeType.Fire:
                {
                    BFFireNode singleChoiceNode = (BFFireNode) graphView.CreateNode("Fire", ENodeType.Fire, localMousePosition);

                    graphView.AddElement(singleChoiceNode);

                    return true;
                }

                case ENodeType.Vanish:
                {
                    BFVanishNode multipleChoiceNode = (BFVanishNode) graphView.CreateNode("Vanish", ENodeType.Vanish, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.Wait:
                {
                    BFWaitNode multipleChoiceNode = (BFWaitNode) graphView.CreateNode("Wait", ENodeType.Wait, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.Repeat:
                {
                    BFRepeatNode multipleChoiceNode = (BFRepeatNode) graphView.CreateNode("Repeat", ENodeType.Repeat, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.Speed:
                {
                    BFSpeedNode multipleChoiceNode = (BFSpeedNode) graphView.CreateNode("Speed", ENodeType.Speed, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.Acceleration:
                {
                    BFAccelerationNode multipleChoiceNode = (BFAccelerationNode) graphView.CreateNode("Acceleration", ENodeType.Acceleration, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.Direction:
                {
                    BFDirectionNode multipleChoiceNode = (BFDirectionNode) graphView.CreateNode("Direction", ENodeType.Direction, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.ChangeSpeed:
                {
                    BFChangeSpeedNode multipleChoiceNode = (BFChangeSpeedNode) graphView.CreateNode("ChangeSpeed", ENodeType.ChangeSpeed, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }
                
                case ENodeType.ChangeDirection:
                {
                    BFChangeDirectionNode multipleChoiceNode = (BFChangeDirectionNode) graphView.CreateNode("ChangeDirection", ENodeType.ChangeDirection, localMousePosition);

                    graphView.AddElement(multipleChoiceNode);

                    return true;
                }

                case Group _:
                {
                    graphView.CreateGroup("Group", localMousePosition);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}