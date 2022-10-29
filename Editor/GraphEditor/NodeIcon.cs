using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public class NodeIcon : Image
    {
        public new class UxmlFactory : UxmlFactory<NodeIcon, Image.UxmlTraits> { }

        public NodeIcon()
        {
            image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/BehaviourGraph/Editor/Resources/Images/exclamation.png");
        }
    }
}
