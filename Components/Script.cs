using System;

namespace Wisp.Components
{
    public class Script : Component
    {
        public ILogic logic;

        public override void Update(Scene scene)
        {
            logic.Run(Parent, scene);
            base.Update(scene);
        }
    }

    public class Input : Script { }
}
