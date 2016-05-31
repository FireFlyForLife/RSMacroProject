using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public class VirtualMouse
    {
        private bool clickEnabled;
        private bool moveEnabled;

        private IInterractionObject accessApi;

        public VirtualMouse(IInterractionObject api) {
            clickEnabled = true;
            moveEnabled = true;

            accessApi = api;
        }

        public void Disable() {
            clickEnabled = false;
            moveEnabled = false;
        }

        public void Enable() {
            clickEnabled = true;
            moveEnabled = true;
        }

        public bool Click(int holdTime = 100) {
            if (!clickEnabled)
                return false;

            bool succes = true;
            //do the clickening.
            succes = succes && accessApi.MousePress(0);
            Thread.Sleep(holdTime);
            succes = succes && accessApi.MouseRelease(0);

            return true;
        }

        public bool Move(Point point) {
            return Move(point.X, point.Y);
        }

        public bool Move(int x, int y) {
            if (!moveEnabled)
                return false;
            //move the mouse
            int sX = accessApi.TargetWindow.X + x;
            int sY = accessApi.TargetWindow.Y + y;
            Console.WriteLine("Moved mouse to: " + sX.ToString() + "," + sY.ToString());

            return accessApi.Move(sX, sY); ;
        }

        public bool Move(Pointable target) {
            if (!moveEnabled)
                return false;

            //move the mouse
            Console.WriteLine("Moving mouse to Pointable: {0}", target);
            return accessApi.Move(target.nextPoint().X, target.nextPoint().Y);
        }
    }
}
