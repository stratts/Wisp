using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{

    public class AnimationHandler : IProcessHandler
    {
        public void Process(Node node, Component component, Scene scene)
        {
            var anim = (Animated)component;
            if (anim.Finished) return;

            anim.Update(scene.elapsedTime);

            var drawable = node.GetFirstChildByType<Drawable>();

            foreach (var track in anim.Tracks)
            {
                var value = track.CurrentValue;
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
                    case AnimationProperty.Collision:
                        if ((int)value == 0) node.DisableComponent<Collidable>();
                        else node.EnableComponent<Collidable>();
                        break;
                }
            }
        }
    }
}
