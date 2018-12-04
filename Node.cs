using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Wisp.Nodes;
using Wisp.Components;

namespace Wisp
{
    public class Node
    {
        
        public Node parent = null;
        public string Name { get; set; } = null;
        private Vector2 pos;
        public Vector2 ScenePos { get; private set; }
        public int Layer { get; set; }
        public Point Size { get; set; }

        private List<Node> children = new List<Node>();
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        public IReadOnlyDictionary<Type, Component> Components { get { return components; } }
        public IReadOnlyList<Node> Children { get { return children;  } }

        public NodeManager nodeManager;
        protected bool _active = true;

        public int Depth
        {
            get
            {
                if (parent != null) return parent.Depth + 1;
                else return 0;
            }
        }

        public Vector2 Pos
        {
            get { return pos; }
            set
            {
                pos = value;
                UpdateScenePos();
                if (parent != null) parent.SortChildren();
            }
        }

        public void UpdateScenePos()
        {
            if (parent != null) ScenePos = parent.ScenePos + pos;
            else ScenePos = pos;
            DepthSortValue = (int)ScenePos.Y + Size.Y;
            foreach (var child in children) child.UpdateScenePos();
        }

        public int SceneLayer {
            get { return parent != null ? parent.SceneLayer : Layer; }
        }

        public Node Root => parent != null ? parent.Root : this;

        public int DepthSortValue { get; private set; } = 0;

        public bool active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                NodesChanged();
            }
        }

        public T AddComponent<T>() where T : Component, new()
        {
            return (T)AddComponent(new T());
        }

        public Component AddComponent(Component component)
        {
            component.Parent = this;
            components[component.GetType()] = component;
            NodesChanged();
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            NodesChanged();
            components.Remove(typeof(T));
        }

        public void EnableComponent<T>() where T : Component => EnableComponent(typeof(T));

        public void EnableComponent(Type type) => GetComponent(type).Enabled = true;

        public void DisableComponent<T>() where T : Component => DisableComponent(typeof(T));

        public void DisableComponent(Type type) => GetComponent(type).Enabled = false;

        public T GetComponent<T>() where T : Component => (T)GetComponent(typeof(T));

        public Component GetComponent(Type type) 
        {
            components.TryGetValue(type, out Component component);
            return component;
        }

        public bool HasComponent<T>() where T : Component
        {
            return components.ContainsKey(typeof(T));
        }

        public Node AddChild(Node node)
        {
            node.parent = this;
            node.UpdateScenePos();
            children.Add(node);
            NodesChanged();

            return node;
        }

        public void RemoveChild(Node node)
        {
            children.Remove(node);
            NodesChanged();
        }

        public void SortChildren() => children.Sort(NodeSorter.Compare);

        public T GetFirstChildByType<T>() where T : Node
        {
            foreach (var node in Children)
            {
                if (node is T) return (T)node;
            }
            return null;
        }

        public Node GetFirstChildByType(Type type)
        {
            foreach (var node in Children)
            {
                if (node.GetType() == type) return node;
            }

            return null;
        }

        private void NodesChanged()
        {
            if (nodeManager != null)
            {
                nodeManager.nodesChanged = true;
            }
            else if (parent != null) parent.NodesChanged();
        }

        public Vector2 Centre => Size.ToVector2() / 2;
        public Vector2 CentrePos => Pos + Centre;    
    }

    public class NodeManager
    {
        public List<Node> Nodes { get; private set; }
        private Dictionary<Type, List<Component>> components;
        public bool nodesChanged;
        public int nodeChanges = 0;
        
        public NodeManager()
        {
            Nodes = new List<Node>();
            components = new Dictionary<Type, List<Component>>();
        }

        public void AddNode(Node node)
        {
            node.nodeManager = this;
            node.active = true;

            Nodes.Add(node);
            nodesChanged = true;
        }

        public void AddNode(Node node, int layer)
        {
            node.Layer = layer;
            AddNode(node);
        }

        public void AddNode(Node node, Vector2 pos, int layer = -1)
        {    
            node.Pos = pos;
            if (layer == -1) AddNode(node);
            else AddNode(node, layer);
        }

        public void RemoveNode(Node node)
        {
            node.nodeManager = null;
            node.active = false;

            Nodes.Remove(node);
            nodesChanged = true;
        }

        public void ClearNodes() {
            Nodes.Clear();
            components.Clear();
        }

        public Node GetNode(string name)
        {
            foreach (var node in Nodes)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }

            return null;
        }
 
        public IReadOnlyCollection<Component> GetComponents(Type type)
        {
            components.TryGetValue(type, out var list);
            if (list != null) return list;
            return new List<Component>(0);
        }

        public IReadOnlyCollection<Component> GetComponents<T>() where T : Component
        {
            return GetComponents(typeof(T));
        }

        public IReadOnlyCollection<Node> GetNodesByComponent(Type type)
        {
            var results = new List<Node>();
            foreach (var component in GetComponents(type)) results.Add(component.Parent);
            return results;
        }

        public IReadOnlyCollection<Node> GetNodesByComponent<T>() where T: Component
        {
            return GetNodesByComponent(typeof(T));
        }

        public void Update() {
            if (nodesChanged) UpdateNodes();      
        }

        private void UpdateNodes()
        {
            components.Clear();
            foreach (var node in Nodes) UpdateNode(node);
            nodesChanged = false;
        }

        private void UpdateNode(Node node)
        {
            if (!node.active) return;
            node.UpdateScenePos();
            AddComponents(node);
            foreach (var child in node.Children) UpdateNode(child);
        }

        private void AddComponents(Node node)
        {
            foreach (var item in node.Components) {
                AddComponent(item.Key, item.Value);
            }
        }

        private void AddComponent(Type type, Component component)
        {
            if (!components.ContainsKey(type))
            {
                components[type] = new List<Component>();
            }

            components[type].Add(component);
        }      
    }

    static class NodeSorter
    {
        public static int Compare(Node a, Node b) 
        {
            var diff = a.Layer - b.Layer;
            if (diff == 0) diff = a.DepthSortValue - b.DepthSortValue;
            return diff;
        }
    }
}
