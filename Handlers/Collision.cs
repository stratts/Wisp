using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{
    public class CollisionHandler : IProcessHandler
    {
        public void Process(Node input, Component component, Scene scene)
        {
            var current = (Collidable)component;
            var nodeManager = scene.NodeManager;

            var entity = input;
            Point size;
            if (current.Size == default(Point)) size = entity.Size;
            else size = current.Size;

            current.collisionBox.UpdateBox(entity.ScenePos + current.Pos, new Vector2(size.X, size.Y));
            var collidableComponents = nodeManager.GetComponents<Collidable>();

            foreach (Collidable other in collidableComponents)
            {
                if (current != other)
                {
                    if (current.Mask >= 0 && other.Mask >= 0 && current.Mask == other.Mask)
                        continue;

                    var box = current.collisionBox;
                    var collision = box.CheckCollision(other.collisionBox);

                    if (collision != null)
                    {
                        var collisionEvent = new CollisionEvent()
                        {
                            source = entity,
                            target = other.Parent,
                            collision = collision
                        };

                        scene.AddEvent(collisionEvent);
                    }
                }
            }
        }
    }

    class ApplyCollision : IEventHandler
    {
        public void Process(Event e, Scene scene)
        {
            var collisionEvent = (CollisionEvent)e;
            
            var entity = collisionEvent.source;
            var target = collisionEvent.target;

            if (entity.HasComponent<Moveable>() && target.HasComponent<Solid>())
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
                    //Console.WriteLine(move.velocity.Y);
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
