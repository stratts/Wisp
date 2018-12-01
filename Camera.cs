using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Wisp
{
    public class Camera
    {
        public Vector2 Pos;
        public Node Target { get; set; }
        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        public float zoom = 1;

        private int width;
        private int height;

        public int Width
        {
            get
            {
                return (int)(width / zoom);
            }
        }

        public int Height
        {
            get
            {
                return (int)(height / zoom);
            }
        }



        public CollisionBox CollisionBox { get; }

        public Camera(Vector2 pos, int width, int height)
        {
            Pos = pos;
            this.width = width;
            this.height = height;

            CollisionBox = new CollisionBox();
        }

        public Camera(Point viewport)
        {
            Pos = new Vector2(0, 0);
            width = viewport.X;
            height = viewport.Y;

            CollisionBox = new CollisionBox();
        }

        public void Update()
        {

            if (Target != null)
            {
                var targetPos = Target.CentrePos;

                Pos = targetPos.ToPoint().ToVector2();
                Pos.X -= (width / zoom) / 2;
                Pos.Y -= (height / zoom) / 2;

                if (Bounds != Rectangle.Empty)
                {
                    if (Pos.X < Bounds.Left) Pos.X = Bounds.Left;
                    if (Pos.Y < Bounds.Top) Pos.Y = Bounds.Top;
                    if (Pos.X + Width > Bounds.Right) Pos.X = Bounds.Right - Width;
                    if (Pos.Y + Height > Bounds.Bottom) Pos.Y = Bounds.Bottom - Height;
                }
            }

            CollisionBox.UpdateBox(Pos, new Vector2(width / zoom, height / zoom));
        }
    }
}
