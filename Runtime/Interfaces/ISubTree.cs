using BehaviorGraph.Runtime.Tasks;

namespace BehaviorGraph.Runtime
{
    public interface ISubTree : ITask
    {
        BehaviorSubTree BehaviourSubTree { get; set; }
    }
}