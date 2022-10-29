using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviourGraph.Editor.Ports
{
    public class NodeEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange graphViewChange;
        private readonly List<Edge> edgesToCreate;
        private readonly List<GraphElement> edgesToDelete;

        public NodeEdgeConnectorListener()
        {
            edgesToCreate = new List<Edge>();
            edgesToDelete = new List<GraphElement>();
            graphViewChange.edgesToCreate = edgesToCreate;
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            this.edgesToCreate.Clear();
            this.edgesToCreate.Add(edge);
            edgesToDelete.Clear();

            if (edge.input.capacity == Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        edgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Capacity.Single)
            {
                foreach (Edge connection2 in edge.output.connections)
                {
                    if (connection2 != edge)
                    {
                        edgesToDelete.Add(connection2);
                    }
                }
            }

            if (edgesToDelete.Count > 0)
            {
                graphView.DeleteElements(edgesToDelete);
            }

            var edgesToCreate = this.edgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(graphViewChange).edgesToCreate;
            }

            foreach (var item in edgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }
        }

        public void OnDropOutsidePort(Edge currentEdge, Vector2 position)
        {
            var graphView = currentEdge.panel.visualTree.Q<GraphView>("graph-view");

            CheckIfDropAboveNode(position, graphView, node =>
            {
                var newEdge = ConnectOnDroppedNode(currentEdge, node);
                if (newEdge != null)
                {
                    this.edgesToCreate.Clear();
                    this.edgesToCreate.Add(newEdge);

                    var edgesToCreate = this.edgesToCreate;
                    if (graphView.graphViewChanged != null)
                    {
                        edgesToCreate = graphView.graphViewChanged(graphViewChange).edgesToCreate;
                    }

                    foreach (var item in edgesToCreate)
                    {
                        graphView.AddElement(item);
                    }
                }
            });

            //var provider = ScriptableObject.CreateInstance<SearchWindowProvider>();

            //var draggedPort = (edge.output?.edgeConnector.edgeDragHelper.draggedPort) ?? (edge.input?.edgeConnector.edgeDragHelper.draggedPort);

            //SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
        }

        private void CheckIfDropAboveNode(Vector2 position, GraphView graphView, System.Action<GraphNodeView> callback)
        {
            var dropPos = graphView.viewTransform.matrix.inverse.MultiplyPoint(position - new Vector2(10, 50));

            foreach (var element in graphView.graphElements)
            {
                if (element is GraphNodeView node)
                {
                    if (node.GetPosition().Contains(dropPos))
                    {
                        callback(node);
                        break;
                    }
                }
            }
        }

        private Edge ConnectOnDroppedNode(Edge edge, GraphNodeView node)
        {
            if (edge.input != null)
            {
                if (node.TryConnectTo(edge.input, Direction.Output, out var edgeToCreate))
                    return edgeToCreate;
            }
            else if (edge.output != null)
            {
                if (node.TryConnectTo(edge.output, Direction.Input, out var edgeToCreate))
                    return edgeToCreate;
            }

            return null;
        }
    }
}
