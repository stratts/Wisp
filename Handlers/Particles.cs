using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Wisp.Nodes;
using Wisp.Components;

namespace Wisp.Handlers
{
    public class ParticleEmitterHandler : ILogic
    {
        List<Particle> particles = new List<Particle>();
        Random random = new Random();

        public void Run(Node node, Scene scene)
        {
            var emitter = (ParticleEmitter)node;
            var elapsed = scene.elapsedTime;

            emitter.ElapsedEmitTime += elapsed;

            if (emitter.ElapsedEmitTime > emitter.NextEmitTime)
            {
                emitter.ElapsedEmitTime = 0;
                emitter.NextEmitTime = GetRandomAttribValue(emitter.Interval);
                var particle = GetNewParticle(emitter);
                scene.AddNode(particle, node.SceneLayer);
                //Console.WriteLine($"Emitted particle {emitter.Particles.Count}, next emit time: {emitter.NextEmitTime}, lifetime: {particle.MaxLifetime}");
            }
        }

        float GetRandomAttribValue(ParticleAttrib attrib)
        {
            float offset = (2 * attrib.Spread * (float)random.NextDouble()) - attrib.Spread;
            return attrib.Base + offset;
        }

        Particle GetNewParticle(ParticleEmitter emitter)
        {
            var velocity = GetRandomAttribValue(emitter.Velocity);

            Particle particle = null;

            foreach (var p in particles)
            {
                if (!p.active)
                {
                    particle = p;
                    break;
                }
            }

            var pVel = new Vector2(velocity, velocity);
            var pScale = GetRandomAttribValue(emitter.Scale);
            var pRot = GetRandomAttribValue(emitter.Rotation);
            var pLife = GetRandomAttribValue(emitter.MaxLifetime);

            if (particle != null)
            {
                particle.Pos = emitter.ScenePos;
                particle.GetComponent<Moveable>().velocity = pVel;
                var anim = particle.GetComponent<ConstantAnim>();
                anim.ScaleSpeed = pScale;
                anim.RotationSpeed = pRot;
                var life = particle.GetComponent<Lifetime>();
                life.Time = 0;
                life.MaxTime = pLife;
                Console.WriteLine("Reused particle");
            }
            else {
                particle = new Particle(pVel, pScale, pRot, pLife)
                {
                    TexturePath = emitter.TexturePath,
                    Size = emitter.TextureSize,
                    Pos = emitter.ScenePos
                };

                Console.WriteLine("Created new particle");
                particles.Add(particle);
            }

            return particle;
        }
    }

    public class LifetimeHandler : IProcessHandler
    {
        public void Process(Node node, Component component, Scene scene)
        {
            var lifetime = (Lifetime)component;
            lifetime.Time += scene.elapsedTime;
            if (lifetime.Time > lifetime.MaxTime) scene.RemoveNode(node);
        }
    }

    public class ConstantAnimHandler : IProcessHandler
    {
        public void Process(Node node, Component component, Scene scene)
        {
            var anim = (ConstantAnim)component;
            anim.Scale += anim.ScaleSpeed * scene.elapsedTime;
            anim.Rotation += anim.RotationSpeed * scene.elapsedTime;
        }
    }
}
