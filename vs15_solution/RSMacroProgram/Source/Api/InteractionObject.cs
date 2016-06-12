using RSMacroProgramApi.MacroApi.Generic;
using RSMacroProgramApi.MacroApi.RS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Point = RSMacroProgramApi.MacroApi.Generic.Point;

namespace RSMacroProgram.Source.Api
{
    public sealed class InteractionObject : MarshalByRefObject, IInteractionObject
    {
        public Size AdBanner
        {
            get
            {
                RSGame g = game;
                if (g.Equals(RSGame.OSRS))
                    return RSMacroProgramApi.MacroApi.RS.OSRS.OSRSConstants.AdBanner;
                else if (g.Equals(RSGame.RS3) || g.Equals(RSGame.DS) || g.Equals(RSGame.NXT))//TODO: Look if NXT has the same AdBanner boundries.
                    return RSMacroProgramApi.MacroApi.RS.RS3.RS3Constants.AdBanner;
                else
                    return Size.Empty;
            }
        }

        public RSGame game
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Rectangle TargetWindow
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasAdBanner
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates a base Api, this is the building block from which all the other apis need the interraction methods.
        /// You cannot create this object from a script, it is supplied to you in the 'api' property
        /// </summary>
        /// <exception cref="System.Security.SecurityException"/>
        internal InteractionObject() {
            SecurityPermission perm = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            perm.Demand();
        }

        public Point MousePosition() {
            Win32.POINT p;
            bool b = WinAPI.GetCursorPos(out p);
            if (b)
                return new Point(p.x, p.y);
            else
                return new Point(-1, -1);
        }

        public bool Move(int x, int y) {
            bool b = WinAPI.SetCursorPos(x, y);
            return b;
        }

        public bool Click() {
            bool b = false;
            return b;
        }

        public bool InputKey(char c) {
            bool b = false;
            return b;
        }

        public bool InputKeys(String str) {
            bool b = true;
            foreach (char c in str) b = InputKey(c) && b;
            return b;
        }

        public bool Click(int key) {
            throw new NotImplementedException();
        }

        public bool MousePress(int key) {
            throw new NotImplementedException();
        }

        public bool MouseRelease(int key) {
            throw new NotImplementedException();
        }

        public bool PressKey(char c) {
            throw new NotImplementedException();
        }

        public bool ReleaseKey(char c) {
            throw new NotImplementedException();
        }
    }
}
