using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProject2.StateManagement;

namespace GameProject2.Screens
{
    public class MessageBoxScreen : GameScreen
    {
        private readonly string _message;
        private Texture2D _gradientTexture;
        private readonly InputAction _menuSelect;
        private readonly InputAction _menuCancel;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        public MessageBoxScreen(string message, bool includeUsageText = true)
        {
            const string usageText = "\nA button, Space, Enter = ok" +
                                     "\nB button, Backspace = cancel";

            if (includeUsageText)
                _message = message + usageText;
            else
                _message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            _menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space }, true);
            _menuCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }

        public override void Activate()
        {
            var content = ScreenManager.Game.Content;
                _gradientTexture = content.Load<Texture2D>("gradient");
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (_menuSelect.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Accepted?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
            else if (_menuCancel.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Cancelled?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = font.MeasureString(_message);
            var textPosition = (viewportSize - textSize) / 2;

            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                (int)textPosition.Y - vPad, (int)textSize.X + hPad * 2, (int)textSize.Y + vPad * 2);

            var color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            spriteBatch.Draw(_gradientTexture, backgroundRectangle, color);
            spriteBatch.DrawString(font, _message, textPosition, color);

            spriteBatch.End();
        }
    }
}
