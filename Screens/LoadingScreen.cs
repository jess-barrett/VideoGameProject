using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject2.StateManagement;

namespace GameProject2.Screens
{
    public class LoadingScreen : GameScreen
    {
        private readonly bool _loadingIsSlow;
        private bool _otherScreensAreGone;
        private readonly GameScreen[] _screensToLoad;
        private Texture2D backgroundTexture;

        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
        {
            foreach (var screen in screenManager.GetScreens())
                screen.ExitScreen();

            var loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }

        public override void Activate()
        {
            base.Activate();

            if (backgroundTexture == null)
            {
                backgroundTexture = ScreenManager.Game.Content.Load<Texture2D>("TitleScreenBG");
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (var screen in _screensToLoad)
                {
                    if (screen != null)
                        ScreenManager.AddScreen(screen, ControllingPlayer);
                }

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (ScreenState == ScreenState.Active && ScreenManager.GetScreens().Length == 1)
                _otherScreensAreGone = true;

            if (_loadingIsSlow)
            {
                var spriteBatch = ScreenManager.SpriteBatch;
                var font = ScreenManager.Font;
                var viewport = ScreenManager.GraphicsDevice.Viewport;

                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                // Draw background
                if (backgroundTexture != null)
                {
                    spriteBatch.Draw(backgroundTexture,
                        new Rectangle(0, 0, viewport.Width, viewport.Height),
                        Color.White);
                }

                // Draw loading text
                const string message = "Loading Game...";
                var viewportSize = new Vector2(viewport.Width, viewport.Height);
                var textSize = font.MeasureString(message);
                var textPosition = (viewportSize - textSize) / 2;
                var color = Color.White;

                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }
        }
    }
}