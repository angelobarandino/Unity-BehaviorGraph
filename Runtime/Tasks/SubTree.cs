using System;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks
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
            subTree = (BehaviorSubTree)subTree.Clone();

            base.Initialize(owner);
            foreach (var node in subTree.DataSource.AllNodes)
            {
                (node as ITask).Initialize(owner);
            }
        }
    }

    public class ActionTask : Task
    {
        public ActionTask()
        {
            Name = "Action Task";
        }
    }
}