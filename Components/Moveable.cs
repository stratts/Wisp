using System;

using Microsoft.Xna.Framework;

namespace Wisp.Components
{
    public class Moveable : Component
    {
        public float accel;
        public Vector2 velocity;
        public float maxVelocity;
        public float frictionMultiplier = 1;
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

            var friction = scene.friction * move.frictionMultiplier * elapsed;
            move.velocity = ApplyFriction(move.velocity, friction);
            move.bonusVelocity = ApplyFriction(move.bonusVelocity, friction);

            if (Math.Abs(move.velocity.X) < 2) move.velocity.X = 0;
            if (Math.Abs(move.velocity.Y) < 2) move.velocity.Y = 0;
        }

        public Vector2 ApplyFriction(Vector2 velocity, float friction)
        {
            Vector2 frictionVector;
            frictionVector.X = -Math.Sign(velocity.X) * MathHelper.Clamp(friction, 0, Math.Abs(velocity.X));
            frictionVector.Y = -Math.Sign(velocity.Y) * MathHelper.Clamp(friction, 0, Math.Abs(velocity.Y));
            return velocity + frictionVector;
        }
    }
}
