using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{

    public class BlackboardView : GraphElement
    {
        private bool isNameOnFocus;

        private DropdownField propertyTypeSelect;
        public DropdownField PropertyTypeDropdown
        {
            get => propertyTypeSelect ??=this.Q<DropdownField>("type-select");
        }

        private TextField nameField;
        public TextField NameField
        {
            get => nameField ??= this.Q<TextField>("type-name");
        }

        private Button addButton;
        public Button AddButton
        {
            get => addButton ??= this.Q<Button>("add-button");
        }

        private IMGUIContainer imguiContainer;
        public IMGUIContainer IMGUIContainer
        {
            get => imguiContainer ??= this.Q<IMGUIContainer>("imgui-container");
        }

        public BlackboardView()
        {
            this.LoadVisualTreeAsset("Assets/BehaviorGraph/Editor/Resources/Uxml/Blackboard.uxml");
            this.AddStyleSheet("Assets/BehaviorGraph/Editor/Resources/StyleSheets/Blackboard.uss");
            AddToClassList("blackboard");

            capabilities |= Capabilities.Movable;
            this.AddManipulator(new Dragger { clampToParentEdges = true });

            NameField.RegisterCallback<FocusInEvent>(evt =>
            {
                isNameOnFocus = true;
                if (nameField.value == "Enter Variable Name")
                    nameField.value = string.Empty;
            });

            NameField.RegisterCallback<FocusOutEvent>(evt =>
            {
                isNameOnFocus = false;
                if (string.IsNullOrWhiteSpace(nameField.value))
                    nameField.value = "Enter Variable Name";
            });
        }

        public string GetVariableName() => NameField.value;
        public string GetPropertyType() => PropertyTypeDropdown.value;
        public void SetPropertyChoices(IEnumerable<string> propertyNames)
        {
            PropertyTypeDropdown.value = "(Select)";
            PropertyTypeDropdown.choices = propertyNames.ToList();
        }

        public void Reset()
        {
            if (!isNameOnFocus)
            {
                NameField.value = "Enter Variable Name";
            }

            PropertyTypeDropdown.value = "(Select)";
        }
    }
}
