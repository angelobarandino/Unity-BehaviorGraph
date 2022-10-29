using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public class BlackboardViewSO : ScriptableObject
    {
        [SerializeReference]
        public List<IBBVariable> allVariables = new List<IBBVariable>();

        public Func<IBlackboard> GetBlackboard;
    }

    public class BlackboardView : GraphElement
    {
        private readonly Button _addButton;
        private readonly TextField _nameField;
        private readonly DropdownField _propertyTypeSelect;
        private readonly IMGUIContainer _imguiContainer;
        
        private bool isNameOnFocus;
        private BlackboardViewSO blackboardViewSO;

        public Action AddVariable { get; set; }

        public BlackboardView()
        {
            this.LoadVisualTreeAsset("Assets/BehaviourGraph/Editor/Resources/Uxml/Blackboard.uxml");
            this.AddStyleSheet("Assets/BehaviourGraph/Editor/Resources/StyleSheets/Blackboard.uss");
            AddToClassList("blackboard");

            capabilities |= Capabilities.Movable;
            this.AddManipulator(new Dragger { clampToParentEdges = true });

            var main = this;

            _propertyTypeSelect = main.Q<DropdownField>("type-select");
            _nameField = main.Q<TextField>("type-name");
            _nameField.RegisterCallback<FocusInEvent>(evt =>
            {
                isNameOnFocus = true;
                if (_nameField.value == "Enter Variable Name")
                    _nameField.value = string.Empty;
            });
            _nameField.RegisterCallback<FocusOutEvent>(evt =>
            {
                isNameOnFocus = false;
                if (string.IsNullOrWhiteSpace(_nameField.value))
                    _nameField.value = "Enter Variable Name";
            });

            _addButton = main.Q<Button>("add-button");
            _addButton.clickable.clicked += () =>
            {
                AddVariable?.Invoke();
            };

            _imguiContainer = main.Q<IMGUIContainer>("imgui-container");
            _imguiContainer.onGUIHandler = () =>
            {
                if (blackboardViewSO != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(blackboardViewSO);
                    if (editor)
                    {
                        editor.OnInspectorGUI();
                    }
                }
            };
        }

        public string GetVariableName()
        {
            return _nameField.value;
        }

        public string GetPropertyType()
        {
            return _propertyTypeSelect.value;
        }

        public void SetPropertyChoices(IEnumerable<string> propertyNames)
        {
            _propertyTypeSelect.value = "(Select)";

            _propertyTypeSelect.choices = propertyNames.ToList();
        }

        public void Reset()
        {
            if (!isNameOnFocus)
            {
                _nameField.value = "Enter Variable Name";
            }

            _propertyTypeSelect.value = "(Select)";
        }

        public void LoadVariables(IBlackboard blackboard)
        {
            blackboardViewSO = ScriptableObject.CreateInstance<BlackboardViewSO>();
            blackboardViewSO.allVariables = blackboard.AllVariables;
            blackboardViewSO.GetBlackboard = () =>
            {
                return blackboard;
            };
        }
    }
}
