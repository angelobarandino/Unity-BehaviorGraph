namespace BehaviorGraph.Runtime.Tasks.Decorators
{
    public class ReturnSuccess : Decorator
    {
        protected override NodeState OnUpdate()
        {
            if (child.Evaluate() == NodeState.Running)
            {
                return NodeState.Running;
            }

            return NodeState.Success;
        }
    }
}
