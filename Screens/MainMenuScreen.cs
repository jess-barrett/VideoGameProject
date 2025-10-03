using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject2.StateManagement;

namespace GameProject2.Screens
{
    public class MainMenuScreen : MenuScreen
    {
        private Texture2D backgroundTexture;
        private Texture2D titleTexture;

        public MainMenuScreen() : base("")
        {
            var playGameMenuEntry = new MenuEntry("Play");
            var optionsMenuEntry = new MenuEntry("Settings");
            var exitMenuEntry = new MenuEntry("Quit");
            playGameMenuEntry.Scale = 1.33f;
            optionsMenuEntry.Scale = 1.33f;
            exitMenuEntry.Scale = 1.33f;
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        public override void Activate()
        {
            base.Activate();

            // Load the textures
            if (backgroundTexture == null)
            {
                backgroundTexture = ScreenManager.Game.Content.Load<Texture2D>("TitleScreenBG");
            }

            if (titleTexture == null)
            {
                titleTexture = ScreenManager.Game.Content.Load<Texture2D>("TitleScreenTitle");
            }

            AudioManager.PlayMenuMusicWithIntro();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (backgroundTexture != null)
            {
                spriteBatch.Draw(backgroundTexture,
                    new Rectangle(0, 0, viewport.Width, viewport.Height),
                    Color.White);
            }

            if (titleTexture != null)
            {
                float titleScale = 1.5f;
                int scaledWidth = (int)(titleTexture.Width * titleScale);
                int scaledHeight = (int)(titleTexture.Height * titleScale);

                int titleX = (viewport.Width - scaledWidth) / 2;
                int titleY = 100;

                spriteBatch.Draw(titleTexture,
                    new Rectangle(titleX, titleY, scaledWidth, scaledHeight),
                    Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        protected override void UpdateMenuEntryLocations()
        {
            base.UpdateMenuEntryLocations();

            var viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 position = new Vector2(0f, viewport.Height * 0.6f);

            foreach (var menuEntry in MenuEntries)
            {
                position.X = viewport.Width / 2 - menuEntry.GetWidth(this) / 2;
                menuEntry.Position = position;
                position.Y += menuEntry.GetHeight(this) + 10;
            }
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to quit?";
            var confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}