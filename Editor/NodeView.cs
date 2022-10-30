//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BehaviorGraph.Runtime.Tasks;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.UIElements;
//using static BehaviorGraph.Editor.TaskPort;
//using NodeGraphView = UnityEditor.Experimental.GraphView.Node;
//using TaskAction = BehaviorGraph.Runtime.Tasks.Action;

//namespace BehaviorGraph.Editor
//{
//    public class NodeView : NodeGraphView
//    {
//        private const string startTaskClassName = "startTask";
//        private const string playModeClassName = "playMode";

//        private VisualElement inputContainerElement;
//        private VisualElement outputContainerElement;

//        public Task task;
//        public TaskPort input;
//        public TaskPort outputFirst;
//        public TaskPort outputLast;
//        public List<TaskPort> outputPorts = new List<TaskPort>();

//        public bool IsStartNode { get; private set; }

//        private readonly IGraphView graphView;
//        public NodeView(Task task, IGraphView graphView) : base("Assets/BehaviorGraph/Editor/Resources/Uxml/NodeView.uxml")
//        {
//            this.graphView = graphView;

//            UpdateTask(task, false);
//            AddToClassList("nodeTask");

//            CreateDefaultInputPort();
//            CreateDefaultOutputPorts(task);

//            inputContainerElement = this.Q<VisualElement>(name: "input-container");
//            outputContainerElement = this.Q<VisualElement>(name: "output-container");
//        }

//        public override void SetPosition(Rect newPos)
//        {
//            base.SetPosition(newPos);

//            task.SetPosition(newPos.xMin, newPos.yMin);
//        }

//        public override void OnSelected()
//        {
//            base.OnSelected();
//            graphView.SelectTask(task);
//        }

//        public override void OnUnselected()
//        {
//            base.OnUnselected();
//            graphView.SelectTask(null);
//        }

//        private void UpdatePortIndexes()
//        {
//            for (int i = 0; i < outputPorts.Count; i++)
//            {
//                outputPorts[i].Index = i;
//            }
//        }

//        private void CreateDefaultInputPort()
//        {
//            inputContainer.Clear();
//            input = TaskPort.CreateInput(this);
//            input.portName = string.Empty;
//            inputContainer.Add(input);
//        }

//        private void CreateDefaultOutputPorts(Task task)
//        {
//            outputPorts.Clear();
//            outputContainer.Clear();

//            switch (task)
//            {
//                case Composite composite:
//                    outputFirst = TaskPort.CreateFirst(this);
//                    outputContainer.Add(outputFirst);

//                    if (composite.ChildCount > 0)
//                    {
//                        composite.GetChildren().ForEach(c =>
//                        {
//                            var port = TaskPort.CreateDefault(this);
//                            port.ChildRef = c.Id;
//                            outputContainer.Add(port);
//                            outputPorts.Add(port);
//                        });

//                        outputLast = TaskPort.CreateLast(this);
//                        outputContainer.Add(outputLast);
//                    }
//                    break;

//                case Decorator:
//                    var port = TaskPort.CreateDefault(this);
//                    outputContainer.Add(port);
//                    outputPorts.Add(port);
//                    break;
//            }

//            UpdatePortIndexes();
//        }

//        public TaskPort GetOrAddOutputPort(NodeView child, PortLocation location, TaskPort currentOutput)
//        {
//            switch (location)
//            {
//                case PortLocation.First:
//                    return InsertOutputPortAt(0, child.task.Id, location);
//                case PortLocation.Last:
//                    return InsertOutputPortAt(outputPorts.Count, child.task.Id, location);

//                case PortLocation.Default:
//                    return InsertOutputPortAt(currentOutput.Index, child.task.Id, location);
//            }

//            throw new System.Exception("Unknow port location origin");
//        }

//        private TaskPort InsertOutputPortAt(int index, string guid, PortLocation location)
//        {
//            var port = TaskPort.CreateDefault(this);
//            port.FromLocation = location;
//            port.ChildRef = guid;
//            outputContainer.Insert(index + 1, port);
//            outputPorts.Insert(index, port);

//            if (outputPorts.Count > 0 && outputLast == null)
//            {
//                outputLast = TaskPort.CreateLast(this);
//                outputContainer.Add(outputLast);
//            }

//            UpdatePortIndexes();
//            return port;
//        }

//        public void RemovePortAt(int index)
//        {
//            if (task is Composite)
//            {
//                var port = outputPorts[index];
//                outputContainer.Remove(port);
//                outputPorts.RemoveAt(index);

//                if (outputPorts.Count == 0 && outputLast != null)
//                {
//                    outputContainer.Remove(outputLast);
//                    outputLast = null;
//                }

//                UpdatePortIndexes();
//            }
//        }

//        public void UpdateNodeState()
//        {
//            var classList = Enum.GetNames(typeof(NodeState));
//            foreach (var state in classList)
//            {
//                RemoveFromClassList(state);
//                RemoveFromClassList(playModeClassName);

//                if (input != null)
//                {
//                    foreach (var edge in input.connections)
//                    {
//                        edge.input.RemoveFromClassList(state);
//                        edge.output.RemoveFromClassList(state);
//                        edge.RemoveFromClassList(state);
//                    }
//                }
//            }

//            if (Application.isPlaying)
//            {
//                var state = task.State.ToString();

//                AddToClassList(state);
//                AddToClassList(playModeClassName);

//                if (input != null)
//                {
//                    foreach (var edge in input.connections)
//                    {
//                        edge.input.AddToClassList(state);
//                        edge.output.AddToClassList(state);
//                        edge.AddToClassList(state);

//                        if (edge.output.node is NodeView nodeView)
//                        {
//                            if (nodeView.task is not Composite)
//                            {
//                                nodeView.outputContainerElement.ClearClassList();
//                                nodeView.outputContainerElement.AddToClassList(state);
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        public void RefreshNodeView()
//        {
//            if (IsStartNode)
//            {
//                IsStartNode = false;
//                RemoveFromClassList(startTaskClassName);
//            }

//            if (!IsStartNode && task.Data.IsStart)
//            {
//                IsStartNode = true;
//                AddToClassList(startTaskClassName);

//                if (input != null)
//                {
//                    var edgesToRemove = new List<Edge>();
//                    foreach (var edge in input.connections)
//                    {
//                        edge.output.DisconnectAll();
//                        edgesToRemove.Add(edge);
//                    }

//                    input.DisconnectAll();
//                    graphView.DeleteElements(edgesToRemove);
//                }
//            }
//        }

//        public void UpdateTask(Task newTask, bool refreshPorts = true)
//        {
//            task = newTask;
//            viewDataKey = task.Id;
//            title = newTask.Name.ToFriendlyName();

//            style.top = newTask.Data.position.y;
//            style.left = newTask.Data.position.x;

//            if (newTask.Data.IsStart)
//            {
//                IsStartNode = true;
//                AddToClassList(startTaskClassName);
//            }

//            if (refreshPorts)
//            {
//                switch (newTask)
//                {
//                    case Composite composite:
//                        if (outputFirst == null)
//                        {
//                            outputFirst = TaskPort.CreateFirst(this);
//                            outputContainer.Insert(0, outputFirst);
//                        }
//                        if (outputLast == null && outputPorts.Any())
//                        {
//                            outputLast = TaskPort.CreateLast(this);
//                            outputContainer.Insert(outputPorts.Count + 1, outputLast);
//                        }
//                        break;

//                    case Decorator:
//                        if (outputLast != null)
//                        {
//                            outputContainer.Remove(outputLast);
//                            outputLast = null;
//                        }
//                        if (outputFirst != null)
//                        {
//                            outputContainer.Remove(outputFirst);
//                            outputFirst = null;
//                        }
//                        if (!outputPorts.Any())
//                        {
//                            var port = TaskPort.CreateDefault(this);
//                            outputContainer.Add(port);
//                            outputPorts.Add(port);
//                        }
//                        else if (outputPorts.Count > 1)
//                        {
//                            var last = outputPorts.Last();
//                            DisconectAndRemovePortEdges(last);
//                            outputContainer.Remove(last);
//                            outputPorts.Remove(last);
//                        }
//                        break;

//                    case TaskAction:
//                        if (outputLast != null)
//                        {
//                            outputContainer.Remove(outputLast);
//                            outputLast = null;
//                        }
//                        if (outputFirst != null)
//                        {
//                            outputContainer.Remove(outputFirst);
//                            outputFirst = null;
//                        }
//                        outputPorts.ForEach(port =>
//                        {
//                            DisconectAndRemovePortEdges(port);
//                            outputContainer.Remove(port);
//                        });
//                        outputPorts.Clear();
//                        break;
//                }
//            }

//            OnSelected();
//        }

//        private void DisconectAndRemovePortEdges(TaskPort port)
//        {
//            var edgesToDelete = new List<Edge>();
//            foreach (var edge in port.connections)
//            {
//                edge.input.DisconnectAll();
//                edgesToDelete.Add(edge);
//            }
//            graphView.DeleteElements(edgesToDelete);
//        }

//        public void RefreshOutputConnectionForChildTask(Task task)
//        {
//            if (this.task is Composite composite)
//            {
//                var children = composite.GetChildren();
//                var index = children.IndexOf(task);
//                graphView.DeleteElements(outputPorts[index].connections);

//                var port = TaskPort.CreateDefault(this);
//                outputContainer.Insert(index + 1, port);
//                outputPorts.Insert(index, port);

//                var childNode = graphView.FindNodeView(children[index].Id);
//                graphView.AddElement(port.ConnectTo(childNode.input));
                
//                UpdatePortIndexes();
//            }
//        }
//    }
//}
