using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    public class BlackboardVariableGUI
    {
        private readonly ReorderableList list;
        private readonly Texture2D settingsIcon;
        private readonly SerializedObject serializedObject;
        private readonly IBlackboard blackboard;

        public BlackboardVariableGUI(SerializedObject serializedObject, SerializedProperty allVariablesProperty, IBlackboard blackboard)
        {
            this.blackboard = blackboard;
            this.serializedObject = serializedObject;

            this.settingsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BehaviourGraph/Editor/Resources/Images/cog.png");

            list = new ReorderableList(allVariablesProperty.serializedObject, allVariablesProperty, true, true, false, false);

            list.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Blackboard Variables", new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold
                });
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var variableProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                var bindDataProperty = variableProperty.FindPropertyRelative("bindData");

                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = (rect.width / 2) - 12;

                DrawVariableNameField(new Rect(rect.x, rect.y, rect.width, rect.height), variableProperty);

                var valueRect = new Rect(rect.x + rect.width + 4, rect.y, rect.width, rect.height);
                if (bindDataProperty.GetUnderlyingValue() is BindData bindData && bindData.IsBindActive)
                {
                    var style = new GUIStyle(GUI.skin.label);
                    style.normal.textColor = UnityEngine.ColorUtility.TryParseHtmlString("#89BBFE", out var color) ? color : Color.black;
                    style.fontStyle = FontStyle.Italic;
                    style.padding.left = 8;
                    EditorGUI.LabelField(valueRect, new GUIContent(bindData.DisplayText), style);
                }
                else
                {
                    var valueProperty = variableProperty.FindPropertyRelative("value");
                    EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
                }

                DrawButton(new Rect(valueRect.x + rect.width + 4, rect.y, rect.height, rect.height), variableProperty);
            };
        }

        private void DrawVariableNameField(Rect nameRect, SerializedProperty variableProperty)
        {
            var nameProperty = variableProperty.FindPropertyRelative("name");
            var invalidProperty = variableProperty.FindPropertyRelative("invalid");

            var nameStyle = new GUIStyle(GUI.skin.textField)
            {
                border = new RectOffset(1, 1, 1, 1)
            };

            if (invalidProperty.boolValue)
            {
                var background = new Texture2D(1, 1);
                background.SetPixel(0, 0, UnityEngine.ColorUtility.TryParseHtmlString("#3E000C", out var bgColor) ? bgColor : Color.clear);
                background.Apply();
                nameStyle.normal.background = background;
            }

            EditorGUI.BeginChangeCheck();
            var nameValue = EditorGUI.TextField(nameRect, nameProperty.stringValue, nameStyle);
            EditorGUI.EndChangeCheck();

            nameProperty.stringValue = nameValue;

            if (variableProperty.GetUnderlyingValue() is IBBVariable bbVariable)
            {
                bbVariable.ReferenceName = nameValue;
                bbVariable.Invalid = blackboard.IsVariableNameInValid(nameValue);
            }
        }

        private void DrawButton(Rect rect, SerializedProperty variableProperty)
        {
            var normalStyle = new Texture2D(1, 1);
            normalStyle.SetPixel(0, 0, Color.clear);
            normalStyle.Apply();

            var buttonStyle = new GUIStyle(GUI.skin.box);
            buttonStyle.normal.background = normalStyle;

            if (variableProperty.managedReferenceValue is IBBVariable variable)
            {
                if (GUI.Button(rect, settingsIcon, buttonStyle))
                {
                    BlackboardContextManager.Show(blackboard, variable);
                }
            }
        }

        public void DoLayout()
        {
            if (blackboard == null)
                return;

            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
