using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Behavior Graph/Blackboard Variable")]
    public class BlackboardVariable : MonoBehaviour, IBlackboard
    {
        public enum VariableScope
        {
            Local, Global
        }

        private List<IBBVariable> dynamicVariables = new();

        [SerializeReference]
        private List<IBBVariable> allVariables = new();
        public List<IBBVariable> AllVariables
        {
            get => allVariables;
        }

        public GameObject GameObject 
        {
            get => gameObject; 
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            for (int index = 0; index < allVariables.Count; index++)
            {
                allVariables[index].Initialize(this);
            }
        }

        public T GetValue<T>(string name) 
        {
            for (int index = 0; index < allVariables.Count; index++)
            {
                var variable = allVariables[index];
                if (variable.Invalid) continue;
                if (variable.Name.Equals(name))
                {
                    return (T)variable.GetValue();
                }
            }

            return default; 
        }

        public void SetValue<T>(string name, T value) 
        {
            for (int index = 0; index < allVariables.Count; index++)
            {
                var variable = allVariables[index];
                if (variable.Invalid) continue;
                if (variable.Name.Equals(name))
                {
                    variable.SetValue(value);
                }
            }
        }

        public void RemoveVariable(IBBVariable variable)
        {
            allVariables.Remove(variable);
        }

        public void AddVariable(IBBVariable variable)
        {
            if (allVariables.Any(x => x.Name == variable.Name))
                return;

            allVariables.Add(variable);
        }

        public void ChangeVariableType(IBBVariable @new, IBBVariable old)
        {
            var index = allVariables.IndexOf(old);
            allVariables.Insert(index, @new);
            allVariables.Remove(old);
        }

        public bool IsVariableNameInValid(string variableName)
        {
            if (!string.IsNullOrWhiteSpace(variableName))
            {
                // checks count to 1, means checking itself
                if (!(allVariables?.Count(v => v.Name == variableName) > 1))
                {
                    if (Regex.Matches(variableName, "^(_)*[a-zA-Z][a-zA-Z0-9]*?$").Count > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public IBBVariable GetVariable(string variableName)
        {
            for (var index = 0; index < allVariables.Count; index++)
            {
                var variable = allVariables[index];
                if (variable.Name.Equals(variableName))
                {
                    if (variable.Invalid)
                    {
                        Debug.LogError($"Get Variable '{variableName}' is invalid.");
                    }

                    return variable;
                }
            }

            return null;
        }

        public IBBVariable AddGetDynamicVariable(IBBVariable variable)
        {
            var index = dynamicVariables.FindIndex(v => v.Name == variable.Name);
            if (index == -1)
            {
                index = 0;
                dynamicVariables.Insert(index, variable);
            }

            return dynamicVariables[index];
        }
    }
}
