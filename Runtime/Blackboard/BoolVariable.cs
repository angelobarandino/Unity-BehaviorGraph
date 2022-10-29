using System;

namespace BehaviourGraph
{
    [Serializable]
    public class BoolVariable : BBVariable<bool>
    {
        public static implicit operator BoolVariable(bool value) => new() { Value = value };
    }
}
