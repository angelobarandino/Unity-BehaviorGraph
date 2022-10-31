using System;
using BehaviorGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    public abstract class Node : INode
    {
        public Node()
        {
            id = Guid.NewGuid();
        }

        [SerializeField, HideInInspector]
        private SerializableGuid id;
        public SerializableGuid Id 
        {
            get => id;
        }

        [SerializeField, HideInInspector]
        private SerializableGuid parentId;
        public SerializableGuid ParentId
        {
            get => parentId;
            set => parentId = value;
        }

        [SerializeField, HideInInspector]
        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField, HideInInspector]
        private Vector2 m_position;
        public Vector2 GetPosition()
        {
            return m_position;
        }
        public void SetPosition(Vector2 position)
        {
            m_position = position;
        }

        public System.Action OnNodeUpdate { get; set; }

        public abstract NodeState GetState();
    }
}
