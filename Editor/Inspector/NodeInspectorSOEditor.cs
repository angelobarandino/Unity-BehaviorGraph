using UnityEditor;

namespace BehaviourGraph.Editor
{
    [CustomEditor(typeof(NodeInspectorSO))]
    public class NodeInspectorSOEditor : UnityEditor.Editor
    {
        private NodeInspectorSO nodeInspector;
        public override void OnInspectorGUI()
        {
            nodeInspector = target as NodeInspectorSO;
            if (nodeInspector == null) return;

            var nodeProperty = serializedObject.FindProperty("node");
            if (nodeProperty == null) return;

            NodeInspectorUtility.GetNodeFields(nodeProperty).ForEach(field =>
            {
                if (field.CheckValueDependsOn())
                {
                    try
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(field.SerializedProperty, field.Label);
                        serializedObject.ApplyModifiedProperties();
                        if (EditorGUI.EndChangeCheck())
                        {
                            nodeInspector.node.OnNodeUpdate?.Invoke();
                        }
                    }
                    catch (System.Exception)
                    {   
                        throw;
                    }
                }
            });
        }
    }
}
