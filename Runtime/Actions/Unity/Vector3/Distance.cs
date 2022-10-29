using BehaviourGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks.Actions.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    public class Distance : Action
    {
        [SerializeField]
        private Vector3Variable target;

        [SerializeField]
        private FloatVariable distance;
        
        protected override NodeState OnUpdate()
        {
            distance.Value = Vector3.Distance(transform.position, target.Value);
            return NodeState.Success;
        }
    }
}
