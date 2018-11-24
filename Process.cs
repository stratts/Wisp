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
            AddHandler<Script>(new ScriptHandler());          
            AddHandler<Moveable>(new MovementHandler());
            AddHandler<Parallax>(new ParallaxHandler());
            AddHandler<Collidable>(new CollisionHandler());
            AddHandler<Animated>(new AnimationHandler());
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
        

        private void AddHandler<T>(IProcessHandler handler) where T : Component
        {
            processHandlers.Add(typeof(T), handler);
            processHandlerList.Add(typeof(T));
            EnableProcessing<T>();
        }

        private void AddEventHandler<T>(IEventHandler handler) where T : Event
        {
            var type = typeof(T);
            eventHandlers.TryGetValue(type, out List<IEventHandler> list);

            if (list == null) eventHandlers[type] = new List<IEventHandler>();
            eventHandlers[type].Add(handler);
        }

        public void EnableProcessing<T>() where T : Component
        {
            processEnabled[typeof(T)] = true;
        }

        public void DisableProcessing<T>() where T : Component
        {
            processEnabled[typeof(T)] = false;
        }

        public bool ProcessingEnabled<T>() where T : Component
        {
            return processEnabled[typeof(T)];
        }
    }
}
