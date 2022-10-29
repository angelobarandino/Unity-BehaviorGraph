using System;
using System.Collections.Generic;
using BehaviourGraph.Runtime.Tasks;
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


        public Action<TaskUpdateEvent> OnBehaviorBeforeUpdate { get; set; }
        public Action<INode, TaskUpdateEvent> OnBehaviorAfterUpdate { get; set; }
        public Action<Task, TaskUpdateEvent> OnDataSourceUpdate { get; set; }

        public void SetRootTask(INode node)
        {
            OnBehaviorBeforeUpdate?.Invoke(TaskUpdateEvent.Update);

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

            OnBehaviorAfterUpdate?.Invoke(node, TaskUpdateEvent.Update);
        }

        public void CreateNode(INode node, bool isCopied = false)
        {
            var updateEvent = isCopied ? TaskUpdateEvent.CopyPaste : TaskUpdateEvent.Create;

            OnBehaviorBeforeUpdate?.Invoke(updateEvent);
            if (rootTask == null)
            {
                rootTask = (ITask)node;
                rootTask?.SetRootTask(true);
            }

            allNodes.Add(node);
            OnBehaviorAfterUpdate?.Invoke(node, updateEvent);
        }

        public void RemoveNode(INode node)
        {
            OnBehaviorBeforeUpdate?.Invoke(TaskUpdateEvent.Remove);

            if (rootTask?.Id == node.Id)
                rootTask = null;

            allNodes.Remove(node);
            OnBehaviorAfterUpdate?.Invoke(node, TaskUpdateEvent.Remove);
        }

        public void AddChild(INode parent, INode child)
        {
            OnBehaviorBeforeUpdate?.Invoke(TaskUpdateEvent.Update);

            if (parent is IParentTask parentTask)
            {
                parentTask.AddChild(child);
                child.ParentId = parentTask.Id;
            }

            OnBehaviorAfterUpdate?.Invoke(parent, TaskUpdateEvent.Update);
        }

        public void RemoveChild(INode parent, INode child)
        {
            OnBehaviorBeforeUpdate?.Invoke(TaskUpdateEvent.Update);

            if (parent is IParentTask parentTask)
            {
                parentTask.RemoveChild(child);
                child.ParentId = Guid.Empty;
            }

            OnBehaviorAfterUpdate?.Invoke(parent, TaskUpdateEvent.Update);
        }

        public INode FindNodeById(string nodeId)
        {
            return allNodes.Find(node => node.Id == nodeId);
        }

        public void DeleteNodes(List<INode> nodesToDelete)
        {
            nodesToDelete.ForEach(node => allNodes.Remove(node));
        }
    }
}
