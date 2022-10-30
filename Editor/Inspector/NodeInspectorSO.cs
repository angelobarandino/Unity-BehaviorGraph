using BehaviorGraph.Runtime;
using UnityEngine;

namespace BehaviorGraph.Editor
{
    public class NodeInspectorSO : ScriptableObject
    {
        [SerializeReference]
        public INode node;
        
        public NodeInspectorProvider Provider { get; private set; }

        public void Load(INode node, NodeInspectorProvider provider)
        {
            this.node = node;
            this.Provider = provider;
        }
    }
}
