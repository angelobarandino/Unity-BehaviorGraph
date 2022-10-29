using BehaviourGraph.Editor.Ports;
using BehaviourGraph.Runtime;
using BehaviourGraph.Runtime.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public class TaskNodeView : GraphNodeView
    {
        private Label startLabel;
        private bool ShowStartLabel
        {
            set
            {
                startLabel ??= this.Q<Label>("start-label");
                startLabel.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public TaskNodeView(INode node) : base(node, "Assets/BehaviourGraph/Editor/Resources/Uxml/TaskNodeView.uxml")
        {
            SetRootTask((node as ITask).IsRootTask);

            Output = CreateOutputPort();
        }

        private NodePort CreateInputPort()
        {
            return NodePort.Create(Direction.Input, Port.Capacity.Single);
        }

        private NodePort CreateOutputPort()
        {
            return Node switch
            {
                Decorator => NodePort.Create(Direction.Output, Port.Capacity.Single),
                Composite => NodePort.Create(Direction.Output, Port.Capacity.Multi),
                _ => null,
            };
        }

        public void SetRootTask(bool isRootTask)
        {
            if (Input != null)
            {
                GraphView.DeleteElements(Input.connections);
            }

            ShowStartLabel = isRootTask;

            Input = isRootTask ? null : CreateInputPort();
        }
    }
}
