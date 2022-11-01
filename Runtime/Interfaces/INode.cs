using System;
using BehaviorGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    public interface INode : ICloneable
    {
        string Name { get; }

        SerializableGuid Id { get; }
        SerializableGuid ParentId { get; set; }

        System.Action OnNodeUpdate { get; set; }

        void SetId(SerializableGuid id);
        void SetPosition(Vector2 position);
        Vector2 GetPosition();
        NodeState GetState();
    }
}
