using System.Collections.Generic;
using UnityEngine;

namespace BehaviourGraph
{
    public interface IBlackboard
    {
        GameObject GameObject { get; }
        List<IBBVariable> AllVariables { get; }

        IBBVariable GetVariable(string variableName);
        T GetValue<T>(string variableName);
        void SetValue<T>(string variableName, T value);

        void AddVariable(IBBVariable variable);
        void RemoveVariable(IBBVariable variable);
        bool IsVariableNameInValid(string nameValue);
        void ChangeVariableType(IBBVariable newVariable, IBBVariable variable);

        IBBVariable AddGetDynamicVariable(IBBVariable variable);
    }
}
