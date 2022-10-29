using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviourGraph.Editor.Ports
{
    public class NodeEdge : Edge
    {
    }

    public class NodePort : Port
    {
        protected NodePort(Direction portDirection, Capacity portCapacity) : base(Orientation.Vertical, portDirection, portCapacity, typeof(INode))
        {
            this.Q<Label>("type").text = string.Empty;
        }

        public static NodePort Create(Direction direction, Capacity capacity)
        {
            var listener = new NodeEdgeConnectorListener();
            var port = new NodePort(direction, capacity)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(listener)
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.portCapLit = false;
            return port;
        }
    }

    public class BoxEl : GraphElement
    {
        public BoxEl()
        {
            var box = new VisualElement();
            box.style.width = 10f;
            box.style.height = 10f;
            box.style.backgroundColor = Color.red;
            Add(box);
        }
    }

    public class NodeEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        public NodeEdgeConnectorListener()
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();

            if (edge.input.capacity == Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        m_EdgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Capacity.Single)
            {
                foreach (Edge connection2 in edge.output.connections)
                {
                    if (connection2 != edge)
                    {
                        m_EdgesToDelete.Add(connection2);
                    }
                }
            }

            if (m_EdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(m_EdgesToDelete);
            }

            var edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (var item in edgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var graphView = edge.panel.visualTree.Q<GraphView>("graph-view");

            TryConnectingPorts(edge, position, graphView);

            //var provider = ScriptableObject.CreateInstance<SearchWindowProvider>();

            //var draggedPort = (edge.output?.edgeConnector.edgeDragHelper.draggedPort) ?? (edge.input?.edgeConnector.edgeDragHelper.draggedPort);

            //SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
        }

        private void TryConnectingPorts(Edge edge, Vector2 position, GraphView graphView)
        {
            var dropPos = graphView.viewTransform.matrix.inverse.MultiplyPoint(position - new Vector2(10, 50));

            foreach (var element in graphView.graphElements)
            {
                if (element is GraphNodeView node)
                {
                    if (node.GetPosition().Contains(dropPos))
                    {
                        m_EdgesToCreate.Clear();
                        
                        if (edge.input != null)
                        {
                            if (node.TryConnectTo(edge.input, Direction.Output, out var edgeToCreate))
                                m_EdgesToCreate.Add(edgeToCreate);
                        }
                        else if (edge.output != null)
                        {
                            if (node.TryConnectTo(edge.output, Direction.Input, out var edgeToCreate))
                                m_EdgesToCreate.Add(edgeToCreate);
                        }

                        var edgesToCreate = m_EdgesToCreate;
                        if (graphView.graphViewChanged != null)
                        {
                            edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                        }

                        foreach (var item in edgesToCreate)
                        {
                            graphView.AddElement(item);
                        }

                        break;
                    }
                }
            }
        }
    }

    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public SearchWindowProvider()
        {
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Add Task"), 0),

                new SearchTreeGroupEntry(new GUIContent("Actions"), 1),
                new SearchTreeEntry(new GUIContent("Log")) { level = 2 },
                new SearchTreeEntry(new GUIContent("Wait")) { level = 2 },

                new SearchTreeGroupEntry(new GUIContent("Composites"), 1),
                new SearchTreeEntry(new GUIContent("Selector")) { level = 2 },
                new SearchTreeEntry(new GUIContent("Sequence")) { level = 2 },
                new SearchTreeEntry(new GUIContent("Parallel")) { level = 2 },

                new SearchTreeGroupEntry(new GUIContent("Conditions"), 1),
                new SearchTreeGroupEntry(new GUIContent("Decoratos"), 1),
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            return true;
        }
    }
}
