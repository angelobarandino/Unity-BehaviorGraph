using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourGraph.Runtime;
using BehaviourGraph.Runtime.Attributes;
using BehaviourGraph.Runtime.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Action = BehaviourGraph.Runtime.Tasks.Action;

namespace BehaviourGraph.Editor
{
    public class BehaviourGraphView : GraphView, IGraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourGraphView, GraphView.UxmlTraits> { }

        private IBehaviour activeBehaviour;
        private readonly BlackboardProvider blackboardProvider;
        private readonly NodeInspectorProvider nodeInspector;

        public IBehaviourOwner BehaviourOwner 
        { 
            get => activeBehaviour.BehaviourOwner;
        }

        public BehaviourGraphView()
        {
            Insert(0, new GridBackground());
            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddStyleSheet("Assets/BehaviourGraph/Editor/Resources/StyleSheets/BehaviourTreeEditorStyle.uss");

            blackboardProvider = new BlackboardProvider(this);
            nodeInspector = new NodeInspectorProvider(this);

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            LoadBehaviourTree(activeBehaviour);
        }

        public void LoadBehaviourTree(IBehaviour behaviour)
        {
            activeBehaviour = behaviour;
            nodeInspector.Hide();

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            activeBehaviour.DataSource.OnBehaviorAfterUpdate -= OnNodesUpdate;
            activeBehaviour.DataSource.OnBehaviorAfterUpdate += OnNodesUpdate;
            activeBehaviour.DataSource.OnBehaviorBeforeUpdate = updateEvent =>
            {
                RecordObjectUndo(updateEvent);
            };

            activeBehaviour.DataSource.AllNodes.ForEach(node => CreateNodeView(node));
            activeBehaviour.DataSource.AllNodes.ForEach(node => CreateNodeEdges(node));
        }


        private void OnNodesUpdate(INode node, TaskUpdateEvent updateEvent)
        {
            switch (updateEvent)
            {
                case TaskUpdateEvent.Create:
                    CreateNodeView(node);
                    break;
                case TaskUpdateEvent.Replace:
                    break;
                case TaskUpdateEvent.Decorate:
                    break;
                case TaskUpdateEvent.Remove:
                    break;
            }
        }

        private void CreateNodeView(INode node)
        {
            GraphNodeView nodeView = null;
            
            switch (node)
            {
                case ISubTree:
                    nodeView = new SubTreeNodeView(node);
                    break;

                case ITask:
                    nodeView = new TaskNodeView(node);
                    break;
            }

            if (nodeView != null)
            {
                nodeView.GraphView = this;
                nodeView.Selected = () => nodeInspector.Show(node);
                AddElement(nodeView);
            }
        }

        private void CreateNodeEdges(INode node)
        {
            if (node is IParentTask parentTask)
            {
                var parentNode = FindGraphNodeView(node.Id);
                parentTask.GetChildren().ForEach(child =>
                {
                    var childNode = FindGraphNodeView(child.Id);
                    AddElement(parentNode.ConnectOutput(childNode));
                });
            }
        }

        private GraphNodeView FindGraphNodeView(string guid)
        {
            return GetNodeByGuid(guid) as GraphNodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is GraphNodeView nodeToRemove)
                    {
                        activeBehaviour.DataSource.RemoveNode(nodeToRemove.Node);
                    }

                    if (element is Edge edge)
                    {
                        var childNode = edge.input.node as GraphNodeView;
                        var parentNode = edge.output.node as GraphNodeView;
                        activeBehaviour.DataSource.RemoveChild(parentNode.Node, childNode.Node);
                    }
                }

                nodeInspector.Hide();
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    var childNode = edge.input.node as GraphNodeView;
                    var parentNode = edge.output.node as GraphNodeView;
                    activeBehaviour.DataSource.AddChild(parentNode.Node, childNode.Node);
                });
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (var elements in graphViewChange.movedElements)
                {
                    if (elements is GraphNodeView nodeView)
                    {
                        var parentNode = FindGraphNodeView(nodeView.Node.ParentId);
                        if (parentNode != null)
                        {
                            parentNode.SortChildren();
                        }
                    }
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var position = GetMousePosition(evt);

            if (evt.target is TaskNodeView nodeView)
            {
                evt.menu.AppendAction("Set Start", a =>
                {
                    //check if there is an existing assigned root task and remove it
                    if (activeBehaviour.DataSource.RootTask != null)
                    {
                        if (GetElementByGuid(activeBehaviour.DataSource.RootTask.Id) is TaskNodeView taskNodeView)
                        {
                            taskNodeView.SetRootTask(false);
                        }
                    }

                    //set the new RootTask
                    activeBehaviour.DataSource.SetRootTask(nodeView.Node);
                   
                    //show 'START' label
                    nodeView.SetRootTask(true);
                });
            }
            else
            {
                GraphContextualManager.BuildMenu(evt, type =>
                {
                    var node = (INode)Activator.CreateInstance(type);
                    node.SetPosition(position);
                    activeBehaviour.DataSource.CreateNode(node);
                });

                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Behaviour SubTree", a =>
                {
                    var subTree = new SubTree();
                    subTree.SetPosition(position);
                    activeBehaviour.DataSource.CreateNode(subTree);
                });

                evt.menu.AppendAction("Create Action Task", a =>
                {
                    var actionTask = new ActionTask();
                    actionTask.SetPosition(position);
                    activeBehaviour.DataSource.CreateNode(actionTask);
                });
            }
        }

        private Vector2 GetMousePosition(ContextualMenuPopulateEvent evt)
        {
            return (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        }

        public UnityEngine.Object GetAssetInstance()
        {
            var id = activeBehaviour.GetInstanceID();
            return EditorUtility.InstanceIDToObject(id);
        }

        public void SaveChangesToAsset()
        {
            var asset = GetAssetInstance();
            if (asset)
            {
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                Debug.Log("CHANGES SAVED!");
            }
        }

        public void RecordObjectUndo(TaskUpdateEvent updateEvent)
        {
            var name = $"{updateEvent} Node ({activeBehaviour.Name})";
            Undo.RegisterCompleteObjectUndo(GetAssetInstance(), name);
        }
    }

    public class GraphContextualManager
    {
        public static void BuildMenu(ContextualMenuPopulateEvent evt, Action<Type> callback)
        {
            AddTaskMenu<Action>(evt, "Actions", callback);
            AddTaskMenu<Composite>(evt, "Composites", callback);
            AddTaskMenu<Condition>(evt, "Conditions", callback);
            AddTaskMenu<Decorator>(evt, "Decorators", callback);
        }

        private static void AddTaskMenu<T>(ContextualMenuPopulateEvent evt, string group, Action<Type> callback)
        {
            foreach (var menu in BuildContextMenuItemsOfType<T>())
            {
                if (menu.IsSeparator)
                {
                    evt.menu.AppendSeparator(string.Empty);
                    continue;
                }

                evt.menu.AppendAction($"Add Task/{group}/{menu.ActionName}", a => callback(menu.Type));
            }
        }

        private static IEnumerable<ContextMenuItem> BuildContextMenuItemsOfType<T>()
        {
            var menuItems = new List<ContextMenuItem>();
            
            foreach (var type in TypeCache.GetTypesDerivedFrom<T>())
            {
                if (type.IsAbstract || type.IsGenericType) continue;

                var menuItem = new ContextMenuItem(type);
                var categoryAttr = type.GetCustomAttribute<TaskCategory>();
                if (categoryAttr != null)
                {
                    menuItem.Category = categoryAttr.Category;
                    menuItem.CustomName = categoryAttr.Name;
                    menuItem.Level = categoryAttr.Level;
                }

                menuItems.Add(menuItem);
            }

            return menuItems.OrderBy(m => m.Level);
        }
    }

    public struct ContextMenuItem
    {
        public string name;
        public ContextMenuItem(Type type) : this()
        {
            name = type.Name;
            Type = type;
        }

        public string ActionName
        {
            get
            {
                var actionName = (CustomName ?? name).ToFriendlyName();
                if (!string.IsNullOrWhiteSpace(Category))
                    return $"{Category}/{actionName}";

                return actionName;
            }
        }

        public Type Type { get; }
        public string Category { get; set; }
        public string CustomName { get; set; }
        public bool IsSeparator { get; set; }
        public int? Level { get; set; }
    }
}