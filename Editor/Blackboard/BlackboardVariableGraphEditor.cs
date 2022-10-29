using UnityEditor;

namespace BehaviourGraph.Editor
{
    [CustomEditor(typeof(BlackboardViewSO))]
    public class BlackboardVariableGraphEditor : UnityEditor.Editor
    {
        private BlackboardVariableGUI blackboardDrawer;

        private void OnEnable()
        {
            try
            {
                if (serializedObject != null)
                {
                    var so = serializedObject.targetObject as BlackboardViewSO;
                
                    var allVariablesProperty = serializedObject.FindProperty("allVariables");
                
                    blackboardDrawer = new BlackboardVariableGUI(serializedObject, allVariablesProperty, so.GetBlackboard());
                }
            }
            catch {}
        }

        public override void OnInspectorGUI()
        {
            blackboardDrawer.DoLayout();
        }
    }
}
