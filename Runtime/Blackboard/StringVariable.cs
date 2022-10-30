using System;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class StringVariable : BBVariable<string>
    {
        public static implicit operator StringVariable(string value) => new() { Value = value };
    }
}
