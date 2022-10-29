using System;

namespace BehaviourGraph.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FieldLabel : Attribute
    {
        public string Label { get; }
        public FieldLabel(string label)
        {
            this.Label = label;
        }
    }
}
