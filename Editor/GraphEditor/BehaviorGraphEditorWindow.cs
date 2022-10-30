using BehaviorGraph.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    public class BehaviourGraphEditorWindow : EditorWindow
    {
        private Label graphNameLabel;
        private ToolbarMenu toolbarMenu;
        private BehaviorGraphView graphView;

        [MenuItem("Tools/BehaviorGraph Editor")]
        public static void OpenWindow()
        {
            BehaviourGraphEditorWindow wnd = GetWindow<BehaviourGraphEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviorGraph Editor");
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
            var root = rootVisualElement;

            // Import StyleSheet
            root.AddStyleSheet("Assets/BehaviorGraph/Editor/Resources/StyleSheets/BehaviorGraphEditorWindow.uss");

            // Import UXML
            AssetDatabase
                .LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviorGraph/Editor/Resources/Uxml/BehaviorGraphEditorWindow.uxml")
                .CloneTree(root);

            graphNameLabel = root.Q<Label>("graph-name");
            toolbarMenu = root.Q<ToolbarMenu>("toolbar-menu");
            graphView = root.Q<BehaviorGraphView>("graph-view");

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
                    graphView.LoadBehaviorTree(behaviour);
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

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        }

        private void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (graphView == null) return;

            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    OnSelectionChange();
                    break;
            }
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying && graphView != null)
            {
                graphView.nodes.ForEach(node =>
                {
                    if (node is GraphNodeView graphNode)
                        graphNode.UpdatePlaymodeStates();
                });
            }
        }
    }
}