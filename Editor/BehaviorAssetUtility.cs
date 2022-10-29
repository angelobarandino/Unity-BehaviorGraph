using UnityEditor;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    public class BehaviorAssetUtility
    {
        public static T Create<T>(string title, string filename) where T : ScriptableObject
        {
            var path = EditorUtility.SaveFilePanel(title, "Asset", filename, "asset");
            if (path.Length != 0)
            {
                var projectDir = Application.dataPath.Replace("Assets", string.Empty);
                var assetPath = path.Replace(projectDir, string.Empty);

                var asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                return asset;
            }

            return null;
        }
    }
}
