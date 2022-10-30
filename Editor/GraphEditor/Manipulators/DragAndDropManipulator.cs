using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviorGraph.Editor
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private readonly Func<UnityEngine.Object, bool> canDrop;
        public DragAndDropManipulator(Func<UnityEngine.Object, bool> canDrop)
        {
            this.canDrop = canDrop;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            if (DragAndDrop.objectReferences?.Length != 1)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                return;
            }

            DragAndDrop.visualMode = canDrop(DragAndDrop.objectReferences[0]) ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            if (DragAndDrop.objectReferences?.Length != 1)
                return;

            foreach (var obj in DragAndDrop.objectReferences)
            {
                if (target is IGraphView graphView)
                {
                    graphView.OnObjectDropped(obj, evt.localMousePosition);
                }
            }
        }
    }
}