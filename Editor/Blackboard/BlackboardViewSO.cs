using System.Collections.Generic;
using UnityEngine;

namespace BehaviourGraph.Editor
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
