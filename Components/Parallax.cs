using System;

using Microsoft.Xna.Framework;

namespace Wisp.Components
{
    public class Parallax : Component
    {
        public float Amount { get; set; }

        private Vector2 lastCameraPos = Vector2.Zero;
        private bool enabled = false;

        public override void Update(Scene scene)
        {
            if (enabled) Parent.Pos += (lastCameraPos - scene.camera.Pos) * Amount;
            else enabled = false;
            lastCameraPos = scene.camera.Pos;

            base.Update(scene);
        }
    }
}
