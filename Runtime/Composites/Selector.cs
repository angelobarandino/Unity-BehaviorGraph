using System.Linq;
using BehaviorGraph.Runtime.Attributes;

namespace BehaviorGraph.Runtime.Tasks.Composites
{
    [TaskIcon("Assets/BehaviorGraph/Editor/Resources/Images/selector.png")]
    public class Selector : Composite
    {
        protected override NodeState OnChildUpdate(int childIndex, NodeState childState)
        {
            switch (childState)
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Failure:
                    ExecuteNextChild();
                    break;
            }

            if (CheckAllChildStatesEquals(NodeState.Failure))
            {
                return NodeState.Failure;
            }

            return NodeState.Running;
        }

        protected override NodeState ReactiveInteruptionState()
        {
            return NodeState.Success;
        }
    }
}
