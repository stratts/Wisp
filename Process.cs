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

    public class Process
    {
        Scene scene;
        
        private Dictionary<Type, IProcessHandler> processHandlers;
        private List<Type> processHandlerList = new List<Type>();
        private Dictionary<Type, bool> processEnabled;

        private Dictionary<Type, List<IEventHandler>> eventHandlers;

        public Process(Scene scene)
        {
            this.scene = scene;

            processHandlers = new Dictionary<Type, IProcessHandler>();
            processEnabled = new Dictionary<Type, bool>();
            eventHandlers = new Dictionary<Type, List<IEventHandler>>();

            AddHandler<AI>(new AIHandler());
            AddHandler<Input>(new ScriptHandler());
            AddHandler<Script>(new ScriptHandler());          
            AddHandler<Moveable>(new MovementHandler());
            AddHandler<Animated>(new AnimationHandler());
            AddHandler<Parallax>(new ParallaxHandler());
            AddHandler<Collidable>(new CollisionHandler());
            AddHandler<Lifetime>(new LifetimeHandler());
            AddHandler<ConstantAnim>(new ConstantAnimHandler());
            
            AddEventHandler<CollisionEvent>(new ApplyCollision());
            AddEventHandler<CollisionEvent>(new PortalHandler());
        }

        public void ProcessComponents()
        {
            foreach (var type in processHandlerList)
            {
                var components = scene.NodeManager.GetComponents(type);

                if (components != null && processEnabled[type])
                {
                    var handler = processHandlers[type];

                    foreach (var component in components)
                    {
                        if (!component.Enabled) continue;
                        var node = component.Parent;
                        handler.Process(node, component, scene);
                    }
                }
            }
        }

        public void ProcessEvents()
        {
            foreach (var type in scene.Events.Keys)
            {
                eventHandlers.TryGetValue(type, out List<IEventHandler> handlers);

                if (handlers != null)
                {
                    foreach (var handler in handlers)
                    { 
                        foreach (Event e in scene.Events[type])
                        {
                            handler.Process(e, scene);
                        }
                    }
                }
            }
        }   
        
        public void AddHandler(Type type, IProcessHandler handler)
        {
            processHandlers.Add(type, handler);
            processHandlerList.Add(type);
            EnableProcessing(type);
        }

        private void AddHandler<T>(IProcessHandler handler) where T : Component
        {
            AddHandler(typeof(T), handler);
        }

        public void AddEventHandler(Type type, IEventHandler handler)
        {
            eventHandlers.TryGetValue(type, out List<IEventHandler> list);
            if (list == null) eventHandlers[type] = new List<IEventHandler>();
            eventHandlers[type].Add(handler);
        }

        private void AddEventHandler<T>(IEventHandler handler) where T : Event
        {
            AddEventHandler(typeof(T), handler);
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
