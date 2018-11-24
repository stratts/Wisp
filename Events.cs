using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Wisp
{
    public class Event
    {
        public Node source;
    }

    public class CollisionEvent : Event
    {   
        public Node target;
        public Collision collision;
    }
}
