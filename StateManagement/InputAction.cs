using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameProject2.StateManagement
{
    public class InputAction
    {
        private readonly Buttons[] _buttons;
        private readonly Keys[] _keys;
        private readonly bool _firstPressOnly;

        private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex player);
        private delegate bool KeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex player);
        
        public InputAction(Buttons[] triggerButtons, Keys[] triggerKeys, bool firstPressOnly)
        {
            _buttons = triggerButtons != null ? triggerButtons.Clone() as Buttons[] : new Buttons[0];
            _keys = triggerKeys != null ? triggerKeys.Clone() as Keys[] : new Keys[0];
            _firstPressOnly = firstPressOnly;
        }

        public bool Occurred(InputState stateToTest, PlayerIndex? playerToTest, out PlayerIndex player)
        {
            ButtonPress buttonTest;
            KeyPress keyTest;

            if (_firstPressOnly)
            {
                buttonTest = stateToTest.IsNewButtonPress;
                keyTest = stateToTest.IsNewKeyPress;
            }
            else
            {
                buttonTest = stateToTest.IsButtonPressed;
                keyTest = stateToTest.IsKeyPressed;
            }

            foreach (var button in _buttons)
            {
                if (buttonTest(button, playerToTest, out player))
                    return true;
            }
            foreach (var key in _keys)
            {
                if (keyTest(key, playerToTest, out player))
                    return true;
            }

            player = PlayerIndex.One;
            return false;
        }
    }
}
