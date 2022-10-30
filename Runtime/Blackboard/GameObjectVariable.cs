using System;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class GameObjectVariable : BBVariable<GameObject>
    {
        public static implicit operator GameObjectVariable(GameObject value) => new() { Value = value };
    }
}
