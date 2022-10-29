using System;
using UnityEngine;

namespace BehaviourGraph
{
    [Serializable]
    public abstract class BBVariable : IBBVariable
    {
        [SerializeField]
        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField]
        private string referenceName;
        public string ReferenceName
        {
            get => referenceName;
            set => referenceName = value;
        }

        [SerializeField]
        private bool isReferenced;
        public bool IsReferenced
        {
            get => isReferenced;
            set => isReferenced = value;
        }

        [SerializeField]
        private bool isDynamic;
        public bool IsDynamic 
        { 
            get => isDynamic;
            set => isDynamic = value; 
        }

        [SerializeField]
        private bool invalid;
        public bool Invalid
        {
            get => invalid;
            set => invalid = value;
        }

        [SerializeField]
        protected BindData bindData;

        protected IBlackboard blackboard;

        public BindData BindData
        {
            get => bindData;
        }

        public void Bind(BindData data)
        {
            bindData = data;
        }
        public bool IsBindActive()
        {
            return bindData.IsBindActive;
        }
        public void ToggleIsReference()
        {
            isDynamic = false;
            referenceName = null;
            isReferenced = !isReferenced;
        }
        

        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract Type GetVariableType();
        public abstract void Initialize(IBlackboard blackboard);
    }

    [Serializable]
    public abstract class BBVariable<T> : BBVariable
    {
        public BBVariable(){}
        public BBVariable(bool isReference) 
        {
            IsReferenced = isReference;
        }

        private Func<T> cachedValueGetter;
        private Action<T> cachedValueSetter;

        [SerializeField]
        private T value;
        public T Value
        {
            get 
            {
                if (cachedValueGetter != null)
                {
                    return cachedValueGetter();
                }

                return value;
            }
            set
            {
                if (cachedValueSetter != null)
                {
                    cachedValueSetter(value);
                }
                else
                {
                    this.value = value;
                }
            }
        }

        public override void Initialize(IBlackboard blackboard)
        {
            this.blackboard = blackboard;

            if (bindData.IsBindActive)
            {
                var component = bindData.GameObject.GetComponent(bindData.ComponentName);
                var property = component.GetType().GetProperty(bindData.PropertyName);
            
                var getMethod = property.GetGetMethod();
                if (getMethod != null)
                {
                    cachedValueGetter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), component, getMethod);
                }

                var setMethod = property.GetSetMethod();
                if (setMethod != null)
                {
                    cachedValueSetter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), component, setMethod);
                }
            }

            if (IsReferenced && IsDynamic && Application.isPlaying)
            {
                cachedValueGetter = () => blackboard.GetValue<T>(Name);
                cachedValueSetter = value => blackboard.SetValue(Name, value);
                this.blackboard.AddVariable(this);
            }
        }

        public override Type GetVariableType()
        {
            return typeof(T);
        }

        public override void SetValue(object value)
        {
            Value = (T)value;
        }

        public override object GetValue()
        {
            return Value;
        }
    }
}
