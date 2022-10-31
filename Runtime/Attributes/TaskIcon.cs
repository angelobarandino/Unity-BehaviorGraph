using System;

namespace BehaviorGraph.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TaskIcon : Attribute
    {
        public string IconPath { get; }
        public TaskIcon(string iconPath)
        {
            IconPath = iconPath;
        }   
    }
}
