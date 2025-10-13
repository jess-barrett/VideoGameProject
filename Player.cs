using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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

        public Direction Direction = Direction.Down;

        private bool isMoving = false;

        private float footstepTimer = 0f;
        private float walkFootstepInterval = 0.5f;
        private float runFootstepInterval = 0.3f;

        public PlayerState State { get; set; } = PlayerState.Idle;

        public SpriteAnimation Animation;

        public SpriteAnimation[] idleAnimations = new SpriteAnimation[4];
        public SpriteAnimation[] walkAnimations = new SpriteAnimation[4];
        public SpriteAnimation[] runAnimations = new SpriteAnimation[4];
        public SpriteAnimation[] attack1Animations = new SpriteAnimation[4];
        public SpriteAnimation[] attack2Animations = new SpriteAnimation[4];
        public SpriteAnimation[] hurtAnimations = new SpriteAnimation[4];

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
                    switch (Direction)
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

        public void Update(GameTime gameTime, List<Skull> skulls, ParticleSystem particleSystem)
        {
            KeyboardState kbState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // === HURT LOGIC ===
            if (State == PlayerState.Hurt)
            {
                Animation.Position = position;
                Animation.Update(gameTime);

                // End hurt state when animation finishes
                if (Animation.CurrentFrameIndex == Animation.FrameCount - 1)
                {
                    State = PlayerState.Idle;
                }

                // Freeze movement while hurt
                return;
            }

            // === ATTACK LOGIC ===
            if (State == PlayerState.Attack1)
            {
                Animation.Update(gameTime);

                // Attack hitbox in front of player
                Rectangle attackHitbox = Hitbox;
                int range = 40;
                switch (Direction)
                {
                    case Direction.Up: attackHitbox.Y -= range; break;
                    case Direction.Down: attackHitbox.Y += range; break;
                    case Direction.Left: attackHitbox.X -= range; break;
                    case Direction.Right: attackHitbox.X += range; break;
                }

                // Destroy skulls
                foreach (var skull in skulls.ToArray())
                {
                    if (attackHitbox.Intersects(skull.Hitbox))
                    {
                        particleSystem.CreateSkullDeathEffect(skull.Position);
                        skulls.Remove(skull);
                    }    
                }

                // End attack when animation finishes
                if (Animation.CurrentFrameIndex == Animation.FrameCount - 1)
                {
                    State = PlayerState.Idle;
                }

                // Do not process movement while attacking
                return;
            }

            // === MOVEMENT INPUT ===
            isMoving = false;

            if (kbState.IsKeyDown(Keys.D)) { Direction = Direction.Right; isMoving = true; }
            if (kbState.IsKeyDown(Keys.A)) { Direction = Direction.Left; isMoving = true; }
            if (kbState.IsKeyDown(Keys.W)) { Direction = Direction.Up; isMoving = true; }
            if (kbState.IsKeyDown(Keys.S)) { Direction = Direction.Down; isMoving = true; }

            // === ATTACK TRIGGER ===
            if (kbState.IsKeyDown(Keys.Space))
            {
                State = PlayerState.Attack1;
                Animation = attack1Animations[(int)Direction];
                Animation.IsLooping = false;
                Animation.setFrame(0);
                Animation.Position = position;
                return;
            }

            // === MOVEMENT LOGIC ===
            if (isMoving)
            {
                State = kbState.IsKeyDown(Keys.LeftShift) ? PlayerState.Run : PlayerState.Walk;
                int speed = State == PlayerState.Walk ? walkSpeed : runSpeed;

                switch (Direction)
                {
                    case Direction.Down: position.Y += speed * dt; break;
                    case Direction.Up: position.Y -= speed * dt; break;
                    case Direction.Left: position.X -= speed * dt; break;
                    case Direction.Right: position.X += speed * dt; break;
                }

                Animation = State == PlayerState.Walk
                    ? walkAnimations[(int)Direction]
                    : runAnimations[(int)Direction];

                // Footsteps
                footstepTimer += dt;
                float interval = State == PlayerState.Walk ? walkFootstepInterval : runFootstepInterval;
                if (footstepTimer >= interval)
                {
                    footstepTimer = 0f;
                    float pitchVariation = (float)(new Random().NextDouble() * 0.2 - 0.1);
                    float volume = State == PlayerState.Walk ? 0.6f : 0.8f;
                    AudioManager.PlayFootstep(volume, pitchVariation);
                }
            }
            else
            {
                State = PlayerState.Idle;
                Animation = idleAnimations[(int)Direction];
                footstepTimer = 0f;
            }

            Animation.Position = position;
            Animation.Update(gameTime);
        }
    }
}
