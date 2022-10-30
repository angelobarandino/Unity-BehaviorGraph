//using UnityEditor.Experimental.GraphView;

//namespace BehaviorGraph.Editor
//{
//    public static class PortExtensions
//    {
//        public static int GetPortIndex(this Port port)
//        {
//            var btPort = port as TaskPort;

//            switch (btPort.FromLocation)
//            {
//                case TaskPort.PortLocation.Default:
//                    return btPort.Index;
//                case TaskPort.PortLocation.First:
//                    return -1;
//                case TaskPort.PortLocation.Last:
//                    return btPort.Index;
//            }

//            throw new System.Exception("Unknow port location origin");
//        }

//        public static TaskPort.PortLocation GetPortLocation(this Port port)
//        {
//            var btPort = port as TaskPort;
//            return btPort.Location;
//        }

//        public static void ClearFromLocation(this Port port)
//        {
//            (port as TaskPort).FromLocation = TaskPort.PortLocation.Default;
//        }
//    }
//}
