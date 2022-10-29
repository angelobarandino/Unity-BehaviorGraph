using BehaviourGraph.Runtime.Tasks;

namespace BehaviourGraph.Runtime
{
    public interface ISubTree : ITask
    {
        BehaviorSubTree BehaviourSubTree { get; set; }
    }
}