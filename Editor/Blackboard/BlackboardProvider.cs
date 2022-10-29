using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace BehaviourGraph.Editor
{
    public class BlackboardProvider
    {
        private readonly IGraphView graphView;
        private readonly BlackboardView blackboardView;

        public BlackboardProvider(IGraphView graphView)
        {
            this.graphView = graphView;

            blackboardView = new BlackboardView
            {
                AddVariable = OnVariableAdded,
            };

            blackboardView.SetPropertyChoices(TypeMap.Select(x => x.Key));
         
            graphView.Add(blackboardView);
        }

        private void OnVariableAdded()
        {
            var variableName = blackboardView.GetVariableName();
            var propertyType = blackboardView.GetPropertyType();

            if (!string.IsNullOrWhiteSpace(variableName) && propertyType != null)
            {
                if (TypeMap.ContainsKey(propertyType))
                {
                    var variable = (IBBVariable)Activator.CreateInstance(TypeMap[propertyType]);
                    variable.Name = variableName;
                    variable.ReferenceName = variableName;
                    variable.IsReferenced = true;
                    blackboardView.Reset();

                    graphView.BehaviourOwner.Blackboard.AddVariable(variable);
                }
            }
        }

        public void Initialize()
        {
            blackboardView.LoadVariables(graphView.BehaviourOwner.Blackboard);
        }

        private static Dictionary<string, Type> typeMap;
        public static Dictionary<string, Type> TypeMap
        {
            get
            {
                if (typeMap == null)
                {
                    typeMap = new Dictionary<string, Type>();
                    foreach (var item in TypeCache.GetTypesDerivedFrom<IBBVariable>())
                    {
                        if (item.IsAbstract || item.IsGenericType) continue;

                        var keyName = item.Name.Replace("Variable", string.Empty);
                        if (!typeMap.ContainsKey(keyName))
                            typeMap.Add(keyName, item);
                    }
                }

                return typeMap;
            }
        }
    }
}
