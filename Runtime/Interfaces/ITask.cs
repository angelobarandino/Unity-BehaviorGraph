using System;
using System.Collections.Generic;
using BehaviorGraph.Runtime.Tasks;

namespace BehaviorGraph.Runtime
{
    public interface ITask : INode, ICloneable
    {
        bool IsRootTask { get; }
        IBehaviourOwner Owner { get; }
        IBlackboard Blackboard { get; }

        NodeState Evaluate();
        string GetInfo();
        List<ITask> GetChildren();

        void SetRootTask(bool rootTask);
        void Interupt(NodeState interuptState);
        void OverrideState(NodeState newState);
        void Initialize(IBehaviourOwner owner);
    }
}