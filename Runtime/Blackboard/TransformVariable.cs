using System;
using UnityEngine;

namespace BehaviourGraph
{
    [Serializable]
    public class TransformVariable : BBVariable<Transform>
    {
        public static implicit operator TransformVariable(Transform value) => new() { Value = value };
    }
}
