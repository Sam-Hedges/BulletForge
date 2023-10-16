using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace BulletForge
{
    public class PatternEditorWindow : EditorWindow
    {
        private Pattern currentTarget;
        private SpawnGroupGraphView graphView;
        private SpawnGroupSimulationView simulationView;

        private string windowUxml = "33ba4b17947ad824f98d56cb03eb0e02";

        public static void ShowMyEditor(Pattern target)
        {
            PatternEditorWindow wnd = GetWindow<PatternEditorWindow>();
            wnd.SetTarget(target);
            wnd.titleContent = new GUIContent("Patern Editor");
            wnd.Show();
        }

        public void CreateGUI()
        {
            var uxmlPath = AssetDatabase.GUIDToAssetPath(windowUxml);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            VisualElement uxml = visualTree.Instantiate();
            //rootVisualElement.Add(uxml);
            while (uxml.childCount > 0)
            {
                rootVisualElement.Add(uxml.ElementAt(0));
            }

            graphView = rootVisualElement.Query<SpawnGroupGraphView>("GraphView");

            rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.ctrlKey)
                {
                    if (evt.keyCode == KeyCode.S)
                    {
                        SaveSpawnGroup();
                    }
                }
            });

            simulationView = rootVisualElement.Query<SpawnGroupSimulationView>("SimulationView");

            ToolbarButton start = rootVisualElement.Query<ToolbarButton>("Start");
            start.clicked += () =>
            {
                SaveSpawnGroup();
                simulationView?.Start();
            };

            ToolbarButton pause = rootVisualElement.Query<ToolbarButton>("Pause");
            pause.clicked += () => simulationView?.Pause();

            ToolbarButton reset = rootVisualElement.Query<ToolbarButton>("Reset");
            reset.clicked += () => simulationView?.Reset();
        }

        public void SetTarget(Pattern target)
        {
            currentTarget = target;
            graphView.LoadGraph(currentTarget);
            simulationView.SetPatern(currentTarget);
        }

        private void OnDestroy()
        {
            SaveSpawnGroup();
        }

        private void OnGUI()
        {
            simulationView.OnGUI();
        }

        public void SaveSpawnGroup()
        {
            (currentTarget.root, currentTarget.allSpawnGroups) = graphView.SaveGraph();

            //graphView.LoadGraph(currentTarget);
            simulationView.SetPatern(currentTarget);

            EditorUtility.SetDirty(currentTarget);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}