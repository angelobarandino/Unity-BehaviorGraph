using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Conditions
{

    public class TargetCheckDistance : Condition
    {
        [SerializeField]
        private TransformVariable checkTarget;

        [SerializeField]
        private CompareType checkType = CompareType.LessThan;
        
        [SerializeField]
        private FloatVariable distance;

        protected override NodeState OnUpdate()
        {
            var withinDistance = false;

            switch (checkType)
            {
                case CompareType.LessThan:
                    withinDistance = Vector3.Distance(transform.position, checkTarget.Value.position) < distance.Value;
                    break;
                case CompareType.GreaterThan:
                    withinDistance = Vector3.Distance(transform.position, checkTarget.Value.position) > distance.Value;
                    break;
                case CompareType.Equal:
                    withinDistance = Vector3.Distance(transform.position, checkTarget.Value.position) == distance.Value;
                    break;
            }

            return withinDistance ? NodeState.Success : NodeState.Failure;
        }
    }
}
