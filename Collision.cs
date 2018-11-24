using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Wisp.Nodes;

namespace Wisp
{
    public class Collision
    {

        public Direction direction;
        public float coord;

        public Collision(Direction direction, float coord)
        {
            this.direction = direction;
            this.coord = coord;
        }
    }

    public class CollisionBox
    {
        public Vector2 pos;

        public float left;
        public float right;
        public float up;
        public float down;

        public void UpdateBox(Vector2 pos, Vector2 size)
        {
            left = pos.X;
            up = pos.Y;
            right = pos.X + size.X;
            down = pos.Y + size.Y;

            this.pos = pos;
        }

        public void UpdateBox(Vector2 pos, Point size)
        {
            left = pos.X;
            up = pos.Y;
            right = pos.X + size.X;
            down = pos.Y + size.Y;

            this.pos = pos;
        }

        public bool IsColliding(CollisionBox box)
        {
            return (!(left > box.right || right < box.left ||
                up > box.down || down < box.up));
        }


        public Collision CheckCollision(CollisionBox box)
        {
            int direction = -1;
            float coord = 0f;

            if (IsColliding(box))
            {
                var leftDist = Math.Abs(left - box.right);
                var rightDist = Math.Abs(right - box.left);
                var upDist = Math.Abs(up - box.down);
                var downDist = Math.Abs(down - box.up);

                var minDist = new [] { leftDist, rightDist, upDist, downDist }.Min();

                if (minDist == leftDist)
                { 
                    direction = (int)Direction.Left;
                    coord = box.right;
                }
                else if (minDist == rightDist)
                {
                    direction = (int)Direction.Right;
                    coord = box.left;
                }
                else if (minDist == upDist)
                {
                    direction = (int)Direction.Up;
                    coord = box.down;
                }
                else if (minDist == downDist)
                {
                    direction = (int)Direction.Down;
                    coord = box.up;
                }

                return new Collision((Direction)direction, coord);
            }

            return null;
        }
      
    }
}
