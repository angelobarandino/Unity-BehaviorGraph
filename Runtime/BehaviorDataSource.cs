using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourGraph.Runtime.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourGraph.Runtime
{
    [Serializable]
    public class BehaviorDataSource
    {
        [SerializeReference]
        private ITask rootTask;
        public ITask RootTask => rootTask;


        [SerializeReference]
        private List<INode> allNodes = new();
        public List<INode> AllNodes => allNodes;

        public Action<INode, TaskUpdateEvent> OnNodesUpdate { get; set; }
        public Action<Task, TaskUpdateEvent> OnDataSourceUpdate { get; set; }


        public void DuplicateTask(Task task, Vector2 position)
        {
            //var newTask = Activator.CreateInstance(task.GetType()) as Task;
            //newTask.SetPosition(position.x, position.y);

            //allNodes.Add(newTask);

            //OnDataSourceUpdate?.Invoke(newTask, TaskUpdateEvent.Add);
        }

        public void ReplaceTask(Type type, Task oldTask)
        {
            //if (type == oldTask.GetType()) return;

            //var index = allNodes.FindIndex(t => t.Id == oldTask.Id);
            //var newTask = Activator.CreateInstance(type) as Task;
            //newTask.CopyTaskData(oldTask.Data);
            //CopyTaskChildren(oldTask, newTask);
            //allNodes[index] = newTask;

            //// also replace root task
            //if (newTask.Data.IsStart)
            //    rootTask = newTask;

            //OnDataSourceUpdate?.Invoke(newTask, TaskUpdateEvent.Replace);
        }

        public void DecorateTask(Type type, Vector2 position, Task selectedTask)
        {
            //var task = Activator.CreateInstance(type) as Task;
            //if (task is Decorator decorator)
            //{
            //    decorator.SetPosition(position.x, position.y);
            //    decorator.child = selectedTask;

            //    if (selectedTask.Data.Parent != null)
            //    {
            //        var parentTask = allNodes.Find(t => t.Id == selectedTask.Data.Parent);
            //        if (parentTask is Composite compositeParent)
            //        {
            //            var index = compositeParent.GetChildren().IndexOf(selectedTask);
            //            decorator.SetParent(selectedTask.Data.Parent);
            //            compositeParent.AddChild(index, decorator);
            //            compositeParent.RemoveChild(selectedTask);
            //        }

            //        if (parentTask is Decorator decoratorParent)
            //        {
            //            decorator.SetParent(selectedTask.Data.Parent);
            //            decoratorParent = decorator;
            //        }
            //    }

            //    selectedTask.SetParent(decorator.Id);
            //    allNodes.Add(decorator);

            //    OnDataSourceUpdate?.Invoke(decorator, TaskUpdateEvent.Decorate);
            //}
        }

        private void CopyTaskChildren(Task oldTask, Task newTask)
        {
            //if (newTask is Decorator decorator)
            //{
            //    decorator.child = oldTask.GetChildren().FirstOrDefault() as Task;
            //    if (decorator.child != null)
            //        decorator.child.SetParent(decorator.Id);
            //}

            //if (newTask is Composite composite)
            //{
            //    foreach (var child in oldTask.GetChildren())
            //    {
            //        composite.AddChild(child as Task);
            //        child.SetParent(composite.Id);
            //    }
            //}

            //if (newTask.Data.Parent != null)
            //{
            //    foreach (var node in allNodes)
            //    {
            //        // check if node is the parent of newTask
            //        if (newTask.Data.Parent == node.Id)
            //        {
            //            if (node is Composite compositeParent)
            //            {
            //                // get newTask index from parent task
            //                var children = compositeParent.GetChildren();
            //                var index = children.FindIndex(x => x.Id == newTask.Id);
            //                children.RemoveAt(index);
            //                children.Insert(index, newTask);
            //                break;
            //            }

            //            if (node is Decorator decoratorParent)
            //            {
            //                decoratorParent.child = newTask;
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        public void SetChild(Task parent, Task child, int index = 0)
        {
            //switch (parent)
            //{
            //    case Decorator decorator:
            //        decorator.child = child;
            //        break;
            //    case Composite composite:
            //        if (index == -1)
            //            composite.AddChild(0, child);
            //        else if (index == composite.ChildCount)
            //            composite.AddChild(composite.ChildCount, child);
            //        else
            //            composite.AddChild(index, child);
            //        break;
            //}

            //child.SetParent(parent.Id);
        }

        public void ClearChild(Task parent, Task child, int index)
        {
            //switch (parent)
            //{
            //    case Decorator decorator:
            //        if (decorator.child != null && decorator.child.Id == child.Id)
            //        {
            //            decorator.child.SetParent(null);
            //            decorator.child = null;
            //        }
            //        break;
            //    case Composite composite:
            //        composite.RemoveChild(child);
            //        break;
            //}

            //child.SetParent(null);
        }

        public void SetRootTask(INode node)
        {
            if (node is ITask newRooTask)
            {
                rootTask?.SetRootTask(false);
                allNodes.ForEach(node =>
                {
                    if (newRooTask.ParentId == node.Id && node is IParentTask parentTask)
                    {
                        parentTask.RemoveChild(newRooTask);
                    }
                });

                rootTask = newRooTask;
                rootTask.SetRootTask(true);
                rootTask.ParentId = Guid.Empty;
            }

            OnNodesUpdate.Invoke(node, TaskUpdateEvent.Update);
        }

        public void CreateNode(INode node)
        {
            allNodes.Add(node);
            OnNodesUpdate.Invoke(node, TaskUpdateEvent.Add);
        }

        public void RemoveNode(INode node)
        {
            if (rootTask?.Id == node.Id)
                rootTask = null;

            allNodes.Remove(node);
            OnNodesUpdate.Invoke(node, TaskUpdateEvent.Remove);
        }

        public void AddChild(INode parent, INode child)
        {
            if (parent is IParentTask parentTask)
            {
                parentTask.AddChild(child);
                child.ParentId = parentTask.Id;
            }

            OnNodesUpdate.Invoke(parent, TaskUpdateEvent.Update);
        }

        public void RemoveChild(INode parent, INode child)
        {
            if (parent is IParentTask parentTask)
            {
                parentTask.RemoveChild(child);
                child.ParentId = Guid.Empty;
            }

            OnNodesUpdate.Invoke(parent, TaskUpdateEvent.Update);
        }
    }
}
