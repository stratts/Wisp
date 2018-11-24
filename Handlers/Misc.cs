using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{
    public class AIHandler : IProcessHandler
    {
        public void Process(Node node, Component component, Scene scene)
        {
            var ai = (AI)component;
            ai.Run(node, scene);
        }
    }

    public class ScriptHandler : IProcessHandler
    {
        public void Process(Node node, Component component, Scene scene)
        {
            var script = (Script)component;
            script.logic.Run(node, scene);
        }
    }

    public class PortalHandler : IEventHandler
    {
        public void Process(Event _e, Scene scene)
        {
            var e = (CollisionEvent)_e;

            var portal = e.source.GetComponent<ScenePortal>();

            if (portal != null && e.target.HasComponent<TriggerPortal>())
            {
                var targetScene = scene.sceneManager.GetScene(portal.target);
                scene.NodeManager.RemoveNode(e.target);
                targetScene.NodeManager.AddNode(e.target);

                if (e.target.HasComponent<IsPlayer>())
                {               
                    scene.sceneManager.ChangeScene(portal.type, portal.target, "Fade", portal.Unload);
                    targetScene.camera.Target = e.target;
                    scene.camera.Target = null;
                }
            }
        }
    }

    public class ParallaxHandler : IProcessHandler
    {
        private Vector2 lastCameraPos;
        private bool enabled = false;

        public void Process(Node node, Component component, Scene scene)
        {
            var parallax = (Parallax)component;
            if (enabled) node.Pos += (lastCameraPos - scene.camera.Pos) * parallax.Amount;
            else enabled = true;
            lastCameraPos = scene.camera.Pos;      
        }
    }

}
