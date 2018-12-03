using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisp.Components
{
    public class Lifetime : Component
    {
        public float Time { get; set; }
        public float MaxTime { get; set; }

        public override void Update(Scene scene)
        {
            Time += scene.elapsedTime;
            if (Time > MaxTime) scene.RemoveNode(Parent);

            base.Update(scene);
        }
    }
}
