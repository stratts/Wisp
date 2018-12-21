using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Wisp.Nodes;
using Wisp.Handlers;
using Wisp.Components;

namespace Wisp
{
    public interface IProcessHandler
    {
        void Process(Node node, Component component, Scene scene);
    }

    public interface IEventHandler
    {
        void Process(Event e, Scene scene);
    }

    public interface IEventHandler<T> where T : Event
    {
        void Process(T e, Scene scene);
    }

    public delegate void HandleEvent(Event e, Scene scene);

    public class Process
    {
        Scene scene;

        private HashSet<Type> orderedComponents = new HashSet<Type>();
        private List<Type> componentOrder = new List<Type>();
        private Dictionary<Type, bool> processEnabled;

        private Dictionary<Type, List<HandleEvent>> eventHandlers;

        public Process(Scene scene)
        {
            this.scene = scene;

            processEnabled = new Dictionary<Type, bool>();
            eventHandlers = new Dictionary<Type, List<HandleEvent>>();

            AddComponent<AI>();
            AddComponent<Input>();
            AddComponent<Script>();
            AddComponent<Moveable>();
            AddComponent<Animated>();
            AddComponent<Parallax>();
            AddComponent<Collidable>();
            AddComponent<Lifetime>();
            AddComponent<ConstantAnim>();

            AddEventHandler(new ApplyCollision());
        }

        public void ProcessComponents()
        {
            foreach (var type in componentOrder)
            {
                ProcessComponentType(scene, type);
            }

            foreach (var type in scene.NodeManager.ComponentTypes)
            {
                if (!orderedComponents.Contains(type))
                {
                    ProcessComponentType(scene, type);
                }
            }
        }

        private void ProcessComponentType(Scene scene, Type type)
        {
            var components = scene.NodeManager.GetComponents(type);

            if (!processEnabled.ContainsKey(type)) processEnabled[type] = true;
            processEnabled.TryGetValue(type, out var enabled);

            if (components != null && enabled)
            {
                foreach (var component in components)
                {
                    component.Update(scene);
                }
            }
        }

        public void ProcessEvents()
        {
            foreach (var type in scene.Events.Keys)
            {
                eventHandlers.TryGetValue(type, out List<HandleEvent> handlers);

                if (handlers != null)
                {
                    foreach (var handler in handlers)
                    {
                        foreach (Event e in scene.Events[type])
                        {
                            handler(e, scene);
                        }
                    }
                }
            }
        }

        public void AddComponent(Type type)
        {
            orderedComponents.Add(type);
            componentOrder.Add(type);
            EnableProcessing(type);
        }

        private void AddComponent<T>() where T : Component
        {
            AddComponent(typeof(T));
        }

        public void AddEventHandler(Type type, HandleEvent handler)
        {
            eventHandlers.TryGetValue(type, out List<HandleEvent> list);
            if (list == null) eventHandlers[type] = new List<HandleEvent>();
            eventHandlers[type].Add(handler);
        }

        public void AddEventHandler<T>(IEventHandler<T> handler) where T : Event
        {
            var type = typeof(T);
            eventHandlers.TryGetValue(type, out List<HandleEvent> list);
            if (list == null) eventHandlers[type] = new List<HandleEvent>();
            HandleEvent handle = (e, scene) => { handler.Process((T)e, scene); };
            eventHandlers[type].Add(handle);
        }

        public void EnableProcessing(Type type) => processEnabled[type] = true;

        public void EnableProcessing<T>() where T : Component => EnableProcessing(typeof(T));

        public void DisableProcessing(Type type) => processEnabled[type] = false;

        public void DisableProcessing<T>() where T : Component => DisableProcessing(typeof(T));

        public bool ProcessingEnabled<T>() where T : Component
        {
            return processEnabled[typeof(T)];
        }
    }
}
