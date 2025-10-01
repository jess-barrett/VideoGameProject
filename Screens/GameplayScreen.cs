using Comora;
using GameProject2.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        private Texture2D skullSheet;
        private List<Skull> enemies = new List<Skull>();
        private float spawnTimer = 0f;
        private float spawnInterval = 5f;

        private Texture2D background;

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

            background = _content.Load<Texture2D>("background");

            player = new Player();
            camera = new Camera(ScreenManager.GraphicsDevice);

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

            player.Animation = player.idleAnimations[0];

            foreach (var animSet in animations)
            {
                foreach (var anim in animSet)
                {
                    anim.Scale = 4f;
                    int frameWidth = walkDown.Width / 8;
                    int frameHeight = walkDown.Height;
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

            player.Update(gameTime);
            camera.Position = player.Position;
            camera.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            // Check for collisions
            RotatedRectangle playerHitbox = player.RotatedHitbox;
            foreach (var skull in enemies)
            {
                if (playerHitbox.Intersects(skull.RotatedHitbox))
                {
                    System.Diagnostics.Debug.WriteLine("Collision detected!");
                    // Handle collision
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                camera,
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone
            );

            _spriteBatch.Draw(background, new Vector2(-500, -500), Color.White);
            player.Animation.Draw(_spriteBatch);

            foreach (var skull in enemies)
                skull.Draw(_spriteBatch);

            DrawRotatedHitbox(_spriteBatch, player.RotatedHitbox, Color.Green);
            foreach (var skull in enemies)
                DrawRotatedHitbox(_spriteBatch, skull.RotatedHitbox, Color.Red);

            _spriteBatch.End();
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
