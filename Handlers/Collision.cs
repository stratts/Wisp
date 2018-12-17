using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{
    class ApplyCollision : IEventHandler<CollisionEvent>
    {
        public void Process(CollisionEvent e, Scene scene)
        {
            var collisionEvent = e;
            
            var entity = collisionEvent.source;
            var target = collisionEvent.target;

            if (entity.HasComponent<Moveable>() && entity.HasComponent<Solid>() && target.HasComponent<Solid>())
            {
                var move = entity.GetComponent<Moveable>();
                var size = entity.Size;
                var collision = collisionEvent.collision;
                var c = entity.GetComponent<Collidable>();

                var targetMove = target.GetComponent<Moveable>();

                if (collision.direction == Direction.Left)
                {
                    if (targetMove != null)
                    {
                        targetMove.velocity.X = move.velocity.X;
                    }

                    move.velocity.X = 0;
                    move.bonusVelocity.X = 0;

                    entity.Pos = new Vector2(collision.coord - c.Pos.X, entity.Pos.Y);
                }
                if (collision.direction == Direction.Right)
                {
                    if (targetMove != null)
                    {
                        targetMove.velocity.X = move.velocity.X;
                    }

                    move.velocity.X = 0;
                    move.bonusVelocity.X = 0;
                    entity.Pos = new Vector2(collision.coord - (c.Size.X + c.Pos.X), entity.Pos.Y);
                }
                if (collision.direction == Direction.Up)
                {
                    if (targetMove != null)
                    {
                        targetMove.velocity.Y = move.velocity.Y;
                    }

                    move.velocity.Y = 0;
                    move.bonusVelocity.Y = 0;
                    entity.Pos = new Vector2(entity.Pos.X, collision.coord - c.Pos.Y);
                }
                if (collision.direction == Direction.Down)
                {
                    if (targetMove != null)
                    {
                        targetMove.velocity.Y = move.velocity.Y;
                    }

                    move.velocity.Y = 0;
                    move.bonusVelocity.Y = 0;
                    entity.Pos = new Vector2(entity.Pos.X, collision.coord - (c.Size.Y + c.Pos.Y));
                }
            }
        }
    }
}
