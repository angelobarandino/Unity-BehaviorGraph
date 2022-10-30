namespace BehaviorGraph.Runtime.Tasks.Actions
{
    public class Idle : Action
    {
        protected override NodeState OnUpdate()
        {
            return NodeState.Running;
        }
    }
}
