using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wisp.Components;

namespace Wisp.Nodes
{
    public class Portal : Node
    {
        public Portal(Vector2 pos, Point size, string target)
        {
            this.Pos = pos;

            Size = size;

            AddComponent<Collidable>();

            AddComponent(new ScenePortal()
            {
                target = target,
                type = SceneType.World
            });
        }
    }

    public class TileMap : Node
    {
        public TileMap(Vector2 pos, string mapPath, string tileset)
        {
            Pos = pos;

            var map = new TiledSharp.TmxMap(mapPath);
            var texture = tileset;

            foreach (var layer in map.Layers)
            {
                foreach (var tile in layer.Tiles)
                {
                    if (tile.Gid == 0) continue;

                    var t = new TileSprite
                    {
                        Pos = new Vector2(tile.X * map.TileWidth,
                        tile.Y * map.TileHeight),
                        MapPos = new Point(tile.X, tile.Y),
                        Size = new Point(map.TileWidth, map.TileHeight),
                        TexturePath = texture,
                        Id = tile.Gid
                    };

                    AddChild(t);
                }
            }

            Size = new Point(map.Width * map.TileWidth, map.Height * map.TileHeight);
        }
    }
}
