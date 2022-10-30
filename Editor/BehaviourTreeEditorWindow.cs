//using BehaviorGraph.Runtime;
//using UnityEditor;
//using UnityEngine;

//namespace BehaviorGraph.Editor
//{
//    public class BehaviourTreeEditorWindow : EditorWindow
//    {
//        private BehaviourTreeGraphView graphView;

//        [MenuItem("Tools/Behaviour Tree")]
//        public static void ShowWindow()
//        {
//            var wnd = GetWindow<BehaviourTreeEditorWindow>();
//            wnd.titleContent = new GUIContent("Behaviour Tree");
//        }

//        private void OnEnable()
//        {
//            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
//            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
//        }

//        private void OnDisable()
//        {
//            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
//        }

//        private void OnPlayModeStateChange(PlayModeStateChange state)
//        {
//            switch (state)
//            {
//                case PlayModeStateChange.EnteredEditMode:
//                    OnSelectionChange();
//                    break;
//                case PlayModeStateChange.ExitingEditMode:
//                    graphView?.UpdateNodeStates();
//                    break;
//                case PlayModeStateChange.EnteredPlayMode:
//                    OnSelectionChange();
//                    break;
//                case PlayModeStateChange.ExitingPlayMode:
//                    graphView?.UpdateNodeStates();
//                    break;
//            }
//        }

//        private void CreateGUI()
//        {
//            graphView = new BehaviourTreeGraphView();

//            rootVisualElement.Add(graphView);

//            OnSelectionChange();
//        }

//        private void OnSelectionChange()
//        {
//            if (Selection.activeGameObject)
//            {
//                var behaviour = Selection.activeGameObject.GetComponent<IBehaviour>();
//                if (behaviour != null && graphView != null)
//                {
//                    graphView.PopulateView(behaviour);
//                }
//            }
//        }

//        private void OnDestroy()
//        {
//            if (graphView != null)
//            {
//                graphView.SaveViewTransform();
//            }
//        }

//        private void OnInspectorUpdate()
//        {
//            if (graphView != null && Application.isPlaying)
//            {
//                graphView.UpdateNodeStates();
//            }
//        }
//    }
//}
