using BehaviorGraph.Runtime.Attributes;

namespace BehaviorGraph.Runtime.Tasks.Actions
{
    public abstract class SetVariable<T> : Action
    {
        public GenericVariable<T> variable = new(true);
        public GenericVariable<T> setValue = new();

        protected override NodeState OnUpdate()
        {
            variable.Value = setValue.Value;
            return NodeState.Success;
        }
    }

    [TaskCategory("Set Variable", Name = "Bool")]
    public class SetVariableBool : SetVariable<bool> { }

    [TaskCategory("Set Variable", Name = "Float")]
    public class SetVariableFloat : SetVariable<float> { }

    [TaskCategory("Set Variable", Name = "Integer")]
    public class SetVariableInteger : SetVariable<int> { }

    [TaskCategory("Set Variable", Name = "String")]
    public class SetVariableString : SetVariable<string> { }
}