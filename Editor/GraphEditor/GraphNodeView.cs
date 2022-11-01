using System.Linq;
using System.Reflection;
using BehaviorGraph.Editor.Ports;
using BehaviorGraph.Runtime;
using BehaviorGraph.Runtime.Attributes;
using BehaviorGraph.Runtime.Tasks;
using BehaviorGraph.Runtime.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using GVNode = UnityEditor.Experimental.GraphView.Node;

namespace BehaviorGraph.Editor
{
    public abstract class GraphNodeView : GVNode
    {
        private bool isPlaymode;
        private readonly INode node;

        public GraphNodeView(INode node, string uiFile) : base(uiFile)
        {
            this.node = node;
            viewDataKey = this.node.Id;

            SetEditorMode();
            SetNodeIcon(node);
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
        public System.Action Selected { get; set; }

        public INode Node
        {
            get => node;
        }

        protected virtual void OnNodeUpdate()
        {
            SetTitle();
        }

        protected virtual void SetTitle()
        {
            var info = (Node as ITask).GetInfo();

            title = TextU.ParseHtmlString(info);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            GraphView.RecordObjectUndo(Runtime.TaskUpdateEvent.Update);

            var newPosition = new Vector2(newPos.x, newPos.y);
            var positionDelta = node.GetPosition() - newPosition;
            node.SetPosition(newPosition);

            if (node is IParentTask parentNode)
            {
                var children = parentNode.GetChildren();
                foreach (var child in children)
                {
                    var nodeView = GraphView.FindGraphNodeView(child.Id);
                    var currentPosition = nodeView.GetPosition();
                    currentPosition.x -= positionDelta.x;
                    currentPosition.y -= positionDelta.y;

                    nodeView.capabilities &= ~Capabilities.Snappable;
                    nodeView.SetPosition(currentPosition);
                }
            }
        }

        private void UseNodePosition(Vector2 position)
        {
            style.left = position.x;
            style.top = position.y;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            capabilities |= Capabilities.Snappable;
            Selected?.Invoke();
        }

        public void SortChildren()
        {
            if (Node is IParentTask parentNode)
            {
                parentNode.SortChildren();
            }
        }

        public bool InHierarchy(GraphNodeView graphNodeView)
        {
            return Node.InHierarchy(graphNodeView.Node);
        }

        public Edge ConnectOutput(GraphNodeView nodeView)
        {
            return Connect(Output, nodeView.Input);
        }

        public Edge ConnectInput(GraphNodeView nodeView)
        {
            return Connect(nodeView.Output, Input);
        }

        private Edge Connect(NodePort output, NodePort input)
        {
            if (output == null || input == null) 
                return null;

            if (input.connected)
                GraphView.DeleteElements(input.connections);

            switch (output.capacity)
            {
                case Port.Capacity.Single when output.connected:
                    GraphView.DeleteElements(output.connections);
                    break;

                case Port.Capacity.Multi when output.connections.Any(e => e.input == input):
                    return null;
            }

            return output.ConnectTo(input);
        }

        private void SetNodeIcon(INode node)
        {
            var iconAttr = node.GetType().GetCustomAttribute<TaskIcon>();
            if (iconAttr == null) 
                return;

            BodyContent.Q<NodeIcon>().SetImageAsset(iconAttr.IconPath);
        }

        private VisualElement bodyContent;
        public VisualElement BodyContent
        {
            get => bodyContent ??= this.Q<VisualElement>("body-content");
        }

        public void SetEditorMode()
        {
            isPlaymode = false;
            BodyContent.ClearClassList();

            AddToClassList("editorMode");
            RemoveFromClassList("playMode");

            SetTitle();

            Input?.SetState(NodeState.Ready, includeEdges: true);
            Output?.SetState(NodeState.Ready, includeEdges: false);
        }

        public void UpdatePlaymodeStates()
        {
            if (!isPlaymode)
            {
                isPlaymode = true;
                AddToClassList("playMode");
                RemoveFromClassList("editorMode");
            }

            var state = Node.GetState();

            BodyContent.ClearClassList();
            BodyContent.AddToClassList(state.ToString());

            SetTitle();

            Input?.SetState(state, includeEdges: true);
            Output?.SetState(state, includeEdges: false);
        }
    }
}
