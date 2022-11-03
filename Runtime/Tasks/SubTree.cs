using System;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks
{
    [Serializable]
    public class SubTree : Task, ISubTree
    {
        [SerializeField]
        private BehaviorSubTree subTree;

        public SubTree()
        {
            Name = "Behaviour SubTree";
        }

        public BehaviorSubTree BehaviourSubTree
        {
            get => subTree;
            set => subTree = value; 
        }

        protected override NodeState OnUpdate()
        {
            return subTree.DataSource.RootTask.Evaluate();
        }

        public override void Initialize(IBehaviourOwner owner)
        {
            base.Initialize(owner);

            subTree = (BehaviorSubTree)subTree.Clone();
            foreach (var node in subTree.DataSource.AllNodes)
            {
                if (node is ITask task)
                {
                    task.Initialize(owner);
                    task.OnBehaviorStart();
                }
            }
        }

        public override string GetInfo()
        {
            return IsNullOrDestroyed(BehaviourSubTree) ? "Behaviour SubTree" : BehaviourSubTree.Name;
        }
    }
}