using BehaviourGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Conditions
{
    [TaskCategory("Comparison")]
    public class FloatComparison : Condition
    {
        [SerializeField]
        private FloatVariable float1;

        [SerializeField]
        private CompareType checkType = CompareType.Equal;

        [SerializeField]
        private FloatVariable float2;

        protected override NodeState OnUpdate()
        {
            var compareResult = true;

            switch (checkType)
            {
                case CompareType.LessThan:
                    compareResult = float1.Value < float2.Value;
                    break;
                case CompareType.GreaterThan:
                    compareResult = float1.Value > float2.Value;
                    break;
                case CompareType.Equal:
                    compareResult = float1.Value == float2.Value;
                    break;
            }

            return compareResult ? NodeState.Success : NodeState.Failure;
        }
    }
}
