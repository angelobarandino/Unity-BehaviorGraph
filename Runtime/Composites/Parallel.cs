using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Composites
{
    public class Parallel : Composite
    {
        public enum ParallelPolicy
        {
            FirstSuccess,
            FirstFailure,
        }

        [SerializeField]
        private ParallelPolicy policy = ParallelPolicy.FirstFailure;

        protected override void OnChildStart(int childIndex)
        {
            if (children[childIndex].State == NodeState.Ready)
                return;

            if (children[childIndex].State == NodeState.Running)
                return;

            ExecuteNextChild();
        }

        protected override NodeState OnChildUpdate(int currentIndex, NodeState childState)
        {
            switch (policy)
            {
                case ParallelPolicy.FirstSuccess:
                    if (CheckAnyChildStatesEquals(NodeState.Success))
                    {
                        foreach (var childTask in children)
                            if (childTask.State == NodeState.Running)
                                childTask.Interupt(NodeState.Success);

                        return NodeState.Success;
                    }

                    if (CheckAllChildStatesEquals(NodeState.Failure))
                        return NodeState.Failure;
                    break;

                case ParallelPolicy.FirstFailure:
                    if (CheckAnyChildStatesEquals(NodeState.Failure))
                    {
                        foreach (var childTask in children)
                            if (childTask.State == NodeState.Running)
                                childTask.Interupt(NodeState.Failure);

                        return NodeState.Failure;
                    }

                    if (CheckAllChildStatesEquals(NodeState.Success))
                        return NodeState.Success;
                    break;
            }

            ExecuteNextChild();

            return NodeState.Running;
        }
    }
}
