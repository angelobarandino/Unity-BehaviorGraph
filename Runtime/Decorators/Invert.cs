namespace BehaviourGraph.Runtime.Tasks.Decorators
{
    public class Invert : Decorator
    {
        protected override NodeState OnUpdate()
        {
            var state = child.Evaluate();
            if (state == NodeState.Running)
                return state;

            return state == NodeState.Success ? NodeState.Failure : NodeState.Success;
        }
    }
}
