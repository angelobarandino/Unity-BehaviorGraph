using BehaviourGraph.Runtime.Tasks;

namespace BehaviourGraph.Runtime
{
    public interface IBehaviour
    {
        string Name { get; }
        IBehaviourOwner BehaviourOwner { get; }
        BehaviourDataSource DataSource { get; }
        NodeState Evaluate(IBlackboard blackboard);

        int GetInstanceID();
    }
}
