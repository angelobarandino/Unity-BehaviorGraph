using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks
{

    public abstract class Composite : ParentTask
    {
        [SerializeField]
        private bool reactive = false;

        private int currentIndex;
        private int lastRunningIndex = 0;
        
        private readonly List<Task> conditionalTasks = new();

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

            return OnChildUpdate(currentIndex, child.Evaluate());
        }

        protected virtual void OnChildStart(int childIndex) { }
        protected virtual NodeState ReactiveInteruptionState() => NodeState.Failure;
        protected virtual NodeState OnChildUpdate(int childIndex, NodeState childState) => childState;

        protected bool CheckAnyChildStatesEquals(NodeState taskState)
        {
            foreach (var child in children)
            {
                if (child.GetState() == taskState)
                    return true;
            }
            
            return false;
        }

        protected bool CheckAllChildStatesEquals(NodeState taskState)
        {
            foreach (var child in children)
            {
                if (child.GetState() != taskState)
                    return false;
            }

            return true;
        }

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
            foreach (var node in children)
            {
                Traverse(node, child => child.OverrideState(NodeState.Ready));
            }
        }
    }
}