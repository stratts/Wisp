using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        public IKeyService keyService { get; protected set; }
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

        public virtual void Load(Game game)
        {
            NodeManager.Update();
            keyService = game.Services.GetService<IKeyService>();
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

    class Transition
    {
        public Scenes.Transition TransitionScene { get; set; }
        public SceneType Target { get; set; }
        public string NextScene { get; set; }
        public bool Unload { get; set; }
    }

    public class SceneManager
    {
        public SceneRender Renderer;

        private Game game;
        private GraphicsDeviceManager graphics;
        private Dictionary<string, Scene> activeScenes;
        private Dictionary<string, Type> sceneTypes = new Dictionary<string, Type>();
        public Dictionary<SceneType, Scene> CurrentScenes;
        public Dictionary<SceneType, Scene> NextScenes;
        private List<Scene> UIScenes;
        private List<SceneType> renderOrder;
        private Stack<Scene> toRemove;
        private Transition transition = null;

        private Dictionary<Type, Type> customHandlers = new Dictionary<Type, Type>();
        private List<(Type, Type)> customEventHandlers = new List<(Type, Type)>();

        public SceneManager(Game game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            this.graphics = graphics;
            activeScenes = new Dictionary<string, Scene>();
            CurrentScenes = new Dictionary<SceneType, Scene>();
            NextScenes = new Dictionary<SceneType, Scene>();
            UIScenes = new List<Scene>();
            Renderer = new SceneRender(game);
            toRemove = new Stack<Scene>();

            AddScene<Scenes.Fade>();
            AddScene<Scenes.Slide>();
        }

        public Scene CreateScene(string name)
        {
            Type sceneClass = sceneTypes[name];
            var scene = (Scene)Activator.CreateInstance(sceneClass);
            scene.sceneManager = this;
            scene.SetViewport(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            foreach (var handler in customHandlers)
                scene.process.AddHandler(handler.Key, (IProcessHandler)Activator.CreateInstance(handler.Value));
            foreach (var handler in customEventHandlers)
                scene.process.AddEventHandler(handler.Item1, (IEventHandler)Activator.CreateInstance(handler.Item2));
            return scene;
        }

        public void AddScene<T>() where T : Scene 
        {
            var type = typeof(T);
            sceneTypes.Add(type.Name, type);
        }

        public void AddHandler<T1, T2>() where T1 : Components.Component where T2 : IProcessHandler
        {
            customHandlers.Add(typeof(T1), typeof(T2));
        }

        public void AddEventHandler<T1, T2>() where T1 : Event where T2 : IEventHandler
        {
            customEventHandlers.Add((typeof(T1), typeof(T2)));
        }

        private void AddScene(string name)
        {
            var scene = CreateScene(name);
            activeScenes.Add(scene.name, scene);
        }

        private void RemoveScene(string name)
        {
            var scene = GetScene(name);

            foreach (SceneType type in NextScenes.Keys)
            {
                if (NextScenes[type] == scene)
                {
                    NextScenes.Remove(type);
                    break;
                }
            }

            activeScenes.Remove(name);
        }

        public void AddUI(string name)
        {
            var scene = CreateScene(name);
            scene.Load(game);
            UIScenes.Add(scene);
            CurrentScenes[SceneType.World].process.DisableProcessing<Components.Input>();
        }

        public void RemoveUI()
        {
            if (UIScenes.Count > 0) toRemove.Push(UIScenes[UIScenes.Count - 1]);
            if (UIScenes.Count == 1)
                CurrentScenes[SceneType.World].process.EnableProcessing<Components.Input>();
        }

        public void RemoveUI(string name)
        {
            foreach (var scene in UIScenes)
            {
                if (scene.name == name) toRemove.Push(scene);
            }
        }

        private void RemoveUIScenes()
        {
            while (toRemove.Count > 0) UIScenes.Remove(toRemove.Pop());
        }

        public Scene GetScene(string name)
        {
            if (!activeScenes.ContainsKey(name)) AddScene(name);
            return activeScenes[name];
        }

        public bool IsCurrentScene(SceneType type, string name)
        {
            if (!CurrentScenes.ContainsKey(type)) return false;
            return CurrentScenes[type].name == name;
        }

        public bool IsCurrentScene(string name) => NextScenes.ContainsValue(GetScene(name));

        public void SetCurrentScene(SceneType type, string name)
        {
            if (!activeScenes.ContainsKey(name)) AddScene(name);
            var scene = GetScene(name);
            scene.update = true;
            NextScenes[type] = scene;
            if (!scene.loaded) scene.Load(game);
        }

        public void ChangeScene(SceneType type, string name, string transition, bool unload)
        {
            this.transition = new Transition()
            {
                TransitionScene = (Scenes.Transition)CreateScene(transition),
                NextScene = name,
                Target = type,
                Unload = unload
            };

            CurrentScenes[type].update = false;

            this.transition.TransitionScene.Load(game);
            activeScenes.TryGetValue(name, out var scene);
            if (scene != null) scene.update = false;
        }

        public void UpdateCurrentScenes(GameTime gameTime)
        {
            CurrentScenes.Clear();

            foreach (var scene in NextScenes)
            {
                CurrentScenes.TryGetValue(scene.Key, out Scene current);
                var next = scene.Value;
                
                if (current == null || current != next)
                {
                    CurrentScenes[scene.Key] = next;
                    //if (current != null) current.Unload();
                }
            }

            UpdateScenes(CurrentScenes.Values, gameTime);
            UpdateScenes(UIScenes, gameTime);
            RemoveUIScenes();
            if (transition != null) UpdateTransition(gameTime);
        }

        public void ClearScenes(ICollection<SceneType> types)
        {
            foreach (var type in types)
            {
                NextScenes.TryGetValue(type, out var scene);
                if (scene != null)
                {
                    activeScenes.Remove(scene.name);
                    scene.Unload();
                    NextScenes.Remove(type);            
                }
            }
        } 

        public void ClearUI()
        {
            while (toRemove.Count < UIScenes.Count) RemoveUI();
        }

        private void UpdateScenes(IEnumerable<Scene> scenes, GameTime gameTime)
        {
            foreach (var scene in scenes)
            {
                if (scene != null && scene.update)
                {    
                    scene.Update(gameTime);
                }
            }
        }

        private void UpdateTransition(GameTime gameTime)
        {
            var transitionScene = transition.TransitionScene;
            transitionScene.Update(gameTime);

            if (!transitionScene.FinishedOut)
            {
                transitionScene.TransitionOut();
            }
            else if (!transitionScene.FinishedIn)
            {
                if (!IsCurrentScene(transition.Target, transition.NextScene))
                {
                    var current = CurrentScenes[transition.Target].name;
                    if (transition.Unload) RemoveScene(current);
                    else SetCurrentScene(SceneType.Background, current);
                    SetCurrentScene(transition.Target, transition.NextScene);   
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }       

                transitionScene.TransitionIn();
            }
            else
            {
                transition = null;
            }
        }

        public void RenderCurrentScenes(SpriteBatch spriteBatch)
        {
            Renderer.NumDrawn = 0;

            foreach (var sceneType in renderOrder)
            {
                CurrentScenes.TryGetValue(sceneType, out Scene scene);

                if (scene != null)
                {
                    RenderScene(scene, spriteBatch);
                }
            }

            foreach (var scene in UIScenes)
            {
                RenderScene(scene, spriteBatch);
            }

            if (transition != null) RenderScene(transition.TransitionScene, spriteBatch);
        }

        private void RenderScene(Scene scene, SpriteBatch spriteBatch)
        {
            scene.camera.Update();
            Renderer.Render(scene, spriteBatch);
        }

        public void SetRenderOrder(IEnumerable<SceneType> order)
        {
            renderOrder = new List<SceneType>(order);
        }
    }
}