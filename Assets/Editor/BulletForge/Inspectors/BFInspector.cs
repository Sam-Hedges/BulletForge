using System.Collections.Generic;
using UnityEditor;

namespace BulletForge.Inspectors
{
    using Utilities;
    using ScriptableObjects;

    [CustomEditor(typeof(BFPattern))]
    public class BFInspector : Editor
    {
        /* Pattern Scriptable Objects */
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;

        /* Filters */
        private SerializedProperty groupedPatternsProperty;
        private SerializedProperty startingPatternsOnlyProperty;

        /* Indexes */
        private SerializedProperty selectedPatternGroupIndexProperty;
        private SerializedProperty selectedPatternIndexProperty;

        private void OnEnable()
        {
            dialogueContainerProperty = serializedObject.FindProperty("patternContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");

            groupedPatternsProperty = serializedObject.FindProperty("groupedPatterns");
            startingPatternsOnlyProperty = serializedObject.FindProperty("startingPatternsOnly");

            selectedPatternGroupIndexProperty = serializedObject.FindProperty("selectedPatternGroupIndex");
            selectedPatternIndexProperty = serializedObject.FindProperty("selectedPatternIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPatternContainerArea();

            BFPatternContainerSO currentPatternContainer = (BFPatternContainerSO) dialogueContainerProperty.objectReferenceValue;

            if (currentPatternContainer == null)
            {
                StopDrawing("Select a Pattern Container to see the rest of the Inspector.");

                return;
            }

            DrawFiltersArea();

            bool currentGroupedPatternsFilter = groupedPatternsProperty.boolValue;
            bool currentStartingPatternsOnlyFilter = startingPatternsOnlyProperty.boolValue;
            
            List<string> dialogueNames;

            string dialogueFolderPath = $"Assets/PatternSystem/Patterns/{currentPatternContainer.FileName}";

            string dialogueInfoMessage;

            if (currentGroupedPatternsFilter)
            {
                List<string> dialogueGroupNames = currentPatternContainer.GetPatternGroupNames();

                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("There are no Pattern Groups in this Pattern Container.");

                    return;
                }

                DrawPatternGroupArea(currentPatternContainer, dialogueGroupNames);

                BFPatternGroupSO dialogueGroup = (BFPatternGroupSO) dialogueGroupProperty.objectReferenceValue;

                dialogueNames = currentPatternContainer.GetGroupedPatternNames(dialogueGroup, currentStartingPatternsOnlyFilter);

                dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Patterns";

                dialogueInfoMessage = "There are no" + (currentStartingPatternsOnlyFilter ? " Starting" : "") + " Patterns in this Pattern Group.";
            }
            else
            {
                dialogueNames = currentPatternContainer.GetUngroupedPatternNames(currentStartingPatternsOnlyFilter);

                dialogueFolderPath += "/Global/Patterns";

                dialogueInfoMessage = "There are no" + (currentStartingPatternsOnlyFilter ? " Starting" : "") + " Ungrouped Patterns in this Pattern Container.";
            }

            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);

                return;
            }

            DrawPatternArea(dialogueNames, dialogueFolderPath);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPatternContainerArea()
        {
            BFInspectorUtility.DrawHeader("Pattern Container");

            dialogueContainerProperty.DrawPropertyField();

            BFInspectorUtility.DrawSpace();
        }

        private void DrawFiltersArea()
        {
            BFInspectorUtility.DrawHeader("Filters");

            groupedPatternsProperty.DrawPropertyField();
            startingPatternsOnlyProperty.DrawPropertyField();

            BFInspectorUtility.DrawSpace();
        }

        private void DrawPatternGroupArea(BFPatternContainerSO patternContainer, List<string> dialogueGroupNames)
        {
            BFInspectorUtility.DrawHeader("Pattern Group");

            int oldSelectedPatternGroupIndex = selectedPatternGroupIndexProperty.intValue;

            BFPatternGroupSO oldPatternGroup = (BFPatternGroupSO) dialogueGroupProperty.objectReferenceValue;

            bool isOldPatternGroupNull = oldPatternGroup == null;

            string oldPatternGroupName = isOldPatternGroupNull ? "" : oldPatternGroup.GroupName;

            UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedPatternGroupIndexProperty, oldSelectedPatternGroupIndex, oldPatternGroupName, isOldPatternGroupNull);

            selectedPatternGroupIndexProperty.intValue = BFInspectorUtility.DrawPopup("Pattern Group", selectedPatternGroupIndexProperty, dialogueGroupNames.ToArray());

            string selectedPatternGroupName = dialogueGroupNames[selectedPatternGroupIndexProperty.intValue];

            BFPatternGroupSO selectedPatternGroup = BFIOUtility.LoadAsset<BFPatternGroupSO>($"Assets/PatternSystem/Patterns/{patternContainer.FileName}/Groups/{selectedPatternGroupName}", selectedPatternGroupName);

            dialogueGroupProperty.objectReferenceValue = selectedPatternGroup;

            BFInspectorUtility.DrawDisabledFields(() => dialogueGroupProperty.DrawPropertyField());

            BFInspectorUtility.DrawSpace();
        }

        private void DrawPatternArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            BFInspectorUtility.DrawHeader("Pattern");

            int oldSelectedPatternIndex = selectedPatternIndexProperty.intValue;

            BFPatternSO oldPattern = (BFPatternSO) dialogueProperty.objectReferenceValue;

            bool isOldPatternNull = oldPattern == null;

            string oldPatternName = isOldPatternNull ? "" : oldPattern.PatternName;

            UpdateIndexOnNamesListUpdate(dialogueNames, selectedPatternIndexProperty, oldSelectedPatternIndex, oldPatternName, isOldPatternNull);

            selectedPatternIndexProperty.intValue = BFInspectorUtility.DrawPopup("Pattern", selectedPatternIndexProperty, dialogueNames.ToArray());

            string selectedPatternName = dialogueNames[selectedPatternIndexProperty.intValue];

            BFPatternSO selectedPattern = BFIOUtility.LoadAsset<BFPatternSO>(dialogueFolderPath, selectedPatternName);

            dialogueProperty.objectReferenceValue = selectedPattern;

            BFInspectorUtility.DrawDisabledFields(() => dialogueProperty.DrawPropertyField());
        }

        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            BFInspectorUtility.DrawHelpBox(reason, messageType);

            BFInspectorUtility.DrawSpace();

            BFInspectorUtility.DrawHelpBox("You need to select a Pattern for this component to work properly at Runtime!", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;

                return;
            }

            bool oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
            bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];

            if (oldNameIsDifferentThanSelectedName)
            {
                if (optionNames.Contains(oldPropertyName))
                {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);

                    return;
                }

                indexProperty.intValue = 0;
            }
        }
    }
}