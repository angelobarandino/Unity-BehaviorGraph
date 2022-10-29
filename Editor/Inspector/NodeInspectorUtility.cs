using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourGraph.Runtime.Attributes;
using BehaviourGraph.Runtime.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    public class NodeInspectorUtility
    {
        public static List<NodeFieldInfo> GetNodeFields(SerializedProperty nodeProperty)
        {
            var list = new List<NodeFieldInfo>();

            if (nodeProperty.GetUnderlyingValue() is INode node)
            {
                var fieldVariables = TypeLookup.GetVariables(node.GetType());

                fieldVariables.ForEach(fieldInfo =>
                {
                    var property = nodeProperty.FindPropertyRelative(fieldInfo.Name);
                    if (property != null)
                    {
                        list.Add(new NodeFieldInfo
                        {
                            Order = GetFieldOrder(fieldInfo),
                            Label = new GUIContent(GetFieldLabel(fieldInfo)),
                            CheckValueDependsOn = () => CheckDependsOnProperty(nodeProperty, fieldInfo),
                            SerializedProperty = property,
                        });
                    }
                });
            }

            return list.OrderBy(n => n.Order).ToList();
        }

        private static string GetFieldLabel(FieldInfo fieldInfo)
        {
            var fieldLabelAttr = fieldInfo.GetCustomAttribute<FieldLabel>();
            var label = fieldLabelAttr != null
                ? fieldLabelAttr.Label
                : fieldInfo.Name.ToFriendlyName();
            return label;
        }

        private static int GetFieldOrder(FieldInfo fieldInfo)
        {
            var fieldOrderAttr = fieldInfo.GetCustomAttribute<FieldOrder>();
            return fieldOrderAttr != null ? fieldOrderAttr.Order : 0;
        }
        
        private static bool CheckDependsOnProperty(SerializedProperty nodeProperty, FieldInfo fieldInfo)
        {
            var dependsOnAttr = fieldInfo.GetCustomAttribute<DependsOn>();
            if (dependsOnAttr != null)
            {
                var dependsOnProperty = nodeProperty.FindPropertyRelative(dependsOnAttr.FieldName);
                if (dependsOnProperty != null)
                {
                    var dependsOnFieldValue = dependsOnProperty.GetUnderlyingValue();
                    if (dependsOnFieldValue is IBBVariable variable)
                        dependsOnFieldValue = variable.GetValue();

                    return dependsOnAttr.Value.Equals(dependsOnFieldValue);
                }
            }

            return true;
        }
    }
}
