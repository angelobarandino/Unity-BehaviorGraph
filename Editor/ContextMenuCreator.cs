//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BehaviorGraph.Runtime;
//using BehaviorGraph.Runtime.Attributes;
//using BehaviorGraph.Runtime.Tasks;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UIElements;
//using TaskAction = BehaviorGraph.Runtime.Tasks.Action;

//namespace BehaviorGraph.Editor
//{
//    public static class BehaviourTreeExtension
//    {
//        public static void CreateTaskInstance(this IBehaviour behaviour, Type type, Vector2 position)
//        {
//            behaviour.DataSource.CreateTask((Task)Activator.CreateInstance(type), position);
//        }
//    }

//    public static class ContextMenuCreator
//    {
//        public static void AddTaskMenus(ContextualMenuPopulateEvent evt, IBehaviour behaviour, Vector2 mousePosition)
//        {
//            CreateMenusFor<TaskAction>(evt, "Add Task/Actions", type => behaviour.CreateTaskInstance(type, mousePosition));
//            CreateMenusFor<Composite>(evt, "Add Task/Composites", type => behaviour.CreateTaskInstance(type, mousePosition));
//            CreateMenusFor<Condition>(evt, "Add Task/Conditions", type => behaviour.CreateTaskInstance(type, mousePosition));
//            CreateMenusFor<Decorator>(evt, "Add Task/Decorators", type => behaviour.CreateTaskInstance(type, mousePosition));
//        }

//        public static void ReplaceMenus(ContextualMenuPopulateEvent evt, IBehaviour behaviour, NodeView nodeView)
//        {
//            CreateMenusFor<TaskAction>(evt, "Replace", type => behaviour.DataSource.ReplaceTask(type, nodeView.task));
//            CreateMenusFor<Composite>(evt, "Replace", type => behaviour.DataSource.ReplaceTask(type, nodeView.task));
//            CreateMenusFor<Decorator>(evt, "Replace", type => behaviour.DataSource.ReplaceTask(type, nodeView.task));
//        }

//        public static void DecorateMenus(ContextualMenuPopulateEvent evt, IBehaviour behaviour, NodeView nodeView)
//        {
//            CreateMenusFor<Decorator>(evt, "Decorate", type =>
//            {
//                var position = nodeView.GetPosition();
//                behaviour.DataSource.DecorateTask(type, new Vector2(position.xMin, position.yMin), nodeView.task);

//                position.yMin += 90;
//                nodeView.SetPosition(position);
//            });
//        }

//        private static void CreateMenusFor<T>(ContextualMenuPopulateEvent evt, string group, Action<Type> action)
//        {
//            foreach (var type in TypeCache.GetTypesDerivedFrom<T>())
//            {
//                var category = group;
//                var categoryAttr = type.GetAttribute<TaskCategory>();
//                if (categoryAttr != null)
//                    category = $"{category}/{categoryAttr.Category}";

//                if (type.IsAbstract)
//                {
//                    continue;
//                }

//                var actionName = string.IsNullOrWhiteSpace(categoryAttr?.Name) ? type.Name.ToFriendlyName() : categoryAttr.Name;

//                evt.menu.AppendAction($"{category}/{actionName}", (a) => action.Invoke(type));
//            }
//        }

//        public static void PasteMenu(ContextualMenuPopulateEvent evt, IBehaviour behaviour, Vector2 mousePosition)
//        {
//            if (CopyTaskHelper.HasTaskCopied())
//            {
//                evt.menu.AppendAction("Paste", a =>
//                {
//                    foreach (var task in CopyTaskHelper.GetCopiedTasks())
//                    {
//                        behaviour.DataSource.DuplicateTask(task, mousePosition);
//                        mousePosition.x += 50;
//                        mousePosition.y += 20;
//                    }

//                    CopyTaskHelper.Clear();
//                });
//            }
//        }

//        public static void CopyAndDeleteMenus(ContextualMenuPopulateEvent evt, IGraphView graphView)
//        {
//            evt.menu.AppendAction("Delete", a =>
//            {
//                graphView.DeleteElements(graphView.GetSelectedElements());
//            });

//            evt.menu.AppendAction("Copy", a => 
//            {
//                CopyTaskHelper.Clear();

//                graphView.GetSelectedElements().ForEach(element =>
//                {
//                    if (element is NodeView nodeView)
//                    {
//                        CopyTaskHelper.Copy(nodeView.task);
//                    }
//                });
//            });
//        }
//    }

//    public class CopyTaskHelper
//    {
//        public static List<Task> copiedTask = new List<Task>();

//        public static bool HasTaskCopied() => copiedTask.Any();

//        public static List<Task> GetCopiedTasks() => copiedTask;

//        public static void Copy(Task task)
//        {
//            if (copiedTask.Any(t => t.Id == task.Id)) return;

//            copiedTask.Add(task);
//        }

//        public static void Clear()
//        {
//            copiedTask.Clear();
//        }
//    }
//}
