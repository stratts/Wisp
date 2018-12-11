using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wisp.Nodes;
using Wisp.Components;

namespace Wisp
{
    public class SceneRender
    {
        private Camera camera;
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphics;
        private SamplerState samplerState;
        private Game game;
        private Matrix matrix;
        private Matrix scale;

        public int NumDrawn { get; set; }

        private delegate void RenderHandler(Drawable node, Vector2 pos);
        private Dictionary<Type, RenderHandler> renderHandlers;
        private Dictionary<Type, bool> renderEnabled;

        private Dictionary<Node, Texture2D> textures = new Dictionary<Node, Texture2D>();

        public SceneRender(Game game)
        {
            this.game = game;

            renderHandlers = new Dictionary<Type, RenderHandler>();
            renderEnabled = new Dictionary<Type, bool>();

            AddHandler<Text>(Text);
            //AddHandler<AnimatedText>(Text);
            AddHandler<Sprite>(Sprite);
            AddHandler<AnimatedSprite>(AnimatedSprite);
            AddHandler<Background>(Background);
            AddHandler<TileSprite>(TileSprite);
            AddHandler<Rect>(Rect);
            AddHandler<Particle>(Particle);
        }

        private void AddHandler<T>(RenderHandler handler) where T : Drawable
        {
            renderHandlers.Add(typeof(T), handler);
            EnableRendering<T>();
        }

        public void EnableRendering<T>() where T : Drawable
        {
            renderEnabled[typeof(T)] = true;
        }

        public void DisableRendering<T>() where T : Drawable
        {
            renderEnabled[typeof(T)] = false;
        }

        public bool RenderingEnabled<T>() where T : Drawable
        {
            return renderEnabled[typeof(T)];
        }

        public void Render(List<Node> nodes, Camera camera, SpriteBatch spriteBatch)
        {
            this.camera = camera;
            this.spriteBatch = spriteBatch;
            graphics = spriteBatch.GraphicsDevice;
            samplerState = SamplerState.PointClamp;

            var cameraPos = camera.Pos.ToPoint();
            var translation = Matrix.CreateTranslation(-cameraPos.X, -cameraPos.Y, 0);
            scale = Matrix.CreateScale(camera.zoom, camera.zoom, 0f);
            matrix = translation * scale;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, samplerState, null, null, null, matrix);

            RenderNodes(nodes);

            spriteBatch.End();
        }

        public void RenderNodes(List<Node> nodes)
        {
            nodes.Sort(NodeSorter.Compare);
            foreach (var node in nodes) RenderNode(node);
        }

        public void RenderNode(Node node)
        {
            if (!node.active) return;
            if (node is Drawable drawNode) RenderDrawable(drawNode);
            foreach (var child in node.Children) RenderNode(child);
        }

        public void RenderDrawable(Drawable node)
        {
            var scenePos = node.ScenePos;

            node.collisionBox.UpdateBox(scenePos, node.RenderSize);

            if (node.collisionBox.IsColliding(camera.CollisionBox))
            {
                var pos = scenePos.ToPoint().ToVector2();
                var type = node.GetType();
                renderHandlers.TryGetValue(node.GetType(), out RenderHandler handler);

                if (handler != null && renderEnabled[type])
                {
                    if (node.Scissor) StartScissor(pos, node.Size);
                    handler(node, pos);
                    if (node.Scissor) EndScissor();
                }

                NumDrawn++;
            }
        }

        public void StartScissor(Vector2 pos, Point size)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, samplerState, null,
            new RasterizerState() { ScissorTestEnable = true }, null, matrix);

            var rectPos = Vector2.Transform(pos, matrix).ToPoint();
            var rectSize = Vector2.Transform(size.ToVector2(), scale).ToPoint();

            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(rectPos, rectSize);
        }

        public void EndScissor()
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, samplerState,
                null, null, null, matrix);
        }

        public void Sprite(Drawable node, Vector2 pos)
        {
            var sprite = (Sprite)node;
            Texture2D texture = GetTexture(sprite);
            int width, height;

            float scale = sprite.Scale;
            float rotation = MathHelper.ToRadians(sprite.Rotation);
            bool flipH = sprite.FlipH;
            bool flipV = sprite.FlipV;

            if (sprite.region)
            {
                width = sprite.Size.X;
                height = sprite.Size.Y;
            }
            else
            {
                width = texture.Width;
                height = texture.Height;
            }

            var sourceRect = new Rectangle(
                sprite.SourceX * width, sprite.SourceY * height, width, height);
            var origin = new Vector2(width / 2, height / 2);

            SpriteEffects effects;

            if (flipH && !flipV)
                effects = SpriteEffects.FlipHorizontally;
            else if (!flipH && flipV)
                effects = SpriteEffects.FlipVertically;
            else if (flipH && flipV)
                effects = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
            else effects = SpriteEffects.None;

            spriteBatch.Draw(
                texture,
                pos + (origin * scale),
                sourceRect, Color.White, rotation,
            origin, scale, effects, 0);
        }

        public void AnimatedSprite(Drawable node, Vector2 pos)
        {
            var sprite = (AnimatedSprite)node;
            Texture2D texture = GetTexture(sprite);

            sprite.region = true;

            var framesX = texture.Width / sprite.Size.X;

            sprite.SourceX = sprite.Frame % framesX;
            sprite.SourceY = sprite.Frame / framesX;

            Sprite(node, pos);
        }

        public void TileSprite(Drawable node, Vector2 pos)
        {
            var sprite = (TileSprite)node;
            var texture = GetTexture(sprite);

            sprite.region = true;

            var tilesX = texture.Width / sprite.Size.X;

            sprite.SourceX = (sprite.Id - 1) % tilesX;
            sprite.SourceY = (sprite.Id - 1) / tilesX;

            Sprite(node, pos);
        }

        public void Text(Drawable node, Vector2 pos)
        {
            var text = (Text)node;
            var font = GetFont(text);

            if (text.UpdateString)
            {
                if (text.Wrap) text.RenderString = TextTools.WrapText(text.Contents, font, text.Size.X);
                else text.RenderString = text.Contents;
                if (text.Index < text.MaxIndex)
                {
                    float indexPos = (float)text.Index / (float)text.MaxIndex;
                    int newLen = (int)(text.RenderString.Length * indexPos);
                    text.RenderString = text.RenderString.Substring(0, newLen);
                }
            }

            if (text.Shadow)
            {
                var shadowPos = new Vector2(pos.X, pos.Y + text.ShadowDist);

                spriteBatch.DrawString(
                    font, text.RenderString, shadowPos, text.ShadowColor,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }

            spriteBatch.DrawString(
                font, text.RenderString, pos, text.Color,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }

        public void Background(Drawable node, Vector2 pos)
        {
            var background = (Background)node;
            var texture = GetTexture(background);
            var size = background.Size;

            if (background.Type == BackgroundType.Stretch)
            {
                var destRect = new Rectangle((int)pos.X, (int)pos.Y, size.X, size.Y);
                spriteBatch.Draw(texture, destRect, Color.White);
            }
            else if (background.Type == BackgroundType.Tile)
            {
                for (int x = 0; x < size.X; x += texture.Width)
                {
                    for (int y = 0; y < size.Y; y += texture.Height)
                    {
                        spriteBatch.Draw(
                            texture, new Vector2(pos.X + x, pos.Y + y), Color.White);
                    }
                }
            }
        }

        public void Rect(Drawable node, Vector2 pos)
        {
            var rect = (Rect)node;

            textures.TryGetValue(node, out var texture);
            if (texture == null)
            {
                var size = rect.Size;
                var fill = new Color[size.X * size.Y];

                for (int i = 0; i < fill.Length; i++)
                {
                    fill[i] = rect.Color;
                }

                texture = new Texture2D(game.GraphicsDevice, size.X, size.Y);
                texture.SetData(fill);
                textures[node] = texture;
            }

            spriteBatch.Draw(texture, pos, Color.White * rect.Opacity);
        }

        public void Particle(Drawable node, Vector2 pos)
        {
            var particle = (Particle)node;
            var texture = GetTexture(node);
            var sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            var anim = particle.GetComponent<ConstantAnim>();
            var offset = new Vector2(texture.Width / 2, texture.Height / 2) * anim.Scale;

            spriteBatch.Draw(
                texture,
                pos + offset,
                sourceRect,
                Color.White * particle.Opacity,
                anim.Rotation,
                offset,
                anim.Scale,
                SpriteEffects.None,
                0
            );
        }

        public Texture2D GetTexture(Drawable node)
        {
            textures.TryGetValue(node, out var texture);

            if (texture == null)
            {
                texture = game.Content.Load<Texture2D>(node.TexturePath);
                textures[node] = texture;
            }

            return texture;
        }

        public SpriteFont GetFont(Text node)
        {
            if (node.Font == null) node.Font = game.Content.Load<SpriteFont>(node.FontPath);
            node.Font.DefaultCharacter = '?';
            return node.Font;
        }
    }
}
