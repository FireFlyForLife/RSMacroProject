using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public class VirtualKeyboard
    {
        private bool canPress;

        private IInterractionObject accessApi;

        public VirtualKeyboard(IInterractionObject api) {
            canPress = true;

            accessApi = api;
        }

        public void Disable() {
            canPress = false;
        }

        public void Enable() {
            canPress = true;
        }

        public bool Press(char c) {
            if (!canPress)
                return false;

            return accessApi.PressKey(c);
        }

        public bool Release(char c) {
            if (!canPress)
                return false;

            return accessApi.ReleaseKey(c);
        }

        public bool Type(char c, int holdTime = 5) {
            if (!canPress)
                return false;

            bool succes = true;
            //press the key
            succes = accessApi.PressKey(c) && succes;
            Thread.Sleep(holdTime);
            succes = accessApi.ReleaseKey(c) && succes;

            return succes;
        }

        public bool Type(String str, int moveTime = 20, int holdTime = 5) {
            if (!canPress)
                return false;
            //press the keys
            //SendKeys.Send(key);
            bool succes = true;
            foreach (char c in str) {
                succes = accessApi.PressKey(c) && succes;
                Thread.Sleep(holdTime);
                succes = accessApi.ReleaseKey(c) && succes;
                Thread.Sleep(moveTime);
            }

            return succes;
        }
    }
}
