using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks
{

    public abstract class Composite : ParentTask
    {
        [SerializeField]
        private bool reactive = false;

        private int currentIndex;
        private int lastRunningIndex = 0;
        
        private readonly List<Task> conditionalTasks = new();
        private readonly Dictionary<string, NodeState> allChildState = new();

        protected override void OnStart()
        {
            currentIndex = reactive ? lastRunningIndex : 0;

            if (currentIndex == 0)
            {
                GetConditionTasks();
                ResetChildStates();
            }
        }

        protected override NodeState OnUpdate()
        {
            OnChildStart(currentIndex);
            var child = children[currentIndex] as ITask;

            if (reactive)
            {
                CheckChildIsConditionTask(ref child);
                
                var conditionFailed = EvaluateConditionalTask();
                if (conditionFailed)
                {
                    var interuptState = ReactiveInteruptionState();
                    child.Interupt(interuptState);
                }
            }

            var childState = child.Evaluate();
            allChildState[child.Id] = childState;
            return OnChildUpdate(currentIndex, childState);
        }


        protected virtual void OnChildStart(int childIndex) { }
        protected virtual NodeState ReactiveInteruptionState() => NodeState.Failure;
        protected virtual NodeState OnChildUpdate(int childIndex, NodeState childState) => childState;


        protected bool CheckAnyChildStatesEquals(NodeState taskState) => allChildState.Any(state => state.Value == taskState);
        protected bool CheckAllChildStatesEquals(NodeState taskState) => allChildState.All(state => state.Value == taskState);
        protected void ExecuteNextChild()
        {
            currentIndex++;
            
            if (currentIndex == children.Count)
                currentIndex = 0;

            lastRunningIndex = currentIndex;
        }

        private void GetConditionTasks()
        {
            conditionalTasks.Clear();
            foreach (var task in children)
            {
                if (task is Condition)
                    conditionalTasks.Add(task as Task);

                if (task is Decorator decorator && decorator.child is Condition)
                    conditionalTasks.Add(decorator);
            }
        }
        private bool EvaluateConditionalTask()
        {
            var interuptionState = ReactiveInteruptionState();
            if (interuptionState != NodeState.Success && interuptionState != NodeState.Failure)
            {
                throw new Exception("ReactiveInteruptionState() function should return either SUCCESS or FAILURE");
            }

            var conditionTaskStates = new List<NodeState>();
            for (var index = 0; index < conditionalTasks.Count; index++)
            {
                var conditionTaskState = conditionalTasks[index].Evaluate();
                allChildState[conditionalTasks[index].Id] = conditionTaskState;
                conditionTaskStates.Add(conditionTaskState);
            }

            return conditionTaskStates.All(state => state == interuptionState);
        }
        private void CheckChildIsConditionTask(ref ITask child)
        {
            if (child is Condition)
            {
                ExecuteNextChild();
                OnChildStart(currentIndex);
                child = (ITask)children[currentIndex];
            }

            if (child is Decorator decorator && decorator.child is Condition)
            {
                ExecuteNextChild();
                OnChildStart(currentIndex);
                child = (ITask)children[currentIndex];
            }
        }
        private void ResetChildStates()
        {
            allChildState.Clear();

            foreach (var node in children)
            {
                allChildState.Add(node.Id, NodeState.Ready);

                Traverse(node as ITask, child => child.OverrideState(NodeState.Ready));
            }
        }
    }
}