using System.Collections.Generic;
using BehaviorGraph.Runtime;
using UnityEngine;

namespace BehaviorGraph.Editor
{
    public class BlackboardViewSO : ScriptableObject
    {
        [SerializeReference]
        private List<IBBVariable> allVariables = new();

        public IBlackboard Blackboard { get; private set; }

        public void Initialize(IBlackboard blackboard)
        {
            if (blackboard == null)
                return;

            Blackboard = blackboard;
            allVariables = blackboard.AllVariables;
        }
    }
}
