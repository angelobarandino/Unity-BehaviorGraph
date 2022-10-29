﻿using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourGraph.Editor.Ports
{
    public class NodePort : Port
    {
        protected NodePort(Direction portDirection, Capacity portCapacity) : base(Orientation.Vertical, portDirection, portCapacity, typeof(INode))
        {
            this.Q<Label>("type").text = string.Empty;
        }

        public static NodePort Create(Direction direction, Capacity capacity)
        {
            var listener = new NodeEdgeConnectorListener();
            var port = new NodePort(direction, capacity)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(listener)
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.portCapLit = false;
            return port;
        }
    }

    public class BoxEl : GraphElement
    {
        public BoxEl()
        {
            var box = new VisualElement();
            box.style.width = 10f;
            box.style.height = 10f;
            box.style.backgroundColor = Color.red;
            Add(box);
        }
    }

    public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public SearchWindowProvider()
        {
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Add Task"), 0),

                new SearchTreeGroupEntry(new GUIContent("Actions"), 1),
                new SearchTreeEntry(new GUIContent("Log")) { level = 2 },
                new SearchTreeEntry(new GUIContent("Wait")) { level = 2 },

                new SearchTreeGroupEntry(new GUIContent("Composites"), 1),
                new SearchTreeEntry(new GUIContent("Selector")) { level = 2 },
                new SearchTreeEntry(new GUIContent("Sequence")) { level = 2 },
                new SearchTreeEntry(new GUIContent("Parallel")) { level = 2 },

                new SearchTreeGroupEntry(new GUIContent("Conditions"), 1),
                new SearchTreeGroupEntry(new GUIContent("Decoratos"), 1),
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            return true;
        }
    }
}