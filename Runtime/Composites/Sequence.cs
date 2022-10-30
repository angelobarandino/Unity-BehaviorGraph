using System.Linq;

namespace BehaviorGraph.Runtime.Tasks.Composites
{
    public class Sequence : Composite
    {
        protected override NodeState OnChildUpdate(int childIndex, NodeState childState)
        {
            switch (childState)
            {
                case NodeState.Success:
                    ExecuteNextChild();
                    break;
                case NodeState.Failure:
                    return NodeState.Failure;
            }

            if (CheckAllChildStatesEquals(NodeState.Success))
            {
                return NodeState.Success;
            }

            return NodeState.Running;
        }
    }
}
