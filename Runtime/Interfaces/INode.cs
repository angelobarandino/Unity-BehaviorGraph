﻿using BehaviourGraph.Runtime;
using UnityEngine;

namespace BehaviourGraph
{
    public interface INode
    {
        string Name { get; }

        SerializableGuid Id { get; }
        SerializableGuid ParentId { get; set; }

        System.Action OnNodeUpdate { get; set; }

        Vector2 GetPosition();
        void SetPosition(Vector2 position);
    }
}
