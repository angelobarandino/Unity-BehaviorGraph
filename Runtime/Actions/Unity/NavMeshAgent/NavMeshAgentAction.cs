using BehaviorGraph.Runtime.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorGraph.Runtime.Tasks.Actions.UnityNavMeshAgent
{
    public abstract class NavMeshAgentAction : Action
    {
        [SerializeField]
        [FieldOrder(Order = 0)]
        [FieldLabel("NavMeshAgent")]
        private bool useSelf = true;

        [SerializeField]
        [FieldOrder(Order = 1)]
        [DependsOn(nameof(useSelf), Value = false)]
        public GameObjectVariable other;

        protected NavMeshAgent agent;

        protected override void OnStart()
        {
            agent = (useSelf ? GameObject : other.Value).GetComponent<NavMeshAgent>();
        }

        protected bool HasReachDestination(Vector3 destination, float distance)
        {
            var agentDistance = Vector3.Distance(transform.position, destination);

            var withinDistance = agentDistance - agent.stoppingDistance <= distance;

            return withinDistance;
        }
    }
}
