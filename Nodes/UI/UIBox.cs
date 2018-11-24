using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Wisp.Nodes.UI
{
    public class UIBox : Node
    {
        public UIBox(Vector2 pos, Point size)
        {
            Pos = pos;
            Size = size;

            var cornerSize = new Point(4, 4);
            var borderWidth = 4;

            AddChild(new Background
            {
                Size = size,
                Type = BackgroundType.Stretch,
                TexturePath = "UI/Box/middle",
                Layer = 0
            });

            var corner = new Sprite
            {
                Layer = 1,
                Size = cornerSize
            };


            AddChild(new Sprite
            {
                Pos = new Vector2(0, 0),
                Size = cornerSize,
                TexturePath = "UI/Box/topleft",
                Layer = 1
            });

            AddChild(new Sprite
            {
                Pos = new Vector2(size.X - cornerSize.X, 0),
                Size = cornerSize,
                TexturePath = "UI/Box/topright",
                Layer = 1
            });

            AddChild(new Sprite
            {
                Pos = new Vector2(0, size.Y - cornerSize.Y),
                Size = cornerSize,
                TexturePath = "UI/Box/bottomleft",
                Layer = 1
            });

            AddChild(new Sprite
            {
                Pos = new Vector2(size.X - cornerSize.X, size.Y - cornerSize.Y),
                Size = cornerSize,
                TexturePath = "UI/Box/bottomright",
                Layer = 1
            });

            AddChild(new Background
            {
                Pos = new Vector2(cornerSize.X, 0),
                Size = new Point(size.X - (cornerSize.X * 2), borderWidth),
                Type = BackgroundType.Stretch,
                TexturePath = "UI/Box/horizontal",
                Layer = 1
            });

            AddChild(new Background
            {
                Pos = new Vector2(cornerSize.X, size.Y - borderWidth),
                Size = new Point(size.X - (cornerSize.X * 2), borderWidth),
                Type = BackgroundType.Stretch,
                TexturePath = "UI/Box/horizontal",
                Layer = 1
            });

            AddChild(new Background
            {
                Pos = new Vector2(0, cornerSize.Y),
                Size = new Point(borderWidth, size.Y - (cornerSize.Y * 2)),
                Type = BackgroundType.Stretch,
                TexturePath = "UI/Box/vertical",
                Layer = 1
            });

            AddChild(new Background
            {
                Pos = new Vector2(size.X - borderWidth, cornerSize.Y),
                Size = new Point(borderWidth, size.Y - (cornerSize.Y * 2)),
                Type = BackgroundType.Stretch,
                TexturePath = "UI/Box/vertical",
                Layer = 1
            });
        }
    }
}
