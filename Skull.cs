using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameProject2
{
    public class Skull
    {
        private SpriteAnimation animation;
        private Vector2 position;
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

                int currentFrame = animation.CurrentFrameIndex;
                int yOffset = frameYOffsets[currentFrame];

                return new Rectangle(
                    (int)(position.X - hitboxWidth / 2),
                    (int)(position.Y - hitboxHeight / 2 + yOffset),
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
            animation = new SpriteAnimation(texture, frames, fps);
            animation.Scale = scale;
            animation.Origin = new Vector2((texture.Width / (float)frames) / 2f, texture.Height / 2f);

            // spawn at screen edges
            Random rng = new Random();
            int side = rng.Next(4);
            switch (side)
            {
                case 0: // left edge
                    position = new Vector2(0, rng.Next(screenHeight));
                    break;
                case 1: // right edge
                    position = new Vector2(screenWidth, rng.Next(screenHeight));
                    break;
                case 2: // top edge
                    position = new Vector2(rng.Next(screenWidth), 0);
                    break;
                case 3: // bottom edge
                    position = new Vector2(rng.Next(screenWidth), screenHeight);
                    break;
            }
            animation.Position = position;
        }

        public void Update(GameTime gameTime, Player player)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 direction = player.Position - position;
            if (direction != Vector2.Zero)
                direction.Normalize();
            position += direction * speed * dt;
            animation.Position = position;
            animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);
        }
    }
}