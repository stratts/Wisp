using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{
    public class PortalHandler : IEventHandler
    {
        public void Process(Event _e, Scene scene)
        {
            var e = (CollisionEvent)_e;

            var portal = e.target.GetComponent<ScenePortal>();

            if (portal != null && e.source.HasComponent<TriggerPortal>())
            {
                var targetScene = scene.sceneManager.GetScene(portal.target);
                scene.NodeManager.RemoveNode(e.source);
                targetScene.NodeManager.AddNode(e.source);

                if (e.source.HasComponent<IsPlayer>())
                {               
                    scene.sceneManager.ChangeScene(portal.type, portal.target, "Fade", portal.Unload);
                    targetScene.camera.Target = e.source;
                    scene.camera.Target = null;
                }
            }
        }
    }
}
