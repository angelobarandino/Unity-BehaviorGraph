using System;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks
{
    [Serializable]
    public struct TaskData
    {
        [SerializeField]
        public string guid;

        [SerializeField]
        private bool isStart;
        public bool IsStart
        {
            get => isStart;
            set => isStart = value;
        }

        [SerializeField]
        private string parent;
        public string Parent
        {
            set { parent = value; }
            get { return parent; }
        }

        [SerializeField]
        [HideInInspector]
        public Vector2 position;

        private string description;
        public string Description
        {
            get { return description ?? string.Empty; }
            set { description = value; }
        }

        public TaskData Clone()
        {
            return new TaskData
            {
                guid = guid,
                Parent = this.Parent,
                IsStart = this.IsStart,
                Description = this.Description,
                position = this.position,
            };
        }
    }
}