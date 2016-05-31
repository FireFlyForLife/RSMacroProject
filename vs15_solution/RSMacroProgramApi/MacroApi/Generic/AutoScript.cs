using System;
using System.Collections.Generic;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public abstract class AutoScript : AbstractScript
    {
        public VirtualMouse mouse;
        public VirtualKeyboard keyboard;
        public GameScreen gameScreen;

        public override IInterractionObject api
        {
            get
            {
                return base.api;
            }

            set
            {
                if(base.api != null) {
                    base.api = value;
                    mouse = new VirtualMouse(value);
                    keyboard = new VirtualKeyboard(value);
                    gameScreen = new GameScreen(value);
                }
            }
        }
    }
}
