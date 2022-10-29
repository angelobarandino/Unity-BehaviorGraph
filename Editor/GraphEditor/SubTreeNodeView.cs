using BehaviourGraph.Editor.Ports;
using BehaviourGraph.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
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
                infoLabel.text = value;
                infoLabel.style.display = string.IsNullOrEmpty(value)
                    ? DisplayStyle.None 
                    : DisplayStyle.Flex;
            }
        }

        public SubTreeNodeView(INode node) : base(node, "Assets/BehaviourGraph/Editor/Resources/Uxml/SubTreeNodeView.uxml")
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
                    Info = subTree.BehaviourSubTree.Name;
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
            var path = EditorUtility.SaveFilePanel("Create BehaviourSubTree", "Assets", "BehaviourSubTree", "asset");
            if (path.Length != 0)
            {
                var projectDir = Application.dataPath.Replace("Assets", string.Empty);
                var assetPath = path.Replace(projectDir, string.Empty);

                var asset = ScriptableObject.CreateInstance<BehaviorSubTree>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();

                if (Node is ISubTree subTree)
                {
                    subTree.BehaviourSubTree = asset;
                }
            }
        }

        protected override void OnNodeUpdate()
        {
            base.OnNodeUpdate();

            CheckBehaviourAsset();
        }
    }
}
