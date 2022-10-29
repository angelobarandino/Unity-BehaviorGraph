using BehaviourGraph.Runtime.Tasks;

namespace BehaviourGraph.Runtime
{
    public interface IBehaviour
    {
        string Name { get; }
        IBehaviourOwner BehaviourOwner { get; }
        BehaviorDataSource DataSource { get; }
        NodeState Evaluate();

        int GetInstanceID();
    }
}
