using System.Collections.Generic;

namespace BehaviorGraph.Runtime
{
    public interface IParentTask : INode
    {
        List<ITask> GetChildren();

        void AddChild(INode child);
        void RemoveChild(INode child);
        void ClearChildren();
        void SortChildren();
    }
}
