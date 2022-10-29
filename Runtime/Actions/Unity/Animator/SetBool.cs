using BehaviourGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Actions.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    public class SetBool : AnimatorAction
    {

        [SerializeField]
        private StringVariable parameterName;

        [SerializeField]
        private BoolVariable boolValue;

        private int hashId;

        protected override NodeState OnExecute()
        {
            hashId = Animator.StringToHash(parameterName.Value);

            Animator.SetBool(hashId, boolValue.Value);

            return NodeState.Success;
        }
    }
}
