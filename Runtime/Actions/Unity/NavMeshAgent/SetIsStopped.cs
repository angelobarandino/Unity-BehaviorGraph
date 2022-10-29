using BehaviourGraph.Runtime.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourGraph.Runtime.Tasks.Actions.UnityNavMeshAgent
{
    [TaskCategory("Unity/NavMeshAgent")]
    public class SetIsStopped : Action
    {
        [SerializeField]
        private bool useSelf = true;

        [SerializeField]
        [DependsOn(nameof(useSelf), Value = false)]
        private GameObjectVariable other;

        [SerializeField]
        private BoolVariable isStopped = true;


        private NavMeshAgent agent;
        protected override void OnStart()
        {
            agent = useSelf ? GameObject.GetComponent<NavMeshAgent>() : other.Value.GetComponent<NavMeshAgent>();
        }

        protected override NodeState OnUpdate()
        {
            agent.isStopped = isStopped.Value;
            return NodeState.Success;
        }
    }
}
