namespace BehaviorGraph.Runtime.Tasks
{
    public abstract class Decorator : ParentTask
    {
        public Task child => (Task)children[0];
    }
}