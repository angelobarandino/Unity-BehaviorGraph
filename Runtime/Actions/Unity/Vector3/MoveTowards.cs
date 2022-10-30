using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    public class MoveTowards : Action
    {
        [SerializeField]
        private FloatVariable speed = 1f;

        [SerializeField]
        private Vector3Variable target;

        protected override NodeState OnUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.Value, speed.Value * Time.deltaTime);
            return NodeState.Success;
        }
    }
}
