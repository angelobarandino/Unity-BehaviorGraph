using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    public class SetTrigger : AnimatorAction
    {
        [SerializeField]
        private StringVariable parameterName;

        private int animId;
        protected override void OnStart()
        {
            base.OnStart();

            animId = Animator.StringToHash(parameterName.Value);
        }

        protected override NodeState OnExecute()
        {
            Animator.SetTrigger(animId);
            return NodeState.Success;
        }
    }
}
