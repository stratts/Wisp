using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Wisp.Nodes;

namespace Wisp.Components
{
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

        public override void Update(Scene scene)
        {
            var node = Parent;
            if (Finished) return;

            Update(scene.elapsedTime);

            var drawable = node.GetFirstChildByType<Drawable>();

            foreach (var track in Tracks)
            {
                var value = track.CurrentValue;

                if (track is ComponentAnimationTrack t)
                {
                    if ((int)value == 0) node.DisableComponent(t.Component);
                    else node.EnableComponent(t.Component);
                    continue;
                }

                switch (track.Property)
                {
                    case AnimationProperty.Frame:
                        if (drawable is AnimatedSprite sprite) sprite.Frame = (int)value;
                        break;
                    case AnimationProperty.Scale:
                        drawable.Scale = value;
                        break;
                    case AnimationProperty.Opacity:
                        drawable.Opacity = value;
                        break;
                    case AnimationProperty.Rotation:
                        drawable.Rotation = value;
                        break;
                    case AnimationProperty.PosX:
                        node.Pos = new Vector2(value, node.Pos.Y);
                        break;
                    case AnimationProperty.PosY:
                        node.Pos = new Vector2(node.Pos.X, value);
                        break;
                    case AnimationProperty.StringIndex:
                        if (node is AnimatedText text) text.Index = (int)value;
                        break;
                }
            }

            base.Update(scene);
        }
    }
}
