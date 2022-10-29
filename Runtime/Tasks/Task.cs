using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourGraph.Runtime.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviourGraph.Runtime.Tasks
{
    [Serializable]
    public abstract class Task : Node, ITask
    {
        private IBehaviourOwner owner;
        public IBehaviourOwner Owner
        {
            get => owner;
        }

        private IBlackboard blackboard;
        public IBlackboard Blackboard
        {
            get => blackboard;
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

        private NodeState currentState;
        public NodeState State => currentState;


        [SerializeField, HideInInspector]
        private bool isRootTask;
        public bool IsRootTask => isRootTask;


        private bool started = false;
        private bool isInterupt = false;
        private NodeState interuptState;
        public Task()
        {
            Name = GetType().Name;
        }

        public NodeState Evaluate(IBlackboard blackboard)
        {
            this.blackboard ??= blackboard;

            return Evaluate();
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

        public virtual void Awake() { }

        protected virtual NodeState OnUpdate() { return NodeState.Success; }
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnInterupt(NodeState interuptState) { }

        public void Interupt(NodeState interuptState)
        {
            OnInterupt(interuptState);

            foreach (var child in GetChildren())
            {
                if (child.State == NodeState.Running)
                {
                    child.Interupt(interuptState);
                    child.Evaluate(blackboard);
                }
            }

            this.interuptState = interuptState;
            this.isInterupt = true;
        }

        public virtual void Initialize(IBehaviourOwner owner)
        {
            this.owner = owner;
            gameObject = owner.gameObject;
            InjectPropertyMappng();
        }

        public void OverrideState(NodeState taskState)
        {
            currentState = taskState;
        }

        public virtual List<ITask> GetChildren() 
        { 
            return new List<ITask>(); 
        }

        private void InjectPropertyMappng()
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
                            var bbVariable = variable.IsDynamic 
                                ? Blackboard.AddGetDynamicVariable(variable)
                                : Blackboard.GetVariable(variable.ReferenceName);

                            if (!bbVariable.Invalid) fieldInfo.SetValue(this, bbVariable);
                        }
                    }
                }
            }
        }

        public void Traverse(ITask task, Action<ITask> callback)
        {
            callback.Invoke(task);
            var children = task.GetChildren();
            children.ForEach(child => Traverse(child, callback));
        }

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

        public object Clone()
        {
            return TaskUtility.CreateCopy(this);
        }
#endif

    }
}