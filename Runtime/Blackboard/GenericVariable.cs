namespace BehaviourGraph
{
    public class GenericVariable<T> : BBVariable<T>
    {
        public GenericVariable(){ }
        public GenericVariable(bool isReferenced) : base(isReferenced) { }
        
        public static implicit operator GenericVariable<T>(T value) => new() { Value = value };
    }
}
