using System.Collections;
using BehaviourGraph.Runtime.Attributes;
using CityBuilder.AI.Tasks;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Actions.UnityNavMeshAgent
{

    [TaskCategory("Unity/NavMeshAgent")]
    public class MoveTowardsTarget : NavMeshAgentAction
    {
        [SerializeField]
        private TransformVariable target;
        private Coroutine coroutine;
        private bool isComplete;

        protected override void OnStart()
        {
            base.OnStart();

            isComplete = false;
            agent.isStopped = false;
            coroutine = StartCoroutine(MoveAgent(target.Value.position));
        }

        protected override void OnStop()
        {
            StopCoroutine(coroutine);
        }

        protected override void OnInterupt(NodeState interuptState)
        {
            agent.isStopped = true;
        }

        protected override NodeState OnUpdate()
        {
            return isComplete ? NodeState.Success : NodeState.Running;
        }

        public IEnumerator MoveAgent(Vector3 destination)
        {
            yield return new WaitForAgentToComplete(agent, destination);
            isComplete = true;
        }
    }
}
