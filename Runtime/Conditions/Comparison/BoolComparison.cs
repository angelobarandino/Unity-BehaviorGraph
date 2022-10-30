using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Conditions
{
    [TaskCategory("Comparison")]
    public class BoolComparison : Condition
    {
        [SerializeField]
        private BoolVariable booleanA;

        [SerializeField]
        private BoolVariable booleanB;

        protected override NodeState OnUpdate()
        {
            return booleanA.Value == booleanB.Value ? NodeState.Success : NodeState.Failure;
        }
    }
}
