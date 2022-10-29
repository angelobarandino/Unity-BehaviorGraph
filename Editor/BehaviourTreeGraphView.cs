//using System.Collections.Generic;
//using System.Linq;
//using BehaviourGraph.Runtime;
//using BehaviourGraph.Runtime.Tasks;
//using UnityEditor;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.SceneManagement;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

//namespace BehaviourGraph.Editor
//{


//    public class BehaviourTreeGraphView : GraphView, IGraphView
//    {
//        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, GraphView.UxmlTraits> { }

//        private IBehaviour behaviour;
//        public IBehaviourOwner BehaviourOwner => behaviour as IBehaviourOwner;

//        private readonly TaskInspectorProvider taskInspectorProvider;
//        private readonly BlackboardProvider blackboardProvider;
//        public BehaviourTreeGraphView()
//        {
//            Insert(0, new GridBackground());
//            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
//            this.AddManipulator(new ContentDragger());
//            this.AddManipulator(new SelectionDragger());
//            this.AddManipulator(new RectangleSelector());
//            this.AddManipulator(new ClickSelector());

//            this.AddStyleSheet("Assets/BehaviourGraph/Editor/Resources/StyleSheets/BehaviourTreeEditorStyle.uss");

//            LoadViewTransform();

//            blackboardProvider = new BlackboardProvider(this);
//            taskInspectorProvider = new TaskInspectorProvider(this);
//        }

//        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
//        {
//            return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
//        }

//        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
//        {

//            if (evt.target is BehaviourTreeGraphView)
//            {
//                var mousePosition = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);

//                ContextMenuCreator.AddTaskMenus(evt, behaviour, mousePosition);
//                evt.menu.AppendSeparator();
//                ContextMenuCreator.PasteMenu(evt, behaviour, mousePosition);
//            }

//            if (evt.target is NodeView nodeView)
//            {
//                if (!nodeView.task.Data.IsStart)
//                {
//                    evt.menu.AppendAction("Set Start", (a) =>
//                    {
//                        behaviour.DataSource.SetStartTask(nodeView.task);
//                        nodes.ForEach(n => (n as NodeView).RefreshNodeView());
//                    });

//                    evt.menu.AppendSeparator();
//                }

//                ContextMenuCreator.CopyAndDeleteMenus(evt, this);
//                evt.menu.AppendSeparator();
                
//                ContextMenuCreator.ReplaceMenus(evt, behaviour, nodeView);
//                ContextMenuCreator.DecorateMenus(evt, behaviour, nodeView);
//            }
//        }

//        public void PopulateView(IBehaviour behaviour)
//        {
//            this.behaviour = behaviour;
//            this.behaviour.DataSource.OnDataSourceUpdate = OnNodeAdded;

//            blackboardProvider.Initialize();

//            graphViewChanged -= OnGraphViewChanged;
//            DeleteElements(graphElements);
//            graphViewChanged += OnGraphViewChanged;

//            behaviour.DataSource.AllNodes.ForEach(node => CreateNodeView(node as Task));
//            behaviour.DataSource.AllNodes.ForEach(node => CreateNodeEdges(node as Task));
            
//            SelectTask(null);
//        }

//        private void OnNodeAdded(Task task, TaskUpdateEvent taskEvent)
//        {
//            switch (taskEvent)
//            {
//                case TaskUpdateEvent.Add:
//                    CreateNodeView(task, selected: true);
//                    break;
//                case TaskUpdateEvent.Replace:
//                    var nodeView = FindNodeView(task);
//                    nodeView.UpdateTask(task);
//                    nodeView.MarkDirtyRepaint();
//                    break;
//                case TaskUpdateEvent.Decorate:
//                    CreateNodeView(task, selected: true);
//                    CreateNodeEdges(task);
//                    ReparentTask(task);
//                    break;
//                case TaskUpdateEvent.Remove:
//                    break;
//            }

//            SaveBehaviourTree();
//        }

//        private void CreateNodeView(Task node, bool selected = false)
//        {
//            var nodeView = new NodeView(node, this);
//            AddElement(nodeView);

//            if (selected)
//            {
//                var currentNode = nodes.FirstOrDefault(x => x.selected);
//                if (currentNode != null) currentNode.selected = false;
//                nodeView.selected = true;
//            }
//        }

//        private void CreateNodeEdges(Task task)
//        {
//            var children = task.GetChildren();
//            for (int i = 0; i < children.Count; i++)
//            {
//                var parent = FindNodeView(task);
//                var child = FindNodeView(children[i] as Task);
//                var edge = parent.outputPorts[i].ConnectTo(child.input);
//                AddElement(edge);
//            }
//        }

//        private void ReparentTask(Task task)
//        {
//            var nodeView = FindNodeView(task.Data.Parent);
//            if (nodeView != null)
//            {
//                nodeView.RefreshOutputConnectionForChildTask(task);
//            }
//        }

//        private void SaveBehaviourTree()
//        {
//            if (!Application.isPlaying)
//            {
//                PrefabUtility.RecordPrefabInstancePropertyModifications(behaviour as Object);
                
//                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
//            }
//        }

//        private void LoadViewTransform()
//        {
//            GraphViewTransform.Load(this);
//        }

//        public void SaveViewTransform()
//        {
//            GraphViewTransform.Save(viewTransform);
//        }

//        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
//        {
//            if (graphViewChange.elementsToRemove != null)
//            {
//                for (var i = 0; i < graphViewChange.elementsToRemove.Count; i++)
//                {
//                    var element = graphViewChange.elementsToRemove[i];
//                    if (element is NodeView nodeView)
//                    {
//                        behaviour.DataSource.RemoveTask(nodeView.task);
//                        SelectTask(null);
//                    }

//                    if (element is Edge edge)
//                    {
//                        var location = edge.output.GetPortLocation();
//                        if (location != TaskPort.PortLocation.Default)
//                            continue;

//                        if (edge.output.node is NodeView parent)
//                        {
//                            var child = edge.input.node as NodeView;
//                            var index = edge.output.GetPortIndex();
//                            behaviour.DataSource.ClearChild(parent.task, child.task, index);
//                            parent.RemovePortAt(index);
//                        }
//                    }
//                }
//            }

//            if (graphViewChange.edgesToCreate != null)
//            {
//                graphViewChange.edgesToCreate.ForEach(edge =>
//                {
//                    if (edge.output.node is NodeView parent)
//                    {
//                        var child = edge.input.node as NodeView;
//                        var portIndex = edge.output.GetPortIndex();
//                        behaviour.DataSource.SetChild(parent.task, child.task, portIndex);
//                        edge.output.ClearFromLocation();
//                    }
//                });
//            }

//            return graphViewChange;
//        }

//        public NodeView FindNodeView(string guid) => GetNodeByGuid(guid) as NodeView;

//        public NodeView FindNodeView(Task task) => FindNodeView(task?.Id);

//        public void UpdateNodeStates()
//        {
//            nodes.ForEach(node =>
//            {
//                if (node is NodeView nodeView)
//                {
//                    nodeView.UpdateNodeState();
//                }
//            });
//        }

//        public void SelectTask(Task task)
//        {
//            taskInspectorProvider.Inspect(task);
//        }

//        //public BehaviorSource GetBehaviourSource() => tree.GetBehaviourSource();

//        public List<GraphElement> GetSelectedElements()
//        {
//            return graphElements.Where(element => element.selected).ToList();
//        }

//        public void SelectNode(INode node)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}
