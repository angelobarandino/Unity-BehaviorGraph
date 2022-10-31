using BehaviorGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    public interface INode
    {
        string Name { get; }

        SerializableGuid Id { get; }
        SerializableGuid ParentId { get; set; }

        System.Action OnNodeUpdate { get; set; }

        NodeState GetState();
        Vector2 GetPosition();
        void SetPosition(Vector2 position);
    }
}
