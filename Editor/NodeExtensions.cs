
using BehaviorGraph.Runtime;

namespace BehaviorGraph.Editor
{
    public static class NodeExtensions
    {
        public static void Traverse(this INode node, System.Action<INode> callback)
        {
            if (node is IParentTask parentNode)
            {
                callback(parentNode);
                var children = parentNode.GetChildren();
                foreach (var child in parentNode.GetChildren())
                {
                    child.Traverse(callback);
                }
            }
        }

        public static void TraverseFromParent(this INode node, System.Action<INode, INode> callback)
        {
            if (node is IParentTask parentNode)
            {
                var children = parentNode.GetChildren();
                foreach (var child in parentNode.GetChildren())
                {
                    callback(parentNode, child);
                    child.TraverseFromParent(callback);
                }
            }
        }

        public static bool InHierarchy(this INode startNode, INode endNode)
        {
            var isInHierarchy = false;

            if (endNode is IParentTask parentNode)
            {
                var startId = startNode.Id;
                parentNode.GetChildren().ForEach(child =>
                {
                    if (isInHierarchy) return;

                    child.Traverse(node =>
                    {
                        if (node.Id == startId)
                        {
                            isInHierarchy = true;
                        }
                    });
                });
            }

            return isInHierarchy;
        }
    }
}
