using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameProject2
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float RotationSpeed;
        public Color Color;
        public float Alpha;
        public float AlphaDecay;
        public float Scale;
        public float ScaleDecay;
        public float Lifetime;
        public bool IsAlive => Lifetime > 0;

        public Particle(Vector2 position, Vector2 velocity, Color color, float lifetime, float scale)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Alpha = 1f;
            AlphaDecay = 1f / lifetime;
            Scale = scale;
            ScaleDecay = scale / lifetime;
            Lifetime = lifetime;
            Rotation = (float)(new Random().NextDouble() * MathHelper.TwoPi);
            RotationSpeed = (float)(new Random().NextDouble() * 4 - 2); // Random rotation speed
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * dt;
            Rotation += RotationSpeed * dt;
            Alpha -= AlphaDecay * dt;
            Scale -= ScaleDecay * dt;
            Lifetime -= dt;

            if (Alpha < 0) Alpha = 0;
            if (Scale < 0) Scale = 0;
        }
    }
}