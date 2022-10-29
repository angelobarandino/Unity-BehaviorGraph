using System;

namespace BehaviourGraph
{
    [Serializable]
    public class FloatVariable : BBVariable<float> 
    {
        public static implicit operator FloatVariable(float value) => new() { Value = value };
    }
}
