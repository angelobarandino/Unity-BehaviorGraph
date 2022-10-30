using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks.Actions
{
    public class Log : Action
    {
        [SerializeField]
        private StringVariable message;

        protected override NodeState OnUpdate()
        {
            Debug.Log(message.Value);
            return NodeState.Success;
        }
    }
}
