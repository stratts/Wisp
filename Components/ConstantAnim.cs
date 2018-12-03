using System;


namespace Wisp.Components
{
    public class ConstantAnim : Component
    {
        public float Scale { get; set; } = 1;
        public float ScaleSpeed { get; set; }
        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public override void Update(Scene scene)
        {
            Scale += ScaleSpeed * scene.elapsedTime;
            Rotation += RotationSpeed * scene.elapsedTime;

            base.Update(scene);
        }
    }
}
