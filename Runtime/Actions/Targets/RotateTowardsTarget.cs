using BehaviourGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Actions
{
    [TaskCategory("Targets")]
    public class RotateTowardsTarget : Action
    {
        [SerializeField]
        private FloatVariable speed;

        [SerializeField]
        private FloatVariable stopAngle = 0.5f;

        [SerializeField]
        private TransformVariable target;

        protected override NodeState OnUpdate()
        {
            var targetDirection = target.Value.position - transform.position;

            var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, speed.Value * Time.deltaTime, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);

            if (Vector3.Angle(targetDirection, transform.forward) < stopAngle.Value)
            {
                return NodeState.Success;
            }

            return NodeState.Running;
        }
    }
}
