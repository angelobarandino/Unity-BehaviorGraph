using UnityEditor;

namespace BehaviourGraph.Editor
{
    [CustomEditor(typeof(BlackboardViewSO))]
    public class BlackboardVariableGraphEditor : UnityEditor.Editor
    {
        private BlackboardVariableGUI blackboardGUI;

        private void OnEnable()
        {
            try
            {
                if (serializedObject != null)
                {
                    var so = serializedObject.targetObject as BlackboardViewSO;
                
                    var allVariablesProperty = serializedObject.FindProperty("allVariables");
                
                    blackboardGUI = new BlackboardVariableGUI(serializedObject, allVariablesProperty, so.Blackboard);
                }
            }
            catch {}
        }

        public override void OnInspectorGUI()
        {
            blackboardGUI.DoLayout();
        }
    }
}
