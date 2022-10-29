using UnityEditor;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    [CustomEditor(typeof(BlackboardVariable))]
    public class BlackboardVariableDefaultEditor : UnityEditor.Editor
    {
        private BlackboardVariableGUI blackboardDrawer;

        private void OnEnable()
        {
            blackboardDrawer = new BlackboardVariableGUI(
                serializedObject,
                serializedObject.FindProperty("allVariables"),
                serializedObject.targetObject as BlackboardVariable
            );
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(new GUIContent("Add Variable")))
            {
                (target as BlackboardVariable).AllVariables.Add(new GameObjectVariable());
            }

            blackboardDrawer.DoLayout();
        }
    }
}
