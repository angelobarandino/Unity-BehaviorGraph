using BehaviorGraph.Runtime.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorGraph.Runtime.Tasks.Actions.UnityNavMeshAgent
{
    [TaskCategory("Unity/NavMeshAgent")]
    public class SetDestination : Action
    {
        [SerializeField]
        private bool useSelf = true;

        [SerializeField]
        [DependsOn(nameof(useSelf), Value = false)]
        private GameObjectVariable other;

        [SerializeField]
        private Vector3Variable destination;

        private NavMeshAgent agent;
        protected override void OnStart()
        {
            agent = useSelf ? GameObject.GetComponent<NavMeshAgent>() : other.Value.GetComponent<NavMeshAgent>();
        }

        protected override NodeState OnUpdate()
        {
            return agent.SetDestination(destination.Value) ? NodeState.Success : NodeState.Failure;
        }
    }
}
