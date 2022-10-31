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
            SetImageAsset("Assets/BehaviorGraph/Editor/Resources/Images/exclamation.png");
            
            this.scaleMode = ScaleMode.ScaleToFit;
        }

        public void SetImageAsset(string iconPath)
        {
            image = AssetDatabase.LoadAssetAtPath<Texture>(iconPath);
        }
    }
}
