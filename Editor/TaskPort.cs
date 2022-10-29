//using UnityEditor.Experimental.GraphView;
//using UnityEngine.UIElements;

//namespace BehaviourGraph.Editor
//{
//    public class TaskPort : Port
//    {
//        public int Index { get; set; }
//        public PortLocation Location { get; }
//        public PortLocation FromLocation { get; set; }
//        public string ChildRef { get; set; }

//        public enum PortLocation
//        {
//            Default,
//            First,
//            Last
//        }

//        protected TaskPort(PortLocation type, Direction direction) : base(Orientation.Vertical, direction, Capacity.Single, typeof(Runtime.Tasks.Task))
//        {
//            portName = string.Empty;

//            Location = type;
//        }

//        public static TaskPort Create(NodeView nodeView, Direction direction, PortLocation type)
//        {
//            var listener = new EdgeConnectorListener(nodeView);
//            var port = new TaskPort(type, direction)
//            {
//                m_EdgeConnector = new EdgeConnector<Edge>(listener)
//            };
//            port.AddManipulator(port.m_EdgeConnector);
//            return port;
//        }

//        public static TaskPort CreateInput(NodeView nodeView) => Create(nodeView, Direction.Input, PortLocation.Default);
//        public static TaskPort CreateDefault(NodeView nodeView) => Create(nodeView, Direction.Output, PortLocation.Default);
//        public static TaskPort CreateFirst(NodeView nodeView) => Create(nodeView, Direction.Output, PortLocation.First);
//        public static TaskPort CreateLast(NodeView nodeView) => Create(nodeView, Direction.Output, PortLocation.Last);
//    }
//}
