using BehaviorGraph.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    public class NodeInspectorGraphElement : GraphElement
    {
        private NodeInspectorSO scriptableObject;
        public NodeInspectorGraphElement()
        {
            AddToClassList("hidden");
            AddToClassList("taskInspector");
            this.LoadVisualTreeAsset("Assets/BehaviorGraph/Editor/Resources/Uxml/NodeInspector.uxml");

            capabilities |= Capabilities.Movable;
            this.AddManipulator(new Dragger { clampToParentEdges = true });

            NodeGUI.onGUIHandler = () =>
            {
                if (!scriptableObject) return;
                var editor = UnityEditor.Editor.CreateEditor(scriptableObject);
                if (editor) editor.OnInspectorGUI();
            };
        }

        private Label titleLabel;
        public string Title 
        {
            set
            {
                titleLabel ??= this.Q<Label>("title-label");
                titleLabel.text = value;
            } 
        }

        private IMGUIContainer imGui;
        public IMGUIContainer NodeGUI
        {
            get
            {
                return imGui ??= this.Q<IMGUIContainer>("imgui-container");
            }
        }

        public void Show(INode node, NodeInspectorProvider provider)
        {
            if (node == null)
            {
                AddToClassList("hidden");
                return;
            }

            Title = node.Name;
            RemoveFromClassList("hidden");
            scriptableObject = ScriptableObject.CreateInstance<NodeInspectorSO>();
            scriptableObject.Load(node, provider);
        }
    }
}
