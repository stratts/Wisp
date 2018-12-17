using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisp.Scenes
{
    public abstract class Transition : Scene
    {
        public bool FinishedIn { get; protected set; }
        public bool FinishedOut { get; protected set; }
        public bool Finished => FinishedIn && FinishedOut;

        public virtual void TransitionIn() { }
        public virtual void TransitionOut() { }
    }
}
