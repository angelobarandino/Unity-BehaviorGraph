using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor
{
    public interface IGraphView
    {
        IBehaviourOwner BehaviourOwner { get; }
        void SaveChangesToAsset();

        void Add(VisualElement element);
        void AddElement(GraphElement element);
        void DeleteElements(IEnumerable<GraphElement> elements);
    }
}
