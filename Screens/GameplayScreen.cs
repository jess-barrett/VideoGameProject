using Comora;
using GameProject2.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using static System.TimeZoneInfo;

namespace GameProject2.Screens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private Player player;
        private Camera camera;
        private ParticleSystem particleSystem;

        private Texture2D skullSheet;
        private List<Skull> enemies = new List<Skull>();
        private float spawnTimer = 0f;
        private float spawnInterval = 2f;

        private Texture2D background;

        private SpriteFont instructionFont;
        private float instructionTimer = 5f;
        private string instructionText = "WASD to move | SHIFT to sprint | SPACE to attack";

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            AudioManager.PlayGameplayMusicWithIntro();

            background = _content.Load<Texture2D>("background");

            player = new Player();
            camera = new Camera(ScreenManager.GraphicsDevice);
            particleSystem = new ParticleSystem(ScreenManager.GraphicsDevice);

            instructionFont = _content.Load<SpriteFont>("InstructionFont");

            skullSheet = _content.Load<Texture2D>("skull");

            // load animations
            var idleDown = _content.Load<Texture2D>("Player/Sprites/IDLE/idle_down");
            var idleUp = _content.Load<Texture2D>("Player/Sprites/IDLE/idle_up");
            var idleLeft = _content.Load<Texture2D>("Player/Sprites/IDLE/idle_left");
            var idleRight = _content.Load<Texture2D>("Player/Sprites/IDLE/idle_right");

            var walkDown = _content.Load<Texture2D>("Player/Sprites/WALK/walk_down");
            var walkUp = _content.Load<Texture2D>("Player/Sprites/WALK/walk_up");
            var walkLeft = _content.Load<Texture2D>("Player/Sprites/WALK/walk_left");
            var walkRight = _content.Load<Texture2D>("Player/Sprites/WALK/walk_right");

            var runDown = _content.Load<Texture2D>("Player/Sprites/RUN/run_down");
            var runUp = _content.Load<Texture2D>("Player/Sprites/RUN/run_up");
            var runLeft = _content.Load<Texture2D>("Player/Sprites/RUN/run_left");
            var runRight = _content.Load<Texture2D>("Player/Sprites/RUN/run_right");

            var attack1Down = _content.Load<Texture2D>("Player/Sprites/ATTACK 1/attack1_down");
            var attack1Up = _content.Load<Texture2D>("Player/Sprites/ATTACK 1/attack1_up");
            var attack1Left = _content.Load<Texture2D>("Player/Sprites/ATTACK 1/attack1_left");
            var attack1Right = _content.Load<Texture2D>("Player/Sprites/ATTACK 1/attack1_right");

            var hurtDown = _content.Load<Texture2D>("Player/Sprites/HURT/hurt_down");
            var hurtUp = _content.Load<Texture2D>("Player/Sprites/HURT/hurt_up");
            var hurtLeft = _content.Load<Texture2D>("Player/Sprites/HURT/hurt_left");
            var hurtRight = _content.Load<Texture2D>("Player/Sprites/HURT/hurt_right");

            List<SpriteAnimation[]> animations = new List<SpriteAnimation[]>();

            player.idleAnimations[0] = new SpriteAnimation(idleDown, 8, 8);
            player.idleAnimations[1] = new SpriteAnimation(idleUp, 8, 8);
            player.idleAnimations[2] = new SpriteAnimation(idleLeft, 8, 8);
            player.idleAnimations[3] = new SpriteAnimation(idleRight, 8, 8);
            animations.Add(player.idleAnimations);

            player.walkAnimations[0] = new SpriteAnimation(walkDown, 8, 8);
            player.walkAnimations[1] = new SpriteAnimation(walkUp, 8, 8);
            player.walkAnimations[2] = new SpriteAnimation(walkLeft, 8, 8);
            player.walkAnimations[3] = new SpriteAnimation(walkRight, 8, 8);
            animations.Add(player.walkAnimations);

            player.runAnimations[0] = new SpriteAnimation(runDown, 8, 12);
            player.runAnimations[1] = new SpriteAnimation(runUp, 8, 12);
            player.runAnimations[2] = new SpriteAnimation(runLeft, 8, 12);
            player.runAnimations[3] = new SpriteAnimation(runRight, 8, 12);
            animations.Add(player.runAnimations);

            player.attack1Animations[0] = new SpriteAnimation(attack1Down, 8, 24);
            player.attack1Animations[1] = new SpriteAnimation(attack1Up, 8, 24);
            player.attack1Animations[2] = new SpriteAnimation(attack1Left, 8, 24);
            player.attack1Animations[3] = new SpriteAnimation(attack1Right, 8, 24);
            animations.Add(player.attack1Animations);

            player.hurtAnimations[0] = new SpriteAnimation(hurtDown, 4, 8);
            player.hurtAnimations[1] = new SpriteAnimation(hurtUp, 4, 8);
            player.hurtAnimations[2] = new SpriteAnimation(hurtLeft, 4, 8);
            player.hurtAnimations[3] = new SpriteAnimation(hurtRight, 4, 8);
            player.hurtAnimations[3] = new SpriteAnimation(hurtRight, 4, 8);
            animations.Add(player.hurtAnimations);

            player.Animation = player.idleAnimations[0];

            foreach (var animSet in animations)
            {
                foreach (var anim in animSet)
                {
                    anim.Scale = 4f;
                    int frameWidth = anim.GetTexture.Width / anim.FrameCount;
                    int frameHeight = anim.GetTexture.Height;
                    anim.Origin = new Vector2(frameWidth / 2f, frameHeight / 2f);
                }
            }
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (instructionTimer > 0)
                instructionTimer -= dt;

            // spawn timer
            spawnTimer += dt;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                enemies.Add(new Skull(skullSheet, 10, 10,
                    ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height));
            }

            // update all skulls
            foreach (var skull in enemies)
                skull.Update(gameTime, player);

            // Check for collisions between player and skulls
            RotatedRectangle playerHitbox = player.RotatedHitbox;
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var skull = enemies[i];

                if (playerHitbox.Intersects(skull.RotatedHitbox))
                {
                    if (player.State != PlayerState.Attack1 && player.State != PlayerState.Hurt)
                    {
                        player.State = PlayerState.Hurt;
                        player.Animation = player.hurtAnimations[(int)player.Direction];
                        player.Animation.IsLooping = false;
                        player.Animation.setFrame(0);

                        //AudioManager.PlayHurtSound();

                        particleSystem.CreateSkullDeathEffect(skull.Position);

                        enemies.RemoveAt(i);
                    }
                }
            }

            particleSystem.Update(gameTime);

            player.Update(gameTime, enemies, particleSystem);

            camera.Position = player.Position;
            camera.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                camera,
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone
            );

            // Draw background at layer 0 (behind everything)
            _spriteBatch.Draw(background, new Vector2(-500, -500), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            // Build draw list
            var drawList = new List<SpriteAnimation>();
            if (player.Animation != null)
                drawList.Add(player.Animation);
            drawList.AddRange(enemies.Select(e => e.Animation));

            // Sort by bottom Y (position + half sprite height)
            drawList = drawList
                .OrderBy(anim => anim.Position.Y + anim.FrameHeight / 2f)
                .ToList();

            // Draw everything with calculated layer depth
            foreach (var anim in drawList)
            {
                float yPosition = anim.Position.Y + anim.FrameHeight / 2f + anim.LayerDepthOffset;
                float normalizedY = yPosition / 2000f;
                float layerDepth = MathHelper.Clamp(0.9f - (normalizedY * 0.8f), 0.1f, 0.9f);

                _spriteBatch.Draw(
                    anim.GetTexture,
                    anim.Position,
                    anim.CurrentFrameRectangle,
                    anim.Color,
                    anim.Rotation,
                    anim.Origin,
                    anim.Scale,
                    anim.SpriteEffect,
                    layerDepth
                );
            }

            particleSystem.Draw(_spriteBatch, 0.05f);

            _spriteBatch.End();

            if (instructionTimer > 0)
            {
                _spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone
                );

                float alpha = MathHelper.Clamp(instructionTimer, 0f, 1f);

                Vector2 textSize = instructionFont.MeasureString(instructionText);
                Vector2 textPosition = new Vector2(
                    (ScreenManager.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                    30 // 30 pixels from top
                );

                _spriteBatch.DrawString(
                    instructionFont,
                    instructionText,
                    textPosition,
                    Color.Black * alpha
                );

                _spriteBatch.End();
            }
        }

        private Texture2D pixelTexture;

        private void DrawRotatedHitbox(SpriteBatch spriteBatch, RotatedRectangle rotRect, Color color)
        {
            if (pixelTexture == null)
            {
                pixelTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
                pixelTexture.SetData(new[] { Color.White });
            }

            Vector2[] corners = rotRect.GetCorners();

            // Draw lines between corners
            for (int i = 0; i < 4; i++)
            {
                Vector2 start = corners[i];
                Vector2 end = corners[(i + 1) % 4];
                DrawLine(spriteBatch, start, end, color * 0.5f, 2f);
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(pixelTexture,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), (int)thickness),
                null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
