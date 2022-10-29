using System;
using BehaviourGraph.Editor.Ports;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    public abstract class GraphNodeView : Node
    {
        private readonly INode node;

        public GraphNodeView(INode node, string uiFile) : base(uiFile)
        {
            this.node = node;
            base.title = this.node.Name;
            viewDataKey = this.node.Id;

            UseNodePosition(node.GetPosition());

            Node.OnNodeUpdate -= OnNodeUpdate;
            Node.OnNodeUpdate += OnNodeUpdate;
        }

        private NodePort input;
        public NodePort Input 
        {
            get => input;
            set
            {
                if (input != null && value != input)
                {
                    inputContainer.Remove(input);
                    input = value;
                }
                else
                {
                    input = value;
                    inputContainer.Add(input);
                }
            }
        }

        private NodePort output;
        public NodePort Output 
        {
            get => output;
            set
            {
                if (output != null && value != output)
                {
                    outputContainer.Remove(output);
                    output = value;
                }
                else
                {
                    output = value;
                    outputContainer.Add(output);
                }
            }
        }
        
        public IGraphView GraphView { get; set; }
        public Action Selected { get; set; }


        public INode Node
        {
            get => node;
        }

        protected virtual void OnNodeUpdate()  {}

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            
            node.SetPosition(new Vector2(newPos.xMin, newPos.yMin));
        }

        private void UseNodePosition(Vector2 position)
        {
            style.top = position.y;
            style.left = position.x;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Selected?.Invoke();
        }

        public bool TryConnectTo(Port port, Direction direction, out Edge edge)
        {
            edge = null;
            switch (direction)
            {
                case Direction.Input:
                    if (Input != null && !Input.connected)
                    {
                        edge = Input.ConnectTo(port);
                    }
                    break;
                case Direction.Output:
                    if (Output != null)
                    {
                        edge = Output.ConnectTo(port);
                    }
                    break;
            }

            return edge != null;
        }

        public bool TryConnectTo(GraphNodeView graphNode, out Edge edge)
        {
            edge = null;
            if (graphNode.Input != null)
            {
                edge = Output.ConnectTo(graphNode.Input);
            }

            return edge != null;
        }

        public void SortChildren()
        {
            if (Node is IParentTask parentNode)
            {
                parentNode.SortChildren();
            }
        }
    }
}
