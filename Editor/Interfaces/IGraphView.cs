using System.Collections.Generic;
using BehaviorGraph.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    public interface IGraphView
    {
        IBehaviourOwner BehaviorOwner { get; }
        
        UnityEngine.Object GetAssetInstance();
        void SetBehaviorAssetDirty();

        GraphNodeView FindGraphNodeView(string guid);

        void Add(VisualElement element);
        void AddElement(GraphElement element);
        void DeleteElements(IEnumerable<GraphElement> elements);
        void RecordObjectUndo(TaskUpdateEvent update);
        void OnObjectDropped(Object obj, Vector2 mousePosition);
    }
}
