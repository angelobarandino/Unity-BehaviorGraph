using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorGraph.Runtime.Utilities;
using UnityEngine;

namespace BehaviorGraph.Runtime.Tasks
{
    [Serializable]
    public abstract class Task : Node, ITask
    {
        private IBehaviourOwner owner;
        public IBehaviourOwner Owner
        {
            get => owner;
        }

        public IBlackboard Blackboard
        {
            get => owner.Blackboard;
        }

        private GameObject gameObject;
        protected GameObject GameObject
        {
            get => gameObject;
        }

        protected Transform transform
        {
            get => gameObject.transform;
        }

        [SerializeField, HideInInspector]
        private bool isRootTask;
        public bool IsRootTask => isRootTask;

        private bool started = false;
        private bool isInterupt = false;
        private NodeState currentState;
        private NodeState interuptState;

        public Task()
        {
            Name = GetType().Name;
        }

        public NodeState Evaluate()
        {
            if (!started)
            {
                started = true;
                currentState = NodeState.Running;
                OnStart();
            }

            currentState = isInterupt ? interuptState : OnUpdate();

            if (currentState == NodeState.Success || currentState == NodeState.Failure)
            {
                started = false;
                isInterupt = false;
                OnStop();
            }

            return currentState;
        }

        public override NodeState GetState()
        {
            return currentState;
        }

        protected virtual NodeState OnUpdate() { return NodeState.Success; }
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnInterupt(NodeState interuptState) { }

        public void Interupt(NodeState interuptState)
        {
            OnInterupt(interuptState);

            foreach (var child in GetChildren())
            {
                if (child.GetState() == NodeState.Running)
                {
                    child.Interupt(interuptState);
                    child.Evaluate();
                }
            }

            this.interuptState = interuptState;
            this.isInterupt = true;
        }

        public virtual void Initialize(IBehaviourOwner owner)
        {
            this.owner = owner;

            gameObject = owner.gameObject;

            InjectBlackboardVariables(owner.Blackboard);
        }

        public void OverrideState(NodeState taskState)
        {
            currentState = taskState;
        }

        public virtual string GetInfo()
        {
            return Name;
        }

        public virtual List<ITask> GetChildren() 
        { 
            return new List<ITask>(); 
        }

        private void InjectBlackboardVariables(IBlackboard blackboard)
        {
            var variables = TaskUtility.GetFieldInfos(GetType());

            foreach (var fieldInfo in variables)
            {
                if (typeof(IBBVariable).IsAssignableFrom(fieldInfo.FieldType))
                {
                    if (fieldInfo.GetValue(this) is IBBVariable variable)
                    {
                        if (variable.IsReferenced)
                        {
                            IBBVariable bbVariable = null;
                            if (variable.IsDynamic)
                            {
                                bbVariable = blackboard.AddGetDynamicVariable(variable);
                            }
                            else
                            {
                                bbVariable = blackboard.GetVariable(variable.ReferenceName);
                                if (bbVariable == null)
                                {
                                    throw new InvalidOperationException($"A Blackboard variable '{variable.ReferenceName}' bound to {Name}.{fieldInfo.Name} property does not exist.");
                                }
                            }

                            if (!bbVariable.Invalid) fieldInfo.SetValue(this, bbVariable);
                        }
                    }
                }
            }
        }

        public bool IsNullOrDestroyed(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

            return false;
        }


        public void Traverse(ITask task, Action<ITask> callback)
        {
            callback.Invoke(task);
            var children = task.GetChildren();
            children.ForEach(child => Traverse(child, callback));
        }

        public virtual void OnBehaviorStart() { }

        protected Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Owner.StartCoroutine(enumerator);
        }
        protected void StopCoroutine(IEnumerator enumerator)
        {
            Owner.StopCoroutine(enumerator);
        }
        protected void StopCoroutine(Coroutine coroutine)
        {
            Owner.StopCoroutine(coroutine);
        }
        protected T GetComponent<T>()
        {
            return Owner.GetComponent<T>();
        }

#if UNITY_EDITOR
        public void SetRootTask(bool rootTask)
        {
            isRootTask = rootTask;
        }
        public override object Clone()
        {
            return TaskUtility.CreateCopy(this);
        }
#endif

    }
}