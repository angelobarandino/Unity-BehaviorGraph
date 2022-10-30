using System;
using BehaviorGraph.Runtime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.Editor
{
    [CustomPropertyDrawer(typeof(BBVariable), true)]
    public class BBVariablePropertyDrawer : PropertyDrawer
    {
        private static Texture2D activeImage;
        private Texture2D ActiveImage
        {
            get
            {
                if (activeImage == null)
                {
                    activeImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BehaviorGraph/Editor/Resources/Images/round.png");
                }

                return activeImage;
            }
        }

        private static Texture2D inActiveImage;

        private Texture2D InActiveImage
        {
            get
            {
                if (inActiveImage == null)
                {
                    inActiveImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BehaviorGraph/Editor/Resources/Images/round-inactive.png");
                }

                return inActiveImage;
            }
        }

        private NodeInspectorSO nodeInspectorSO;
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            nodeInspectorSO = property.serializedObject.targetObject as NodeInspectorSO;

            if (property.GetUnderlyingValue() is IBBVariable variable)
            {
                var nameProperty = property.FindPropertyRelative("name");
                var valueProperty = property.FindPropertyRelative("value");

                if (variable.IsReferenced)
                {
                    EditorGUI.BeginProperty(pos, label, valueProperty);

                    var position = new Rect(pos.x, pos.y, EditorGUIUtility.labelWidth, pos.height);
                    EditorGUI.PrefixLabel(position, label);

                    position.x += position.width + 2;
                    position.width = pos.width - position.x - pos.height;
                    
                    if (variable.IsDynamic)
                    {
                        position.width -= 3;
                        position.height -= 2;
                        position.y += 1;
                        position.x += 1;
                        
                        var style = new GUIStyle(GUI.skin.textField);
                        EditorGUI.DrawRect(position, Color.gray);

                        EditorGUI.BeginChangeCheck();
                        var value = EditorGUI.TextField(position, nameProperty.stringValue, GUIStyle.none);
                        if (EditorGUI.EndChangeCheck())
                        {
                            nameProperty.stringValue = value;
                        }
                    }
                    else
                    {
                        if (EditorGUI.DropdownButton(position, new GUIContent(variable.ReferenceName), FocusType.Keyboard))
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("[NONE]"), false, () => 
                            {
                                variable.ReferenceName = null;
                            });
                            menu.AddItem(new GUIContent("[DYNAMIC]"), false, () => 
                            {
                                variable.IsDynamic = true;
                            });

                            menu.AddSeparator(string.Empty);
                            GetBlackboardVariables(variable.GetVariableType(), referenceName =>
                            {
                                var on = variable.ReferenceName == referenceName;
                                menu.AddItem(new GUIContent(referenceName), on, () =>
                                {
                                    variable.ReferenceName = referenceName;
                                });
                            });
                            menu.DropDown(position);
                        }
                    }

                    EditorGUI.EndProperty();
                }
                else
                {
                    var position = new Rect(pos.x, pos.y, pos.width - (pos.height + 2), pos.height);
                    EditorGUI.PropertyField(position, valueProperty, label);
                }

                if (ToggleButton(pos, variable.IsReferenced ? ActiveImage : InActiveImage))
                {
                    variable.ToggleIsReference();
                    variable.ReferenceName = null;
                    variable.Name = null;
                }
            }
        }

        private bool ToggleButton(Rect pos, Texture2D imageTexture)
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(6, 6, 6, 6)
            };

            var position = new Rect(
                pos.x + (pos.width - pos.height), 
                pos.y, 
                pos.height, 
                pos.height
            );

            return GUI.Button(position, imageTexture, style);
        }

        private void GetBlackboardVariables(Type type, Action<string> callback)
        {
            if (nodeInspectorSO == null) return;

            var variables = nodeInspectorSO.Provider.GetBlackboardVariables();

            variables.ForEach(variable =>
            {
                if (type.Equals(variable.GetVariableType()))
                {
                    callback(variable.Name);
                }
            });
        }
    }
}
