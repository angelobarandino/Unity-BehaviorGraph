using BehaviorGraph.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions
{
    [TaskIcon("Assets/BehaviorGraph/Editor/Resources/Images/stopwatch.png")]
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

        private float timeRemaining;

        protected override void OnStart()
        {
            if (randomWait.Value)
            {
                waitTime.Value = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            }

            timeRemaining = waitTime.Value;
        }

        protected override NodeState OnUpdate()
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining > 0) return NodeState.Running;
            timeRemaining = 0;

            return finishStatus == FinishStatus.Success ? NodeState.Success : NodeState.Failure;
        }

        public override string GetInfo()
        {
            return randomWait.Value && !Application.isPlaying ? $"Wait [{randomWaitMin.Value}, {randomWaitMax.Value}] sec" : $"Wait {waitTime.Value:0.#} sec";
        }
    }
}
