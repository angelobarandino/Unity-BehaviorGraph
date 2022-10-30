using BehaviorGraph.Runtime.Tasks;

namespace BehaviorGraph.Runtime
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
