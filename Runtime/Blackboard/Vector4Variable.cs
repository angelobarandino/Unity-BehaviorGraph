using System;
using UnityEngine;

namespace BehaviourGraph
{
    [Serializable]
    public class Vector4Variable : BBVariable<Vector4>
    {
        public static implicit operator Vector4Variable(Vector4 value) => new() { Value = value };
    }
}
