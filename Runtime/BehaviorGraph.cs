using BehaviorGraph.Runtime.Tasks;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    public abstract class BehaviorGraph : ScriptableObject, IBehaviour
    {
        public string Name => name;

        [SerializeField]
        private BehaviorDataSource dataSource = new();
        public BehaviorDataSource DataSource
        {
            get => dataSource;
        }

        public NodeState Evaluate()
        {
            return dataSource.RootTask.Evaluate();
        }

        public IBehaviourOwner BehaviourOwner { get; set; }

        public IBehaviour Clone()
        {
            return Instantiate(this);
        }
    }
}
