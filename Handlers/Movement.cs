using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{
    class MovementHandler : IProcessHandler
    {
        public void Process(Node node, Component component, Scene scene)
        {
            var move = (Moveable)component;
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

            node.Pos += (move.velocity + move.bonusVelocity) * elapsed;

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
