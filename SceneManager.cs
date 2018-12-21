using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Wisp.Scenes;

namespace Wisp
{
    class TransitionDef
    {
        public Transition TransitionScene { get; set; }
        public SceneType Target { get; set; }
        public Scene NextScene { get; set; }
        public bool Unload { get; set; }
    }

    public class SceneManager
    {
        private GraphicsDeviceManager graphics;
        public Dictionary<SceneType, Scene> CurrentScenes;
        public Dictionary<SceneType, Scene> NextScenes;
        private List<Scene> UIScenes;
        private List<SceneType> renderOrder;
        private Stack<Scene> toRemove;
        private TransitionDef transition = null;

        private List<Type> customHandlers = new List<Type>();
        private List<(Type, HandleEvent)> customEventHandlers = new List<(Type, HandleEvent)>();

        public SceneManager(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            CurrentScenes = new Dictionary<SceneType, Scene>();
            NextScenes = new Dictionary<SceneType, Scene>();
            UIScenes = new List<Scene>();
            toRemove = new Stack<Scene>();
        }

        private void SetupScene(Scene scene)
        {
            scene.sceneManager = this;
            scene.SetViewport(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            foreach (var handler in customHandlers)
                scene.process.AddComponent(handler);
            foreach (var handler in customEventHandlers)
                scene.process.AddEventHandler(handler.Item1, handler.Item2);
        }

        public void AddHandler<T1>() where T1 : Component
        {
            customHandlers.Add(typeof(T1));
        }

        public HandleEvent AddEventHandler<T>(IEventHandler<T> handler) where T : Event
        {
            HandleEvent handle = (e, scene) => { handler.Process((T)e, scene); };
            customEventHandlers.Add((typeof(T), handle));
            return handle;
        }

        private Scene GetScene(string name)
        {
            foreach (var scene in CurrentScenes.Values)
            {
                if (scene != null && scene.name == name) return scene;
            }

            return null;
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
        }

        public void AddUI(Scene scene)
        {
            SetupScene(scene);
            scene.Load();
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

        public bool IsCurrentScene(SceneType type, string name)
        {
            if (!CurrentScenes.ContainsKey(type)) return false;
            return CurrentScenes[type].name == name;
        }

        public void SetCurrentScene(SceneType type, Scene scene)
        {
            if (NextScenes.ContainsKey(type)) NextScenes[type].Unload();
            scene.update = true;
            SetupScene(scene);
            NextScenes[type] = scene;
            if (!scene.loaded) scene.Load();
        }

        public void ChangeScene(SceneType type, Scene scene, Transition transition, bool unload)
        {
            this.transition = new TransitionDef()
            {
                TransitionScene = transition,
                NextScene = scene,
                Target = type,
                Unload = unload
            };

            CurrentScenes[type].update = false;

            SetupScene(transition);
            transition.Load();
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
                if (!IsCurrentScene(transition.Target, transition.NextScene.name))
                {
                    /*var current = CurrentScenes[transition.Target].name;
                    /*if (transition.Unload) 
                    RemoveScene(current);*/
                    //else SetCurrentScene(SceneType.Background, current);
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

        public IEnumerable<Scene> Scenes
        {
            get
            {
                foreach (var sceneType in renderOrder)
                {
                    CurrentScenes.TryGetValue(sceneType, out Scene scene);

                    if (scene != null)
                    {
                        yield return scene;
                    }
                }

                foreach (var scene in UIScenes)
                {
                    yield return scene;
                }

                if (transition != null) yield return transition.TransitionScene;
            }
        }

        public void SetRenderOrder(IEnumerable<SceneType> order)
        {
            renderOrder = new List<SceneType>(order);
        }
    }
}
