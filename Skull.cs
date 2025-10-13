using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameProject2
{
    public class Skull
    {
        public SpriteAnimation Animation;
        public Vector2 Position;
        private float speed = 100f;
        private float scale = 1f;

        // Offset for each frame of the bobbing animation
        private int[] frameYOffsets = new int[10]
        {
            0, 
            5,
            10,
            10,
            5,
            0,
            -5,
            -10,
            -10,
            -5
        };

        public Rectangle Hitbox
        {
            get
            {
                int hitboxWidth = 64;
                int hitboxHeight = 64;

                int currentFrame = Animation.CurrentFrameIndex;
                int yOffset = frameYOffsets[currentFrame];

                return new Rectangle(
                    (int)(Position.X - hitboxWidth / 2),
                    (int)(Position.Y - hitboxHeight / 2 + yOffset),
                    hitboxWidth,
                    hitboxHeight
                );
            }
        }

        public RotatedRectangle RotatedHitbox
        {
            get
            {
                return new RotatedRectangle(Hitbox, 0);
            }
        }

        public Skull(Texture2D texture, int frames, int fps, int screenWidth, int screenHeight)
        {
            Animation = new SpriteAnimation(texture, frames, fps);
            Animation.Scale = scale;
            Animation.Origin = new Vector2((texture.Width / (float)frames) / 2f, texture.Height / 2f);
            Animation.LayerDepthOffset = -100f;

            // spawn at screen edges
            Random rng = new Random();
            int side = rng.Next(4);
            switch (side)
            {
                case 0: // left edge
                    Position = new Vector2(0, rng.Next(screenHeight));
                    break;
                case 1: // right edge
                    Position = new Vector2(screenWidth, rng.Next(screenHeight));
                    break;
                case 2: // top edge
                    Position = new Vector2(rng.Next(screenWidth), 0);
                    break;
                case 3: // bottom edge
                    Position = new Vector2(rng.Next(screenWidth), screenHeight);
                    break;
            }
            Animation.Position = Position;
        }

        public void Update(GameTime gameTime, Player player)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 direction = player.Position - Position;
            if (direction != Vector2.Zero)
                direction.Normalize();
            Position += direction * speed * dt;
            Animation.Position = Position;
            Animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Animation.Draw(spriteBatch);
        }
    }
}