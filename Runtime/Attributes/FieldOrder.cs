using System;

namespace BehaviorGraph.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FieldOrder : Attribute
    {
        public int Order { get; set; }
    }
}
