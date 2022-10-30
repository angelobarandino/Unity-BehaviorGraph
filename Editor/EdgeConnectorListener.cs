//using System.Collections.Generic;
//using BehaviorGraph.Runtime.Tasks;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using static UnityEditor.Experimental.GraphView.Port;

//namespace BehaviorGraph.Editor
//{
//    public class EdgeConnectorListener : IEdgeConnectorListener
//    {
//        private NodeView parent;

//        private readonly List<Edge> _edgesToCreate;
//        private readonly List<GraphElement> _edgesToDelete;
//        private GraphViewChange _graphViewChange;

//        public EdgeConnectorListener(NodeView nodeView)
//        {
//            this.parent = nodeView;

//            _edgesToCreate = new List<Edge>();
//            _edgesToDelete = new List<GraphElement>();
//            _graphViewChange.edgesToCreate = _edgesToCreate;
//        }
//        public void OnDropOutsidePort(Edge edge, Vector2 position)
//        {
//            //var graphView = parent.panel.visualTree.Q<BehaviourTreeGraphView>();
//        }

//        public void OnDrop(GraphView graphView, Edge edge)
//        {
//            _edgesToDelete.Clear();
//            _edgesToCreate.Clear();
//            _edgesToCreate.Add(edge);

//            if (edge.input.capacity == Capacity.Single)
//                foreach (var edgeToDelete in edge.input.connections)
//                    if (edgeToDelete != edge)
//                        _edgesToDelete.Add(edgeToDelete);

//            if (edge.output.capacity == Capacity.Single)
//                foreach (var edgeToDelete in edge.output.connections)
//                    if (edgeToDelete != edge)
//                        _edgesToDelete.Add(edgeToDelete);

//            if (edge.output.node is NodeView parent)
//                this.parent = parent;

//            if (this.parent.task is Composite)
//            {
//                var child = edge.input.node as NodeView;
//                var location = edge.output.GetPortLocation();
//                var output = this.parent.GetOrAddOutputPort(child, location, edge.output as TaskPort);

//                if (location != TaskPort.PortLocation.Default)
//                {
//                    _edgesToDelete.Add(edge);
//                }

//                _edgesToCreate.Remove(edge);
//                _edgesToCreate.Add(new Edge
//                {
//                    output = output,
//                    input = child.input
//                });
//            }

//            if (_edgesToDelete.Count > 0)
//            {
//                graphView.DeleteElements(_edgesToDelete);
//            }

//            var edgesToCreate = _edgesToCreate;
//            if (graphView.graphViewChanged != null)
//            {
//                edgesToCreate = graphView.graphViewChanged(_graphViewChange).edgesToCreate;
//            }

//            foreach (Edge e in edgesToCreate)
//            {
//                if (e.output == null) continue;

//                var _edge = e.output.ConnectTo(e.input);
//                _edge.edgeControl.capRadius = 60;
//                graphView.AddElement(_edge);
//            }
//        }

//    }
//}
