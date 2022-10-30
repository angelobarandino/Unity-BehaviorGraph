using System;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class Vector3Variable : BBVariable<Vector3>
    {
        public static implicit operator Vector3Variable(Vector3 value) => new() { Value = value };
    }
}
