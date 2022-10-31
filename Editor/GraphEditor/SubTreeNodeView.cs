using BehaviorGraph.Editor.Ports;
using BehaviorGraph.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    public class SubTreeNodeView : GraphNodeView
    {
        private readonly Button createNewButton;

        private Label infoLabel;
        public string Info
        {
            get => infoLabel?.text;
            set 
            {
                infoLabel ??= this.Q<Label>("info-label");
                infoLabel.text = value?.Replace("(Clone)", "");
                infoLabel.style.display = string.IsNullOrEmpty(value)
                    ? DisplayStyle.None 
                    : DisplayStyle.Flex;
            }
        }

        public SubTreeNodeView(INode node) : base(node, "Assets/BehaviorGraph/Editor/Resources/Uxml/SubTreeNodeView.uxml")
        {
            createNewButton = this.Q<Button>("create-new-button");
            createNewButton.clickable.clicked -= CreateNewClicked;
            createNewButton.clickable.clicked += CreateNewClicked;

            CheckBehaviourAsset();

            Input = CreateInputPort();
        }

        private NodePort CreateInputPort()
        {
            return NodePort.Create(Direction.Input, Port.Capacity.Single);
        }

        private void CheckBehaviourAsset()
        {
            if (Node is ISubTree subTree)
            {
                if (subTree.BehaviourSubTree)
                {
                    Info = subTree.GetInfo();
                    createNewButton.style.display = DisplayStyle.None;
                }
                else
                {
                    Info = null;
                    createNewButton.style.display = DisplayStyle.Flex;
                }
            }
        }

        private void CreateNewClicked()
        {
            if (Node is ISubTree subTree)
            {
                subTree.BehaviourSubTree = BehaviorAssetUtility.Create<BehaviorSubTree>(
                    "Create BehaviorSubTree", "BehaviorSubTree");

                CheckBehaviourAsset();
            }
        }

        protected override void OnNodeUpdate()
        {
            base.OnNodeUpdate();

            CheckBehaviourAsset();
        }
    }
}
