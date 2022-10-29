namespace BehaviourGraph.Runtime.Tasks.Decorators
{
    public class ReturnFailure : Decorator
    {
        protected override NodeState OnUpdate()
        {
            if (child.Evaluate() == NodeState.Running)
            {
                return NodeState.Running;
            }

            return NodeState.Failure;
        }
    }
}
