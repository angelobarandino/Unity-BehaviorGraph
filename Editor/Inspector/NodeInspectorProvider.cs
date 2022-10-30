
using System.Collections.Generic;
using BehaviorGraph.Runtime;

namespace BehaviorGraph.Editor
{
    public class NodeInspectorProvider
    {
        private readonly IGraphView graphView;
        private readonly NodeInspectorGraphElement graphElement;
        public NodeInspectorProvider(IGraphView graphView)
        {
            graphElement = new NodeInspectorGraphElement();
         
            this.graphView = graphView;
            this.graphView.Add(graphElement);
        }

        public void Show(INode node)
        {
            graphElement.Show(node, this);
        }

        public void Hide()
        {
            graphElement.Show(null, this);
        }

        public List<IBBVariable> GetBlackboardVariables()
        {
            return graphView.BehaviorOwner?.Blackboard?.AllVariables ?? new List<IBBVariable>();
        }
    }
}
