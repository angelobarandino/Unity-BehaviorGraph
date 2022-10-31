using BehaviorGraph.Runtime.Attributes;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Decorators
{
    [TaskIcon("Assets/BehaviorGraph/Editor/Resources/Images/repeater.png")]
    public class Repeater : Decorator
    {
        public enum RepeaterMode
        {
            RepeatTimes,
            RepeatUntil,
            RepeatForever
        }

        public enum RepeatUntil
        {
            Failure,
            Success
        }

        [SerializeField]
        private RepeaterMode repeatMode = RepeaterMode.RepeatTimes;

        [SerializeField]
        [DependsOn(nameof(repeatMode), Value = RepeaterMode.RepeatTimes)]
        private IntVariable repeatTimes = 1;

        [SerializeField]
        [DependsOn(nameof(repeatMode), Value = RepeaterMode.RepeatUntil)]
        private RepeatUntil repeatUntil = RepeatUntil.Success;

        private int currentCount;
        protected override void OnStart()
        {
            currentCount = repeatTimes.Value;
        }

        protected override NodeState OnUpdate()
        {
            if(repeatMode == RepeaterMode.RepeatForever)
            {
                child.Evaluate();
                return NodeState.Running;
            }
            else
            {
                if (currentCount == 0 && repeatMode == RepeaterMode.RepeatTimes)
                {
                    return NodeState.Success;
                }

                var state = child.Evaluate();
                switch (state)
                {
                    case NodeState.Running:
                        return NodeState.Running;

                    case NodeState.Failure:
                        if (repeatMode == RepeaterMode.RepeatUntil && repeatUntil == RepeatUntil.Failure)
                            return NodeState.Failure;
                        
                        if (repeatMode == RepeaterMode.RepeatTimes)
                            return NodeState.Failure;
                        break;
                    case NodeState.Success:
                        if (repeatMode == RepeaterMode.RepeatTimes)
                            currentCount--;
                        if (repeatMode == RepeaterMode.RepeatUntil && repeatUntil == RepeatUntil.Success)
                            return NodeState.Success;
                        break;
                }

                return NodeState.Running;
            }
        }
    }
}
