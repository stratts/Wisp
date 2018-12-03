using System;

using Microsoft.Xna.Framework;

namespace Wisp.Components
{
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

        public override void Update(Scene scene)
        {
            var move = this;
            var elapsed = scene.elapsedTime;

            // Limit velocity to maximum velocity of entity
            move.velocity.X = MathHelper.Clamp(move.velocity.X,
                -move.maxVelocity, move.maxVelocity);
            move.velocity.Y = MathHelper.Clamp(move.velocity.Y,
                -move.maxVelocity, move.maxVelocity);

            var velXY = Math.Abs(move.velocity.X) + Math.Abs(move.velocity.Y);
            var limXY = move.maxVelocity * 1.5f;

            if (velXY > limXY)
            {
                move.velocity = move.velocity / (velXY / limXY);
            }

            Parent.Pos += (move.velocity + move.bonusVelocity) * elapsed;

            float friction = (1f +
                ((scene.friction * move.frictionMultiplier) * elapsed));

            // Apply friction
            move.velocity = move.velocity / friction;
            move.bonusVelocity = move.bonusVelocity / friction;

            if (Math.Abs(move.velocity.X) < 2) move.velocity.X = 0;
            if (Math.Abs(move.velocity.Y) < 2) move.velocity.Y = 0;
        }
    }
}
