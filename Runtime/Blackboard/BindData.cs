using System;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class BindData
    {
        [SerializeField]
        private GameObject gameObject;
        public GameObject GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }

        [SerializeField]
        private string componentName;
        public string ComponentName
        {
            get => componentName;
            set => componentName = value;
        }

        [SerializeField]
        private string propertyName;
        public string PropertyName
        {
            get => propertyName;
            set => propertyName = value;
        }

        [SerializeField]
        private string targetName;
        public string TargetName
        {
            get => targetName;
            set => targetName = value;
        }

        public bool IsBindActive => GameObject;
        
        public string DisplayText => $"{targetName}.{componentName}.{propertyName}";
    }
}
