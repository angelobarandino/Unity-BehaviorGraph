using BehaviourGraph.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    [CustomEditor(typeof(BehaviourOwner))]
    public class BehaviourTreeInspectorEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Open", GUILayout.ExpandWidth(true)))
            {
                BehaviourGraphEditorWindow.OpenWindow();
            }
        }
    }
}
