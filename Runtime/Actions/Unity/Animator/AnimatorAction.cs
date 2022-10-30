using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions.UnityAnimator
{
    public abstract class AnimatorAction : Action
    {
        [SerializeField]
        [FieldOrder(Order = 0)]
        [FieldLabel("Animator")]
        private bool useSelf = true;

        [SerializeField]
        [FieldOrder(Order = 1)]
        [DependsOn(nameof(useSelf), Value = false)]
        private GameObjectVariable otherGameObject;

        private Animator animator;
        protected Animator Animator
        {
            get => animator;
        }

        protected override void OnStart()
        {
            animator = useSelf ? GameObject.GetComponent<Animator>() : otherGameObject.Value.GetComponent<Animator>();
        }

        protected override NodeState OnUpdate()
        {
            if (Animator == null)
            {
                return NodeState.Failure;
            }

            return OnExecute();
        }

        protected abstract NodeState OnExecute();
    }
}
