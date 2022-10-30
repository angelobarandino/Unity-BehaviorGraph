using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions
{
    public class Wait : Action
    {
        public enum FinishStatus
        {
            Success,
            Failure
        }

        [SerializeField]
        private BoolVariable randomWait = false;
        
        [SerializeField]
        [DependsOn(nameof(randomWait), Value = true)]
        private FloatVariable randomWaitMin = 1;
        
        [SerializeField]
        [DependsOn(nameof(randomWait), Value = true)]
        private FloatVariable randomWaitMax = 1;

        [SerializeField]
        [DependsOn(nameof(randomWait), Value = false)]
        private FloatVariable waitTime = 1f;

        [SerializeField]
        private FinishStatus finishStatus = FinishStatus.Success;

        private float timeToWait;

        protected override void OnStart()
        {
            if (randomWait.Value)
            {
                waitTime.Value = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            }

            timeToWait = Time.time + waitTime.Value;
        }

        protected override NodeState OnUpdate()
        {
            return Time.time > timeToWait 
                ? finishStatus == FinishStatus.Success ? NodeState.Success : NodeState.Failure
                : NodeState.Running;
        }
    }
}
