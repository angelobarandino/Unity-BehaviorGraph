using System.Collections.Generic;
using BehaviourGraph.Runtime;

namespace BehaviourGraph
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
