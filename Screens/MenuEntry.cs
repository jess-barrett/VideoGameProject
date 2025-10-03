using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject2.StateManagement;

namespace GameProject2.Screens
{
    public class MenuEntry
    {
        private string _text;
        private float _selectionFade;
        private Vector2 _position;
        private float _baseScale = 1.0f;

        public string Text
        {
            private get => _text;
            set => _text = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Scale
        {
            get => _baseScale;
            set => _baseScale = value;
        }

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
        }

        public MenuEntry(string text)
        {
            _text = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
            if (isSelected)
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            else
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            var color = Color.White;
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = _baseScale * (1 + pulsate * 0.05f * _selectionFade);
            color *= screen.TransitionAlpha;

            var screenManager = screen.ScreenManager;
            var spriteBatch = screenManager.SpriteBatch;
            var font = screenManager.Font;
            var origin = new Vector2(0, font.LineSpacing / 2);

            if (isSelected)
            {
                Color outlineColor = Color.Black * screen.TransitionAlpha;
                int outlineThickness = 2;

                for (int x = -outlineThickness; x <= outlineThickness; x++)
                {
                    for (int y = -outlineThickness; y <= outlineThickness; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            spriteBatch.DrawString(font, _text, _position + new Vector2(x, y),
                                outlineColor, 0, origin, scale, SpriteEffects.None, 0);
                        }
                    }
                }
            }

            // Draw the main text on top
            spriteBatch.DrawString(font, _text, _position, color, 0,
                origin, scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return (int)(screen.ScreenManager.Font.LineSpacing * _baseScale);
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)(screen.ScreenManager.Font.MeasureString(Text).X * _baseScale);
        }
    }
}