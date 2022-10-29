using System;
using BehaviourGraph.Runtime;

namespace BehaviourGraph
{
    [Serializable]
    public class GuidVariable : BBVariable<SerializableGuid>
    {
        public static implicit operator GuidVariable(SerializableGuid value) => new() { Value = value };
    }
}
