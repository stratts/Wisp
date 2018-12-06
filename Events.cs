using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Wisp
{
    public class Event
    {
        public Node source;
        public Node target;
    }

    public class CollisionEvent : Event
    {     
        public Collision collision;
        public bool IsNew { get; set; }
    }
}
