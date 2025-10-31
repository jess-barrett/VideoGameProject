using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject2
{
    public class Coin
    {
        public SpriteAnimation Animation;
        public Vector2 Position;
        public bool IsCollected = false;
        private float scale = 4f;

        public Rectangle Hitbox
        {
            get
            {
                int hitboxWidth = 24;
                int hitboxHeight = 24;
                return new Rectangle(
                    (int)(Position.X - hitboxWidth / 2),
                    (int)(Position.Y - hitboxHeight / 2),
                    hitboxWidth,
                    hitboxHeight
                );
            }
        }

        public Coin(Texture2D texture, Vector2 position, int frames, int fps)
        {
            Position = position;
            Animation = new SpriteAnimation(texture, frames, fps);
            Animation.Scale = scale;
            Animation.Origin = new Vector2((texture.Width / (float)frames) / 2f, texture.Height / 2f);
            Animation.Position = Position;
        }

        public void Update(GameTime gameTime)
        {
            Animation.Position = Position;
            Animation.Update(gameTime);
        }
    }
}
