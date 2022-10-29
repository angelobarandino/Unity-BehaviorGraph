using System;
using UnityEngine;

namespace BehaviourGraph
{
    [Serializable]
    public class GameObjectVariable : BBVariable<GameObject>
    {
        public static implicit operator GameObjectVariable(GameObject value) => new() { Value = value };
    }
}
