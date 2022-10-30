using System;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class IntVariable : BBVariable<int>
    {
        public static implicit operator IntVariable(int value) => new() { Value = value };
    }
}
