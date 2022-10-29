﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourGraph.Runtime;
using BehaviourGraph.Runtime.Attributes;
using BehaviourGraph.Runtime.Tasks;
using CityBuilder.AI.Tasks.Actions;
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
        private Vector2 contextualMousePosition;
        private readonly BlackboardProvider blackboardProvider;
        private readonly NodeInspectorProvider nodeInspectorProvider;

        public IBehaviourOwner BehaviorOwner 
        { 
            get => activeBehaviour?.BehaviourOwner;
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
            nodeInspectorProvider = new NodeInspectorProvider(this);

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            
            //copy and paste callbacks
            canPasteSerializedData -= HandleCanPasetSerializedData;
            canPasteSerializedData += HandleCanPasetSerializedData;
            serializeGraphElements -= HandleSerializeGraphElements;
            serializeGraphElements += HandleSerializeGraphElements;
            unserializeAndPaste -= HandleUnserializeAndPaste;
            unserializeAndPaste += HandleUnserializeAndPaste;
        }

        public void LoadBehaviourTree(IBehaviour behaviour)
        {
            activeBehaviour = behaviour;
            blackboardProvider.LoadVariables();
            nodeInspectorProvider.Hide();

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
                    ClearSelection();
                    AddToSelection(node);
                    break;

                case TaskUpdateEvent.CopyPaste:
                    CreateNodeView(node);
                    AddToSelection(node);
                    break;

                case TaskUpdateEvent.Replace:
                    break;
                case TaskUpdateEvent.Decorate:
                    break;
                case TaskUpdateEvent.Remove:
                    break;
            }

            SetBehaviorAssetDirty();
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
                nodeView.Selected = () => nodeInspectorProvider.Show(node);
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

                nodeInspectorProvider.Hide();
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

                SetBehaviorAssetDirty();
            }

            return graphViewChange;
        }

        private Vector2 GetMousePosition(ContextualMenuPopulateEvent evt)
        {
            return (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        }

        private void OnUndoRedoPerformed()
        {
            LoadBehaviourTree(activeBehaviour);
        }

        private GraphNodeView FindGraphNodeView(string guid)
        {
            return GetNodeByGuid(guid) as GraphNodeView;
        }

        private void AddToSelection(INode node)
        {
            AddToSelection(FindGraphNodeView(node.Id));
        }

        #region GraphView Overriden Methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            contextualMousePosition = GetMousePosition(evt);
            
            evt.menu.AppendAction("Copy", a => CopySelectionCallback(),
                (evt.target is GraphNodeView) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Paste", a => PasteCallback(), 
                canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendAction("Delete", a => DeleteSelectionCallback(AskUser.DontAskUser),
                (evt.target is GraphNodeView) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendSeparator();

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

                evt.menu.AppendAction("Convert To SubTree", a =>
                {
                    var behaviorSubTree = BehaviorAssetUtility.Create<BehaviorSubTree>(
                        "Create BehaviorSubTree", "BehaviorSubTree");

                    if (behaviorSubTree == null)
                        return;

                    if (nodeView.Node is ITask nodeTask)
                    {
                        var nodesToDelete = new List<INode>();
                        nodesToDelete.Add(nodeTask);

                        //create and set sub tree root node
                        var cloneTask = nodeTask.Clone() as ITask;
                        cloneTask.SetPosition(nodeTask.GetPosition());
                        cloneTask.SetRootTask(true);
                        behaviorSubTree.DataSource.CreateNode(cloneTask);

                        //copy children to subtree
                        nodeTask.GetChildren().ForEach(child =>
                        {
                            nodesToDelete.Add(child);

                            var cloneChild = child.Clone() as ITask;
                            cloneChild.SetPosition(child.GetPosition());
                            behaviorSubTree.DataSource.CreateNode(cloneChild);
                            behaviorSubTree.DataSource.AddChild(cloneTask, cloneChild);
                        });

                        //create subtree node
                        var subTree = new SubTree();
                        subTree.BehaviourSubTree = behaviorSubTree;
                        subTree.SetPosition(nodeView.Node.GetPosition());
                        activeBehaviour.DataSource.CreateNode(subTree);
                        activeBehaviour.DataSource.DeleteNodes(nodesToDelete);

                        //set subtree parent node
                        var parentNode = activeBehaviour.DataSource.FindNodeById(nodeTask.ParentId);
                        if (parentNode != null)
                        {
                            activeBehaviour.DataSource.RemoveChild(parentNode, nodeTask);
                            activeBehaviour.DataSource.AddChild(parentNode, subTree);
                        }

                        LoadBehaviourTree(activeBehaviour);
                    }
                });
            }

            if (evt.target is BehaviourGraphView)
            {

                GraphContextualManager.BuildMenu(evt, type =>
                {
                    var node = (INode)Activator.CreateInstance(type);
                    node.SetPosition(contextualMousePosition);
                    activeBehaviour.DataSource.CreateNode(node);
                });

                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Create Behaviour SubTree", a =>
                {
                    var subTree = new SubTree();
                    subTree.SetPosition(contextualMousePosition);
                    activeBehaviour.DataSource.CreateNode(subTree);
                });
            }
        }
        #endregion

        #region IGraphView Implemented Methods
        public UnityEngine.Object GetAssetInstance()
        {
            var id = activeBehaviour.GetInstanceID();
            return EditorUtility.InstanceIDToObject(id);
        }

        public void SetBehaviorAssetDirty()
        {
            var asset = GetAssetInstance();

            if (asset) EditorUtility.SetDirty(asset);
        }

        public void RecordObjectUndo(TaskUpdateEvent updateEvent)
        {
            var name = $"{updateEvent} Node ({activeBehaviour.Name})";
            Undo.RegisterCompleteObjectUndo(GetAssetInstance(), name);
        }
        #endregion

        #region Copy And Paste Method Callbacks
        private bool HandleCanPasetSerializedData(string data)
        {
            var nodeIds = data.Split('|');
            if (nodeIds.Length == 0) return false;
            return nodeIds.All(guid => Guid.TryParse(guid, out var id));
        }

        private string HandleSerializeGraphElements(IEnumerable<GraphElement> elementsToCopy)
        {
            var nodes = new List<INode>();
            foreach (var element in elementsToCopy)
                if (element is GraphNodeView nodeView)
                    nodes.Add(nodeView.Node);

            nodes.Sort((lNode, rNode) =>
            {
                return lNode.GetPosition().x < rNode.GetPosition().x ? -1 : 1;
            });

            return string.Join("|", nodes.Select(n => n.Id));
        }

        private void HandleUnserializeAndPaste(string operationName, string data)
        {
            if (operationName == "Paste")
            {
                ClearSelection();

                var nodeIds = data.Split('|').ToList();
                var lastNodePosition = Vector2.zero;
                nodeIds.ForEach(nodeId =>
                {
                    var node = activeBehaviour.DataSource.FindNodeById(nodeId);
                    if (node != null)
                    {
                        if (lastNodePosition != Vector2.zero)
                        {
                            contextualMousePosition += node.GetPosition() - lastNodePosition;
                        }

                        var cloneNode = (INode)(node as ITask).Clone();
                        cloneNode.SetPosition(contextualMousePosition);
                        activeBehaviour.DataSource.CreateNode(cloneNode, isCopied: true);
                        lastNodePosition = node.GetPosition();
                    }
                });
            }
        }
        #endregion

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