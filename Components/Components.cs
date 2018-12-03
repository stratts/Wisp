using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wisp.Components
{
    public class Solid : Component { }

    public class IsPlayer : Component { }
}

namespace Wisp
{
    public abstract class Component
    {
        public Node Parent { get; set; }
        public bool Enabled { get; set; } = true;
        public virtual void Update(Scene scene) { }
    }

    public enum Direction { Left, Right, Up, Down }
}
