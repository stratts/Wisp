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
            sceneManager = new SceneManager(game, graphics);
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
            sceneManager.RenderCurrentScenes(spriteBatch);
        }

        public void AddScene<T>() where T : Scene => sceneManager.AddScene<T>();

        public void SetCurrentScene(SceneType type, string scene) => sceneManager.SetCurrentScene(type, scene);

        public void AddHandler<T>() where T : Component => sceneManager.AddHandler<T>();

        public void AddEventHandler<T>(IEventHandler<T> handler) where T : Event => 
            sceneManager.AddEventHandler(handler);

        public void SetRenderOrder(IEnumerable<SceneType> order) => sceneManager.SetRenderOrder(order);
    }
}
