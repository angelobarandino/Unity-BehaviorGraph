using System.Collections;
using BehaviorGraph.Runtime;
using UnityEngine;

namespace BehaviorGraph.Runtime
{
    public interface IBehaviourOwner
    {
        GameObject gameObject { get; }
        IBlackboard Blackboard { get; }

        IBehaviour GetBehavior();

        T GetComponent<T>();
        Coroutine StartCoroutine(IEnumerator enumerator);
        void StopCoroutine(IEnumerator enumerator);
        void StopCoroutine(Coroutine coroutine);
    }
}
