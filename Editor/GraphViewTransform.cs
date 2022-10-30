using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    [System.Serializable]
    public class GraphViewTransform
    {
        public Vector3 scale;
        public Vector3 position;

        public static void Save(ITransform viewTransform)
        {
            var grapViewTransform = new GraphViewTransform
            {
                scale = viewTransform.scale,
                position = viewTransform.position,
            };

            var data = JsonUtility.ToJson(grapViewTransform);
            EditorPrefs.SetString(nameof(GraphViewTransform), data);
        }

        public static void Load(GraphView graphView)
        {
            var json = EditorPrefs.GetString(nameof(GraphViewTransform));
            if (!string.IsNullOrEmpty(json))
            {
                var data = JsonUtility.FromJson<GraphViewTransform>(json);
                graphView.viewTransform.position = data.position;
                graphView.viewTransform.scale = data.scale;
            }
        }
    }
}
