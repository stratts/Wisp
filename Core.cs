using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Wisp
{
    public class Core
    {
        Game game;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SceneRender renderer;
        SceneManager sceneManager;

        public Color ClearColor { get; set; } = Color.CornflowerBlue;

        public Core(Game game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            this.graphics = graphics;

            renderer = new SceneRender(game);
            sceneManager = new SceneManager(graphics);
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            KeyManager.Update(Keyboard.GetState(), (float)gameTime.ElapsedGameTime.TotalSeconds);
            sceneManager.UpdateCurrentScenes(gameTime);
        }

        public void Render()
        {
            graphics.GraphicsDevice.Clear(ClearColor);

            foreach (var scene in sceneManager.Scenes)
            {
                scene.camera.Update();
                renderer.Render(scene.NodeManager.Nodes, scene.camera, spriteBatch);
            }
        }

        public void SetCurrentScene(SceneType type, Scene scene) => sceneManager.SetCurrentScene(type, scene);

        public void AddHandler<T>() where T : Component => sceneManager.AddHandler<T>();

        public void AddEventHandler<T>(IEventHandler<T> handler) where T : Event => 
            sceneManager.AddEventHandler(handler);

        public void SetRenderOrder(IEnumerable<SceneType> order) => sceneManager.SetRenderOrder(order);
    }
}
