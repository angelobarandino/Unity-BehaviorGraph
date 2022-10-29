using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public static class VisualElementExtensions
    {
        public static void AddStyleSheet(this VisualElement visualElement, string assetPath)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath);
            
            if (styleSheet == null) Debug.LogErrorFormat("Missing stylesheet at path: {0}", assetPath);

            visualElement.styleSheets.Add(styleSheet);
        }

        public static void LoadVisualTreeAsset(this VisualElement visualElement, string assetPath)
        {
            var tpl = EditorGUIUtility.Load(assetPath) as VisualTreeAsset;
            tpl.CloneTree(visualElement);
        }
    }
}
