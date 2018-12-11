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
        public float Opacity { get; set; } = 1;
        public float Rotation { get; set; }
        public float Scale { get; set; } = 1f;

        public virtual Point RenderSize
        {
            get => new Point((int)(Size.X * Scale), (int)(Size.Y * Scale));
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
        private string contents;
        public string Contents
        {
            get { return contents; }
            set
            {
                contents = value;
                Index = MaxIndex;
                UpdateString = true;
            }
        }

        private bool wrap;
        public bool Wrap
        {
            get { return wrap; }
            set
            {
                wrap = value;
                UpdateString = true;
            }
        }

        private float index;
        public float Index
        {
            get { return index; }
            set
            {
                if (value != index) UpdateString = true;
                index = value;
            }
        }

        public int MaxIndex => Contents.Length - 1;
        public string FontPath { get; set; }
        public SpriteFont Font { get; set; } = null;
        public Color Color { get; set; } = Color.Black;
        public bool Shadow { get; set; } = false;
        public Color ShadowColor { get; set; } = Color.TransparentBlack;
        public int ShadowDist { get; set; } = 1;

        public bool UpdateString { get; private set; }

        private string renderString;
        public string RenderString
        {
            get { return renderString; }
            set
            {
                renderString = value;
                UpdateString = false;
            }
        }

        public Text(Vector2 pos, string font)
        {
            Pos = pos;
            FontPath = font;
        }

        public override Point RenderSize => Size;
    }

    public class Background : Drawable
    {
        public BackgroundType Type { get; set; } = BackgroundType.Stretch;
        public bool WrapX { get; set; } = false;
        public bool WrapY { get; set; } = false;

        public override Point RenderSize { get { return Size; } }
    }

    public enum BackgroundType { Tile, Stretch }

    public class TileSprite : Sprite
    {
        public int Id { get; set; }
        public Point MapPos { get; set; }
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

        public override Point RenderSize
        {
            get
            {
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
