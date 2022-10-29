using System;

namespace BehaviourGraph.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class DependsOn : Attribute
    {
        public string FieldName { get; }
        public bool Invert { get; set; }
        public object Value { get; set; }
        
        public DependsOn(string fieldName)
        {
            this.FieldName = fieldName;
        }
    }
}
