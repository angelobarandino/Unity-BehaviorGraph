using System;

namespace BehaviourGraph
{
    [Serializable]
    public class IntVariable : BBVariable<int>
    {
        public static implicit operator IntVariable(int value) => new() { Value = value };
    }
}
