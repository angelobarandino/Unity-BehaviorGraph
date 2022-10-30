using System;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class Vector2Variable : BBVariable<Vector2>
    {
        public static implicit operator Vector2Variable(Vector2 value) => new() { Value = value };
    }
}
