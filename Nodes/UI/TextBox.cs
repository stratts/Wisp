using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wisp.Nodes.UI
{
    public class TextBox : Node
    {
        public String Content
        {
            get
            {
                return text.Contents;
            }
            set
            {
                text.Contents = value;
            }
        }

        private Text text;

        public TextBox(Vector2 pos, Point size)
        {
            Pos = pos;
            Size = size;

            int padding = 8;

            AddChild(new UIBox(Vector2.Zero, size)
            {
                Layer = 0
            });

            text = (Text)AddChild(new Text(new Vector2(padding), "Fonts/spritefont")
            {
                Size = new Point(size.X - (padding * 2), size.Y - (padding * 2)),
                Wrap = true,
                Color = Color.White,
                Shadow = true,
                ShadowColor = new Color(0, 0, 0, 0.5f),
                Layer = 1,
                Scissor = true
            });

            
        }
    }
}
