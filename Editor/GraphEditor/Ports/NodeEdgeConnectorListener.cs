using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviorGraph.Editor.Ports
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

            CheckIfDropAboveNode(graphView, targetNode =>
            {
                var newEdge = ConnectToDroppedNode(currentEdge, targetNode);
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

        private void CheckIfDropAboveNode(GraphView graphView, System.Action<GraphNodeView> callback)
        {
            var dropPos = graphView.ChangeCoordinatesTo(graphView.contentViewContainer, Event.current.mousePosition - new Vector2(10f, 50f));

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

        private Edge ConnectToDroppedNode(Edge edge, GraphNodeView targetNode)
        {
            if (edge.output?.node is GraphNodeView outputNode)
            {
                if (outputNode.InHierarchy(targetNode))
                    return null;

                return targetNode.ConnectInput(outputNode);
            }

            if (edge.input?.node is GraphNodeView inputNode)
            {
                if (targetNode.InHierarchy(inputNode))
                    return null;

                return targetNode.ConnectOutput(inputNode);
            }

            return null;
        }
    }
}
