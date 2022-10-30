using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    public class NodeIcon : Image
    {
        public new class UxmlFactory : UxmlFactory<NodeIcon, Image.UxmlTraits> { }

        public NodeIcon()
        {
            image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/BehaviorGraph/Editor/Resources/Images/exclamation.png");
        }
    }
}
