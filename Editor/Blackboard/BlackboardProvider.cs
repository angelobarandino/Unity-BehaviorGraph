using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorGraph.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.Editor
{
    public class BlackboardProvider
    {
        private readonly IGraphView graphView;
        private readonly BlackboardView blackboardView;

        public BlackboardProvider(IGraphView graphView)
        {
            this.graphView = graphView;
            blackboardView = new BlackboardView();
            blackboardView.SetPropertyChoices(VariablesTypeMap.Select(x => x.Key));
            blackboardView.AddButton.clickable.clicked -= CreateVariable;
            blackboardView.AddButton.clickable.clicked += CreateVariable;
            this.graphView.Add(blackboardView);
        }


        private void CreateVariable()
        {
            var variableName = blackboardView.GetVariableName();
            var propertyType = blackboardView.GetPropertyType();

            if (!string.IsNullOrWhiteSpace(variableName) && propertyType != null)
            {
                if (VariablesTypeMap.ContainsKey(propertyType))
                {
                    var type = VariablesTypeMap[propertyType];
                    var variable = (IBBVariable)Activator.CreateInstance(type);

                    variable.Name = variableName;
                    variable.ReferenceName = variableName;
                    variable.IsReferenced = true;

                    graphView.BehaviorOwner?.Blackboard?.AddVariable(variable);
                    blackboardView.Reset();
                }
            }
        }

        public void LoadVariables()
        {
            blackboardView.IMGUIContainer.onGUIHandler -= OnGUI;
            blackboardView.IMGUIContainer.onGUIHandler += OnGUI;
        }
        private void OnGUI()
        {
            if (graphView.BehaviorOwner != null)
            {
                try
                {
                    var so = ScriptableObject.CreateInstance<BlackboardViewSO>();
                    so.Initialize(graphView.BehaviorOwner.Blackboard);
                    var editor = UnityEditor.Editor.CreateEditor(so);
                    if (editor)
                    {
                        editor.OnInspectorGUI();
                    }
                } 
                catch 
                {
                    //ignore error 'BehaviorOwner' has been destroyed exiting playmode
                }
            }
        }

        private static Dictionary<string, Type> typeMap;
        public static Dictionary<string, Type> VariablesTypeMap
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
