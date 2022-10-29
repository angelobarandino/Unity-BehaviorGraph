using BehaviourGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviourGraph.Runtime
{
    public abstract class BehaviourGraph : ScriptableObject, IBehaviour
    {
        public string Name => name;

        [SerializeField]
        private BehaviourDataSource dataSource;
        public BehaviourDataSource DataSource
        {
            get => dataSource;
        }

        public NodeState Evaluate(IBlackboard blackboard)
        {
            return dataSource.RootTask.Evaluate(blackboard);
        }

        public IBehaviourOwner BehaviourOwner { get; set; }

    }
}
