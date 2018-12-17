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

            var friction = new Vector2(scene.friction * move.frictionMultiplier * elapsed);
            if (friction.X > Math.Abs(move.velocity.X)) friction.X = Math.Abs(move.velocity.X);
            if (friction.Y > Math.Abs(move.velocity.Y)) friction.Y = Math.Abs(move.velocity.Y);

            // Apply friction
            if (move.velocity.X > 0) move.velocity.X -= friction.X;
            else if (move.velocity.X < 0) move.velocity.X += friction.X;
            if (move.velocity.Y > 0) move.velocity.Y -= friction.Y;
            else if (move.velocity.Y < 0) move.velocity.Y += friction.Y;

            if (Math.Abs(move.velocity.X) < 2) move.velocity.X = 0;
            if (Math.Abs(move.velocity.Y) < 2) move.velocity.Y = 0;
        }
    }
}
