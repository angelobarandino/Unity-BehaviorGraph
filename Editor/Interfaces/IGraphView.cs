using System.Collections.Generic;
using BehaviourGraph.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public interface IGraphView
    {
        IBehaviourOwner BehaviourOwner { get; }
        UnityEngine.Object GetAssetInstance();
        void SaveChangesToAsset();

        void Add(VisualElement element);
        void AddElement(GraphElement element);
        void DeleteElements(IEnumerable<GraphElement> elements);
        void RecordObjectUndo(TaskUpdateEvent update);
    }
}
