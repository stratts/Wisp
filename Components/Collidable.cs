using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Wisp.Components
{
    public class Collidable : Component
    {
        public CollisionBox collisionBox;
        public int Mask { get; set; } = -1;
        public Point Size { get; set; } = Point.Zero;
        public Vector2 Pos { get; set; }

        private HashSet<Node> previousCollisions = new HashSet<Node>();
        private HashSet<Node> currentCollisions = new HashSet<Node>();

        public Collidable()
        {
            collisionBox = new CollisionBox();
        }

        public override void Update(Scene scene)
        {
            var current = this;
            var nodeManager = scene.NodeManager;

            var entity = Parent;
            Point size;
            if (current.Size == Point.Zero) current.Size = entity.Size;
            size = current.Size;

            current.collisionBox.UpdateBox(entity.ScenePos + current.Pos, new Vector2(size.X, size.Y));

            if (IsStatic(entity)) return;

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
                            collision = collision,
                            IsNew = !previousCollisions.Contains(other.Parent)
                        };

                        currentCollisions.Add(other.Parent);
                        scene.AddEvent(collisionEvent);
                    }
                }
            }

            previousCollisions.Clear();
            foreach (var collision in currentCollisions) previousCollisions.Add(collision);
            currentCollisions.Clear();

            base.Update(scene);
        }

        private bool IsStatic(Node node)
        {
            if (node.parent != null) return IsStatic(node.parent);
            return !node.HasComponent<Moveable>();
        }
    }
}
