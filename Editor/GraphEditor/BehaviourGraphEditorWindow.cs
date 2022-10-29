using System;
using BehaviourGraph.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public class BehaviourGraphEditorWindow : EditorWindow
    {
        private Label graphNameLabel;
        private ToolbarMenu toolbarMenu;
        private BehaviourGraphView graphView;

        [MenuItem("Tools/Behaviour Graph Editor")]
        public static void OpenWindow()
        {
            BehaviourGraphEditorWindow wnd = GetWindow<BehaviourGraphEditorWindow>();
            wnd.titleContent = new GUIContent("Behaviour Graph Editor");
            wnd.OnSelectionChange();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is IBehaviour)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import StyleSheet
            root.AddStyleSheet("Assets/BehaviourGraph/Editor/Resources/StyleSheets/BehaviourGraphEditorWindow.uss");

            // Import UXML
            AssetDatabase
                .LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourGraph/Editor/Resources/Uxml/BehaviourGraphEditorWindow.uxml")
                .CloneTree(root);

            graphNameLabel = root.Q<Label>("graph-name");
            toolbarMenu = root.Q<ToolbarMenu>("toolbar-menu");
            graphView = root.Q<BehaviourGraphView>("graph-view");

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject)
            {
                IBehaviour behaviour = null;
                if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent<IBehaviourOwner>(out var owner))
                    behaviour = owner.GetBehavior();

                if (Selection.activeObject is IBehaviour behaviourObject)
                    behaviour = behaviourObject;

                if (behaviour != null)
                {
                    graphNameLabel.text = GetTitle(behaviour.Name);
                    graphView.LoadBehaviourTree(behaviour);
                }
            }
        }

        private string GetTitle(string name)
        {
            var title = Selection.activeObject.name;
            if (Selection.activeGameObject)
                title += $"({name})";
            title += " - Behaviour";
            return title;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (graphView == null) return;

            switch (state)
            {
                case PlayModeStateChange.ExitingPlayMode:
                    graphView.nodes.ForEach(node => (node as GraphNodeView).SetEditModeState());
                    break;
            }
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying && graphView != null)
            {
                graphView.nodes.ForEach(node => (node as GraphNodeView).SetPlayModeState());
            }
        }
    }
}