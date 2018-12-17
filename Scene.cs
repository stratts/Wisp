using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Wisp
{
    public enum SceneType { Background, World, HUD };

    interface ISceneLogic
    {
        void Load(Scene scene);
        void Update(Scene scene);
    }

    public class Scene<TEnum> : Scene
    {
        public void AddNode(Node node, TEnum layer) =>
            AddNode(node, Convert.ToInt32(layer));
        public void AddNode(Node node, Vector2 pos, TEnum layer) =>
            AddNode(node, pos, Convert.ToInt32(layer));
    }

    public class Scene
    {
        public readonly string name;

        public SceneManager sceneManager;
        public NodeManager NodeManager { get; private set; }
        public Process process;
        public Camera camera;
        public float elapsedTime;
        public float friction = 5f;

        public Point viewport;

        public bool loaded = false;
        public bool update = true;

        public float updateInterval = 0;
        public float lastUpdate = 0;

        private List<Event> newEvents = new List<Event>();
        private Dictionary<Type, List<Event>> events = new Dictionary<Type, List<Event>>();

        public IReadOnlyDictionary<Type, List<Event>> Events { get { return events; } }

        public Scene()
        {
            NodeManager = new NodeManager();
            process = new Process(this);
            name = GetType().Name;
        }

        public void SetViewport(int width, int height)
        {
            viewport = new Point(
                width,
                height
                );
            camera = new Camera(viewport);
        }

        public virtual void Load()
        {
            NodeManager.Update();
            loaded = true;
            update = true;
        }

        public virtual void Unload()
        {
            NodeManager.ClearNodes();
            events.Clear();
            newEvents.Clear();
            loaded = false;
            update = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            NodeManager.Update();
            process.ProcessComponents();

            UpdateEvents();
            newEvents.Clear();

            process.ProcessEvents();
            events.Clear();
        }

        public void AddEvent(Event e)
        {
            newEvents.Add(e);
        }

        public List<Event> GetEvents<T>() where T : Event
        {
            events.TryGetValue(typeof(T), out List<Event> list);

            if (list != null) return list;
            return new List<Event>();
        }

        private void UpdateEvents()
        {
            foreach (Event e in newEvents)
            {
                var type = e.GetType();
                if (!events.ContainsKey(type)) events[type] = new List<Event>();
                events[type].Add(e);
            }
        }

        private void ClearEvents()
        {
            foreach (var list in events.Values)
            {
                list.Clear();
            }
        }

        public void AddNode(Node node) { NodeManager.AddNode(node); }
        public void AddNode(Node node, int layer) { NodeManager.AddNode(node, layer); }
        public void AddNode(Node node, Vector2 pos, int layer = -1) => NodeManager.AddNode(node, pos, layer);
        public void RemoveNode(Node node) { NodeManager.RemoveNode(node); }
    }
}