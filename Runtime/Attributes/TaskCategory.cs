using System;

namespace BehaviorGraph.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TaskCategory : Attribute
    {
        public string Category { get; }
        public string Name { get; set; }
        public int? Level { get; set; }

        public TaskCategory(string category)
        {
            Category = category;
        }
    }
}
