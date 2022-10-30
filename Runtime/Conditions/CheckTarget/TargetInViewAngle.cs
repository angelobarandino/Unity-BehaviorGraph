using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Conditions
{
    public class TargetInViewAngle : Condition
    {
        [SerializeField]
        private TransformVariable checkTarget;

        [SerializeField]
        private FloatVariable viewAngle = 60f;

        protected override NodeState OnUpdate()
        {
            var targetDirection = checkTarget.Value.position - transform.position;

            var angle= Vector3.Angle(targetDirection, transform.forward);

            return angle < viewAngle.Value ? NodeState.Success : NodeState.Failure;
        }
    }
}
