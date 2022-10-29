using System;

namespace BehaviourGraph
{
    public interface IBBVariable
    {
        string Name { get; set; }
        bool Invalid { get; set; }
        string ReferenceName { get; set; }
        bool IsReferenced { get; set; }
        bool IsDynamic { get; set; }
        BindData BindData { get; }

        object GetValue();
        void SetValue(object value);
        Type GetVariableType();

        bool IsBindActive();
        void Bind(BindData data);
        void ToggleIsReference();
        void Initialize(IBlackboard blackboard);
    }
}
