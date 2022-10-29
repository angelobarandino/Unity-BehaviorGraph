using UnityEngine;

namespace BehaviourGraph.Editor
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
