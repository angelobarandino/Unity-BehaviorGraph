using System.Collections;
using BehaviourGraph.Runtime;
using UnityEngine;

namespace BehaviourGraph
{
    public interface IBehaviourOwner
    {
        GameObject gameObject { get; }
        IBlackboard Blackboard { get; }

        IBehaviour GetBehaviour();

        T GetComponent<T>();
        Coroutine StartCoroutine(IEnumerator enumerator);
        void StopCoroutine(IEnumerator enumerator);
        void StopCoroutine(Coroutine coroutine);
    }
}
