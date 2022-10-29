using System;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks
{
    [Serializable]
    public class SubTree : Task, ISubTree
    {
        [SerializeField]
        private BehaviourSubTree subTree;

        public SubTree()
        {
            Name = "Behaviour SubTree";
        }

        public BehaviourSubTree BehaviourSubTree
        {
            get => subTree;
            set => subTree = value; 
        }

        protected override NodeState OnUpdate()
        {
            return subTree.DataSource.RootTask.Evaluate(Blackboard);
        }

        public override void Initialize(IBehaviourOwner owner)
        {
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