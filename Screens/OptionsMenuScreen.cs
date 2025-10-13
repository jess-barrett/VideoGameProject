using GameProject2.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject2.Screens
{
    public class OptionsMenuScreen : MenuScreen
    {
        private Texture2D backgroundTexture;

        private readonly MenuEntry _musicVolumeMenuEntry;
        private readonly MenuEntry _sfxVolumeMenuEntry;

        private static float _musicVolume = 0.5f;
        private static float _sfxVolume = 1.0f;

        public OptionsMenuScreen() : base("")
        {
            _musicVolumeMenuEntry = new MenuEntry(string.Empty);
            _sfxVolumeMenuEntry = new MenuEntry(string.Empty);

            AudioManager.MusicVolume = _musicVolume;
            AudioManager.SFXVolume = _sfxVolume;

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            _musicVolumeMenuEntry.Selected += MusicVolumeMenuEntrySelected;
            _sfxVolumeMenuEntry.Selected += SfxVolumeMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(_musicVolumeMenuEntry);
            MenuEntries.Add(_sfxVolumeMenuEntry);
            MenuEntries.Add(back);
        }

        public override void Activate()
        {
            base.Activate();

            if (backgroundTexture == null)
            {
                backgroundTexture = ScreenManager.Game.Content.Load<Texture2D>("TitleScreenBG");
            }
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

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UpdateMenuEntryLocations()
        {
            base.UpdateMenuEntryLocations();

            var viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 position = new Vector2(0f, viewport.Height * 0.62f);

            foreach (var menuEntry in MenuEntries)
            {
                position.X = viewport.Width / 2 - menuEntry.GetWidth(this) / 2;
                menuEntry.Position = position;
                position.Y += menuEntry.GetHeight(this) + 10;
            }
        }

        private void SetMenuEntryText()
        {
            int musicPercent = (int)(_musicVolume * 100);
            int sfxPercent = (int)(_sfxVolume * 100);

            _musicVolumeMenuEntry.Text = $"Music Volume: {musicPercent}%";
            _sfxVolumeMenuEntry.Text = $"SFX Volume: {sfxPercent}%";
        }

        private void MusicVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _musicVolume += 0.1f;
            if (_musicVolume > 1.01f)
                _musicVolume = 0.0f;

            _musicVolume = MathHelper.Clamp(_musicVolume, 0f, 1f);

            AudioManager.MusicVolume = _musicVolume;

            SetMenuEntryText();
        }

        private void SfxVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _sfxVolume += 0.1f;
            if (_sfxVolume > 1.01f)
                _sfxVolume = 0.0f;

            _sfxVolume = MathHelper.Clamp(_sfxVolume, 0f, 1f);

            AudioManager.SFXVolume = _sfxVolume;

            SetMenuEntryText();
        }
    }
}