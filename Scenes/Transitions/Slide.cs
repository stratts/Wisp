using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Scenes
{
    public class Slide : Transition
    {
        Node rect;
        Animated anim;

        public override void Load()
        {
            rect = new Node();
            rect.Pos = new Vector2(0, 0);
            rect.AddChild(new Rect(new Point(viewport.X, viewport.Y), Color.Black));
            var anim = new AnimationGroup();
            var fadeIn = anim.AddAnimation("slideIn");
            var posY = fadeIn.AddTrack(AnimationProperty.PosY);
            posY.Loop = false;
            posY.Length = 0.5f;
            posY.Ease = EaseType.CubicEaseIn;
            posY.AddFrame(0, 0);
            posY.AddFrame(0.5f, -viewport.Y);

            var fadeOut = anim.AddAnimation("slideOut");
            posY = fadeOut.AddTrack(AnimationProperty.PosY);
            posY.Loop = false;
            posY.Length = 0.5f;
            posY.Ease = EaseType.CubicEaseOut;
            posY.AddFrame(0, viewport.Y);
            posY.AddFrame(0.5f, 0);

            this.anim = (Animated)rect.AddComponent(new Animated(anim));
            this.anim.CurrentAnimation = "slideOut";

            AddNode(rect);
            base.Load();
        }

        public override void TransitionOut()
        {
            anim.CurrentAnimation = "slideOut";
            if (anim.Finished) FinishedOut = true;
            base.TransitionOut();
        }

        public override void TransitionIn()
        {
            anim.CurrentAnimation = "slideIn";
            if (anim.Finished) FinishedIn = true;
            base.TransitionIn();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
