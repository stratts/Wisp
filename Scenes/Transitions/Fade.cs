using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Scenes
{
    class Fade : Transition
    {
        Node rect;
        Animated anim;

        public override void Load(Game game)
        {
            rect = new Node();
            rect.AddChild(new Rect(new Point(viewport.X, viewport.Y), Color.Black));
            rect.GetFirstChildByType<Rect>().Opacity = 0;
            rect.GetFirstChildByType<Rect>().Name = "fadeRect";
            var anim = new AnimationGroup();
            var fadeIn = anim.AddAnimation("fadeIn");
            var opacity = fadeIn.AddTrack(AnimationProperty.Opacity);
            opacity.Loop = false;
            opacity.Length = 0.4f;
            opacity.Ease = EaseType.Linear;
            opacity.AddFrame(0, 1);
            opacity.AddFrame(0.4f, 0);

            var fadeOut = anim.AddAnimation("fadeOut");
            opacity = fadeOut.AddTrack(AnimationProperty.Opacity);
            opacity.Loop = false;
            opacity.Length = 0.4f;
            opacity.Ease = EaseType.Linear;
            opacity.AddFrame(0, 0);
            opacity.AddFrame(0.4f, 1);

            this.anim = (Animated)rect.AddComponent(new Animated(anim));
            this.anim.CurrentAnimation = "fadeOut";

            AddNode(rect);
            base.Load(game);
        }

        public override void TransitionOut()
        {
            anim.CurrentAnimation = "fadeOut";
            if (anim.Finished) FinishedOut = true;
            base.TransitionOut();
        }

        public override void TransitionIn()
        {
            anim.CurrentAnimation = "fadeIn";
            if (anim.Finished) FinishedIn = true;
            base.TransitionIn();
        }    

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
