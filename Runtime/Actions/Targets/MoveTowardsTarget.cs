using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions
{
    [TaskCategory("Targets")]
    public class MoveTowardsTarget : Action
    {
        [SerializeField]
        private FloatVariable speed = 1f;
        
        [SerializeField]
        private FloatVariable stopDistance = 0.05f;
        
        [SerializeField]
        private TransformVariable target;

        protected override NodeState OnUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.Value.position, speed.Value * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.Value.position) < stopDistance.Value)
            {
                return NodeState.Success;
            }

            return NodeState.Running;
        }
    }
}
