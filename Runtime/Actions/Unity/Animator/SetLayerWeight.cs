using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    public class SetLayerWeight : AnimatorAction
    {
        [SerializeField]
        private IntVariable layerIndex;

        [SerializeField]
        private FloatVariable weight;

        protected override NodeState OnExecute()
        {
            Animator.SetLayerWeight(layerIndex.Value, weight.Value);

            return NodeState.Success;
        }
    }
}
