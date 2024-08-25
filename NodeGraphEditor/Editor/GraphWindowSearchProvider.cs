using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{

    public struct SearchContextElement
    {
        public object target { get; private set; }
        public string title { get; private set; }

        public SearchContextElement(object target, string title)
        {
            this.target = target;
            this.title = title;
        }
    }
    public class GraphWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {

        public BaseGraphView graph;
        public VisualElement target;
        public static List<SearchContextElement> elements;
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
    {
        new SearchTreeGroupEntry(new GUIContent("Nodes"), 0)
    };

            elements = new List<SearchContextElement>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = type.GetCustomAttribute<NodeInfoAttribute>();
                    if (attribute != null)
                    {
                        NodeInfoAttribute nodeInfo = (NodeInfoAttribute)attribute;
                        var node = Activator.CreateInstance(type);
                        if (!string.IsNullOrEmpty(nodeInfo.menuItem))
                        {
                            elements.Add(new SearchContextElement(node, nodeInfo.menuItem));
                        }
                    }
                }
            }

            elements.Sort((entry1, entry2) => {
                return entry1.title.CompareTo(entry2.title);
            });

            HashSet<string> groups = new HashSet<string>();

            foreach (SearchContextElement element in elements)
            {
                string[] entryTitle = element.title.Split('/');
                string groupName = "";

                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (groups.Add(groupName)) // Only adds if it doesn't already exist
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    }
                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()))
                {
                    level = entryTitle.Length,
                    userData = element
                };
                tree.Add(entry);
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = graph.ChangeCoordinatesTo(graph, context.screenMousePosition - graph.window.position.position);
            var graphMousePosition = graph.contentViewContainer.WorldToLocal(windowMousePosition);

            if (searchTreeEntry.userData is SearchContextElement element)
            {
                if (element.target is BaseGraphNode node)
                {
                    node.SetPosition(new Rect(graphMousePosition, Vector2.zero));
                    graph.Add(node);
                    return true;
                }
            }
            return false;
        }

    }
}
