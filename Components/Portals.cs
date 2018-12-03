using System;


namespace Wisp.Components
{
    public class ScenePortal : Component
    {
        public string target;
        public SceneType type;
        public bool Unload { get; set; } = false;
    }

    public class TriggerPortal : Component { }
}
