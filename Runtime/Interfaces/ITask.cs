using System;
using System.Collections.Generic;
using BehaviourGraph.Runtime.Tasks;

namespace BehaviourGraph.Runtime
{
    public interface ITask : INode, ICloneable
    {
        bool IsRootTask { get; }
        IBehaviourOwner Owner { get; }
        IBlackboard Blackboard { get; }

        NodeState State { get; }

        NodeState Evaluate();

        List<ITask> GetChildren();

        void SetRootTask(bool rootTask);

        void Interupt(NodeState interuptState);
        void OverrideState(NodeState newState);
        
        void Initialize(IBehaviourOwner owner);
    }
}