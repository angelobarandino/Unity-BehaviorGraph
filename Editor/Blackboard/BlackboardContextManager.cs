using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    public static class BlackboardContextManager
    {
        public class MenuItem
        {
            public MenuItem() { }
            public MenuItem(string text)
            {
                Text = text;
            }

            public bool On { get; set; }
            public string Text { get; set; }
            public bool IsSeparator { get; set; }
            public System.Action OnSelect { get; set; }

            public static MenuItem Separator() => new() { IsSeparator = true };
        }

        public static void Show(IBlackboard blackboard, IBBVariable variable)
        {
            var menus = new List<MenuItem>();

            menus.AddRange(GetAllComponents($"Bind Self ({blackboard.GameObject.name})", blackboard.GameObject, variable));

            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                if (gameObject == blackboard.GameObject) continue;

                menus.AddRange(GetAllComponents($"Bind (From Scene)/{gameObject.name}", gameObject, variable));
            }

            menus.Add(MenuItem.Separator());
            menus.Add(new MenuItem("Un Bind")
            {
                OnSelect = variable.IsBindActive() ? () => variable.Bind(null) : null
            });
            menus.AddRange(GetAllVariableTypes(blackboard, variable));
            menus.Add(new MenuItem("Delete")
            {
                OnSelect = () => blackboard.RemoveVariable(variable)
            });

            var genericMenu = new GenericMenu();
            CreateMenuItems(menus, genericMenu);
            genericMenu.ShowAsContext();
        }

        private static IEnumerable<MenuItem> GetAllVariableTypes(IBlackboard blackboard, IBBVariable variable)
        {
            var menus = new List<MenuItem>();

            foreach (var item in TypeCache.GetTypesDerivedFrom<IBBVariable>())
            {
                if (item.IsAbstract) continue;

                menus.Add(new MenuItem($"Change Type/{item.Name.Replace("Variable", string.Empty)}")
                {
                    OnSelect = () =>
                    {
                        var newVar = System.Activator.CreateInstance(item) as IBBVariable;
                        newVar.Name = variable.Name;
                        blackboard.ChangeVariableType(newVar, variable);
                    }
                });
            }

            return menus;
        }

        private static void CreateMenuItems(List<MenuItem> menus, GenericMenu genericMenu)
        {
            foreach (var item in menus)
            {
                if (item.IsSeparator)
                {
                    genericMenu.AddSeparator(string.Empty);
                    continue;
                }

                genericMenu.AddItem(item);
            }
        }

        private static void AddItem(this GenericMenu menu, MenuItem item)
        {
            menu.AddItem(
                CreateLabel(item.Text), 
                item.On, 
                item.OnSelect != null ? () => item.OnSelect() : null
            );
        }

        private static GUIContent CreateLabel(string text)
        {
            return new GUIContent(text);
        }

        public static List<MenuItem> GetAllComponents(string rootPath, GameObject component, IBBVariable variable)
        {
            var menus = new List<MenuItem>();

            foreach (var comp in component.GetComponents(typeof(Component)))
            {
                var path = $"{rootPath}/{comp.GetType().Name}";
                
                GetComponentProperties(comp).ForEach(property =>
                {
                    if (property.PropertyType.IsAssignableFrom(variable.GetVariableType()))
                    {
                        menus.Add(new MenuItem($"{path}/{property.Name}")
                        {
                            OnSelect = () =>
                            {
                                variable.Bind(new BindData
                                {
                                    TargetName = component.name,
                                    GameObject = comp.gameObject,
                                    PropertyName = property.Name,
                                    ComponentName = comp.GetType().Name,
                                });
                            }
                        });
                    }
                });
            }

            return menus;
        }

        private static List<PropertyInfo> GetComponentProperties(Component component)
        {
            var properties = component.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => p.PropertyType != typeof(Component))
                .ToList();

            return properties;
        }
    }
}
