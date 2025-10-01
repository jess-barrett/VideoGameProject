using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameProject2
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Attack1,
        Attack2,
        Heal,
        Hurt,
        Dash,
        Death
    }

    public class Player
    {
        private Vector2 position = new Vector2(500, 300);

        private int walkSpeed = 300;
        private int runSpeed = 500;

        private Direction direction = Direction.Down;
        private bool isMoving = false;

        public PlayerState State { get; private set; } = PlayerState.Idle;

        public SpriteAnimation Animation;

        public SpriteAnimation[] idleAnimations = new SpriteAnimation[4];
        public SpriteAnimation[] walkAnimations = new SpriteAnimation[4];
        public SpriteAnimation[] runAnimations = new SpriteAnimation[4];

        public Vector2 Position => position;

        public RotatedRectangle RotatedHitbox
        {
            get
            {
                if (Animation == null) return new RotatedRectangle(Rectangle.Empty, 0);

                int hitboxWidth = 48;
                int hitboxHeight = 112;
                float rotation = 0f;

                if (State == PlayerState.Run)
                {
                    switch (direction)
                    {
                        case Direction.Left:
                            hitboxWidth = 48;
                            hitboxHeight = 112;
                            rotation = MathHelper.ToRadians(-15);
                            break;

                        case Direction.Right:
                            hitboxWidth = 48;
                            hitboxHeight = 112;
                            rotation = MathHelper.ToRadians(15);
                            break;

                        case Direction.Up:
                        case Direction.Down:
                            hitboxWidth = 48;
                            hitboxHeight = 96;
                            break;
                    }
                }

                Rectangle rect = new Rectangle(
                    (int)(position.X - hitboxWidth / 2),
                    (int)(position.Y - hitboxHeight / 2),
                    hitboxWidth,
                    hitboxHeight
                );

                return new RotatedRectangle(rect, rotation);
            }
        }
        public Rectangle Hitbox => RotatedHitbox.Rectangle;

        public void SetX(float newX) => position.X = newX;
        public void SetY(float newY) => position.Y = newY;

        public void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            isMoving = false;

            if (kbState.IsKeyDown(Keys.D))
            {
                direction = Direction.Right;
                isMoving = true;
                State = kbState.IsKeyDown(Keys.LeftShift) ? PlayerState.Run : PlayerState.Walk;
            }
            if (kbState.IsKeyDown(Keys.A))
            {
                direction = Direction.Left;
                isMoving = true;
                State = kbState.IsKeyDown(Keys.LeftShift) ? PlayerState.Run : PlayerState.Walk;
            }
            if (kbState.IsKeyDown(Keys.W))
            {
                direction = Direction.Up;
                isMoving = true;
                State = kbState.IsKeyDown(Keys.LeftShift) ? PlayerState.Run : PlayerState.Walk;
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                direction = Direction.Down;
                isMoving = true;
                State = kbState.IsKeyDown(Keys.LeftShift) ? PlayerState.Run : PlayerState.Walk;
            }

            if (isMoving)
            {
                int speed = State == PlayerState.Walk ? walkSpeed : runSpeed;
                switch (direction)
                {
                    case Direction.Down: position.Y += speed * dt; break;
                    case Direction.Up: position.Y -= speed * dt; break;
                    case Direction.Left: position.X -= speed * dt; break;
                    case Direction.Right: position.X += speed * dt; break;
                }

                Animation = State == PlayerState.Walk
                    ? walkAnimations[(int)direction]
                    : runAnimations[(int)direction];
            }
            else
            {
                Animation = idleAnimations[(int)direction];
                State = PlayerState.Idle;
            }

            Animation.Position = position;

            Animation.Update(gameTime);
        }
    }
}
