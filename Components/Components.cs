using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wisp.Components
{
    public class Component
    {
        public Node Parent { get; set; }
        public bool Enabled { get; set; }
    }

    public class Animated : Component
    {
        private AnimationGroup group;
        public AnimationGroup Group { set => group = value; }

        public Animated() { }

        public Animated(AnimationGroup group)
        {
            this.group = group;
        }

        public string CurrentAnimation
        {
            get => group.CurrentAnimation.Name;
            set => group.SetAnimation(value);
        }

        public IEnumerable<AnimationTrack> Tracks => group.CurrentAnimation.Tracks;
        public void Update(float elapsed) => group.CurrentAnimation.Update(elapsed);
        public void Reset() => group.CurrentAnimation.Reset();
        public bool Finished => group.CurrentAnimation.Finished;
    }

    public class Script : Component
    {
        public ILogic logic;
    }

    public class Solid : Component { }

    public class Collidable : Component
    {
        public CollisionBox collisionBox;
        public int Mask { get; set; } = -1;
        public Point Size { get; set; }
        public Vector2 Pos { get; set; }

        public Collidable()
        {
            collisionBox = new CollisionBox();
        }
    }

    public class Moveable : Component
    {
        public float accel;
        public Vector2 velocity;
        public float maxVelocity;
        public float frictionMultiplier;
        public float bounciness;
        public Vector2 bonusVelocity;

        public Moveable()
        {
            velocity = new Vector2(0, 0);
            bonusVelocity = new Vector2(0, 0);
        }
    }

    public class Lifetime : Component 
    {
        public float Time { get; set; }
        public float MaxTime { get; set; }
    }

    public class ConstantAnim : Component 
    {
        public float Scale { get; set; } = 1;
        public float ScaleSpeed { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }
    }

    public class ScenePortal : Component
    {
        public string target;
        public SceneType type;
        public bool Unload { get; set; } = false;
    }

    public class TriggerPortal : Component { }

    public class Parallax : Component
    {
        public float Amount { get; set; }
    }

    public class IsPlayer : Component { }
}

namespace Wisp
{
    public enum Direction { Left, Right, Up, Down }
}
