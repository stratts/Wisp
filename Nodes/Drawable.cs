using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wisp.Components;
using Wisp.Handlers;

namespace Wisp.Nodes
{
    public abstract class Drawable : Node
    {
        public CollisionBox collisionBox = new CollisionBox();
        public bool Scissor { get; set; } = false;
        public string TexturePath { get; set; }
        public Texture2D Texture { get; set; } = null;
        public float Opacity { get; set; } = 1;
        public float Rotation { get; set; }
        public float Scale { get; set; } = 1f;

        public virtual Point RenderSize
        {
            get => new Point((int)(Size.X * Scale),(int)(Size.Y * Scale));
        }
    }

    public class Sprite : Drawable
    { 
        public bool FlipH { get; set; }
        public bool FlipV { get; set; }

        public bool region;
        public int SourceX { get; set; } = 0;
        public int SourceY { get; set; } = 0;
    }

    public class AnimatedSprite : Sprite
    {
        public int Frame { get; set; } = 0;
    }

    public class Text : Drawable
    {
        private bool wrap = false;
        private string contents = "";
        private bool textChanged = true;
        private string _string;
        private Point renderSize;

        public string FontPath { get; set; }
        public SpriteFont Font { get; set; } = null;
        public Color Color { get; set; } = Color.Black;
        public bool Shadow { get; set; } = false;
        public Color ShadowColor { get; set; } = Color.TransparentBlack;
        public int ShadowDist { get; set; } = 1;

        public Text(Vector2 pos, string font)
        {
            Pos = pos;
            FontPath = font;
        }

        public bool Wrap
        {
            get { return wrap; }
            set
            {
                textChanged = true;
                wrap = value;
            }
        }

        public string Contents
        {
            get { return contents; }
            set
            {
                textChanged = true;
                contents = value;
            }
        }

        public string String
        {
            get
            {
                if (textChanged)
                {
                    if (Wrap) _string = TextTools.WrapText(Contents, Font, Size.X);
                    else _string = Contents;
                    textChanged = false;

                    var size = Font.MeasureString(_string);
                    renderSize = new Point((int)size.X, (int)size.Y);
                }

                return _string;
            }
        }

        public override Point RenderSize { get { return renderSize; } }

    }

    public class Background : Drawable
    {
        public BackgroundType Type { get; set; } = BackgroundType.Stretch;
        public bool WrapX { get; set; } = false;
        public bool WrapY { get; set; } = false;

        public override Point RenderSize { get { return Size; } }
    }

    public enum BackgroundType { Tile, Stretch }

    public class TileSprite : Drawable
    {
        public int id;
        public Point source;
        public bool SourceSet { get; private set; } = false;

        public void SetSource()
        {
            var tilesetWidth = Texture.Width / Size.X;
            var tilesetHeight = Texture.Height / Size.Y;

            source.Y = (id - 1) / tilesetWidth;
            source.X = id - (tilesetWidth * source.Y) - 1;

            SourceSet = true;
        }

        public override Point RenderSize { get { return Size; } }
    }

    public class Rect : Drawable
    {
        public Color Color { get; set; }

        public Rect(Point size, Color color)
        {
            Size = size;
            Color = color;
        }

        public override Point RenderSize { get { return Size; } }
    }

    public class Particle : Drawable
    {
        public Particle(Vector2 velocity, float scaleSpeed, float rotationSpeed, float lifetime) 
        {
            var movement = AddComponent<Moveable>();
            movement.velocity = velocity;
            movement.frictionMultiplier = 0;
            movement.maxVelocity = 1000;

            var anim = AddComponent<ConstantAnim>();
            anim.ScaleSpeed = scaleSpeed;
            anim.RotationSpeed = rotationSpeed;

            var life = AddComponent<Lifetime>();
            life.MaxTime = lifetime;
        }

        public override Point RenderSize {
            get {
                var anim = GetComponent<ConstantAnim>();
                return new Point((int)(Size.X * anim.Scale), (int)(Size.Y * anim.Scale));
            }
        }
    }

    public struct ParticleAttrib
    {
        public float Base;
        public float Spread;

        public ParticleAttrib(float initial, float spread)
        {
            Base = initial;
            Spread = spread;
        }
    }

    public class ParticleEmitter : Node
    {
        public string TexturePath;
        public Point TextureSize;

        public float ElapsedEmitTime { get; set; }
        public float NextEmitTime { get; set; }

        public ParticleAttrib Interval { get; set; }
        public ParticleAttrib Direction { get; set; }
        public ParticleAttrib Velocity { get; set; }
        public ParticleAttrib MaxLifetime { get; set; }
        public ParticleAttrib Rotation { get; set; }
        public ParticleAttrib Scale { get; set; }

        public ParticleEmitter() 
        {
            var script = AddComponent<Script>();
            script.logic = new ParticleEmitterHandler();
        }
    }
}
