using System.Collections.Generic;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks
{
    public abstract class ParentTask : Task, IParentTask
    {
        [SerializeReference]
        protected List<ITask> children;

        public override List<ITask> GetChildren()
        {
            return children;
        }

        public void AddChild(INode child)
        {
            children.Add((ITask)child);

            SortChildren();
        }

        public void RemoveChild(INode child)
        {
            children.Remove((ITask)child);

            SortChildren();
        }

        public void ClearChildren()
        {
            children.Clear();
        }

        public void SortChildren()
        {
            children.Sort((leftNode, rightNode) =>
            {
                var left = leftNode.GetPosition();
                var right = rightNode.GetPosition();
                return left.x < right.x ? -1 : 1;
            });
        }
    }
}