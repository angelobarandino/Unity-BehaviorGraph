using System;
using UnityEditor;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    public struct NodeFieldInfo
    {
        public int Order { get; set; }
        public GUIContent Label { get; set; }
        public Func<bool> CheckValueDependsOn { get; set; }
        public SerializedProperty SerializedProperty { get; set; }
    }
}
