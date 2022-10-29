using BehaviourGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviourGraph.Runtime
{
    public abstract class BehaviorGraph : ScriptableObject, IBehaviour
    {
        public string Name => name;

        [SerializeField]
        private BehaviorDataSource dataSource;
        public BehaviorDataSource DataSource
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
