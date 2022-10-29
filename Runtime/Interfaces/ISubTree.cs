using BehaviourGraph.Runtime.Tasks;

namespace BehaviourGraph.Runtime
{
    public interface ISubTree : ITask
    {
        BehaviourSubTree BehaviourSubTree { get; set; }
    }
}