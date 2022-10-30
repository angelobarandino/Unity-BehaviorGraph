using System;
using BehaviorGraph.Runtime;

namespace BehaviorGraph.Runtime
{
    [Serializable]
    public class GuidVariable : BBVariable<SerializableGuid>
    {
        public static implicit operator GuidVariable(SerializableGuid value) => new() { Value = value };
    }
}
