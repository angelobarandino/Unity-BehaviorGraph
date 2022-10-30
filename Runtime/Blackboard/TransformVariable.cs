using System;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class TransformVariable : BBVariable<Transform>
    {
        public static implicit operator TransformVariable(Transform value) => new() { Value = value };
    }
}
