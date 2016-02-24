using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using RSMacroProgram.Api.Type;
using System.Windows.Forms;
using RSMacroProgram.Api;
using WindowPainterLib;
using System.Security.Permissions;
using System.Security;
using Point = RSMacroProgram.Api.Type.Point;
using System.Runtime.InteropServices;
using RSMacroProgram.Api.Exceptions;
using System.Diagnostics;

namespace RSMacroProgram.Api
{
    [Serializable]
    public abstract class BaseApi
    {
        public InterractionObject api;
        public VirtualMouse mouse;
        public VirtualKeyboard keyboard;

        public BaseApi(InterractionObject api) {
            this.api = api;
            mouse = new VirtualMouse(api);
            keyboard = new VirtualKeyboard(api);
        }
    }

    public sealed class DataAccessObject : MarshalByRefObject
    {
        internal DataAccessObject() {
            
        }

        public Size AdBanner {
            get { return Configuration.AdBanner; }
        }

        public RSGame game {
            internal set { Configuration.game = value; }
            get { return Configuration.game; }
        }

        public Rectangle rsScreen {
            internal set { Configuration.screen = value; }
            get { return Configuration.screen; }
        }

        public bool HasAdBanner {
            internal set { Configuration.HasAdBanner = value; }
            get { return Configuration.HasAdBanner; }
        }

        public Size NonResizableViewport { get { return Configuration.OSRS.NonResizableViewport; } }
        public bool resizable { get { return Configuration.OSRS.resizable; } }
        public Rectangle viewport {
            get { return Configuration.OSRS.viewport; }
        }

        private class OSRS
        {
            public Size NonResizableViewport { get { return Configuration.OSRS.NonResizableViewport; } }
            public bool resizable { get { return Configuration.OSRS.resizable; } }
            public Rectangle viewport {
                get { return Configuration.OSRS.viewport; }
            }
        }
    }

    public sealed class InterractionObject : MarshalByRefObject 
    {

        /// <summary>
        /// Creates a base Api, this is the building block from which all the other apis need the interraction methods.
        /// You cannot create this object from a script, it is supplied to you in the 'api' propperty
        /// </summary>
        /// <exception cref="System.Security.SecurityException" cref="RSMacroProgram.Api.Exceptions.HookingException" />
        internal InterractionObject() {
            SecurityPermission perm = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            perm.Demand();

            //hook in windows
            //hookID = setHook(Win32.HookingIDs.WH_Mouse_LL, LLCallback);
            //Console.WriteLine(hookID);
            //if (hookID == IntPtr.Zero)
            //    Console.WriteLine("Could not hook mouse_LL input");
                //throw new HookingException("Could not hook mouse_LL input");
        }

        ~InterractionObject() {
            //if(hookID != IntPtr.Zero) {
            //    //unhook mouse
            //    if (!removeHook(hookID))
            //        Debug.WriteLine("Could not remove hook!");
            //}
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

        /*private static Standard standardInstance = new Standard();
        public static Standard standard {
            get { return standardInstance; }
        }

        private static RSMacroProgram.Api.OSRS.OSRSApi OSRSInstance = new RSMacroProgram.Api.OSRS.OSRSApi();
        public static RSMacroProgram.Api.OSRS.OSRSApi OSRS {
            get { return OSRSInstance; }
        }

        private static RSMacroProgram.Api.RS3.RS3Api RS3Instance = new RSMacroProgram.Api.RS3.RS3Api();
        public static RSMacroProgram.Api.RS3.RS3Api RS3 {
            get { return RS3Instance; }
        }*/
    }

    internal class Configuration
    {
        #region Constants
        public static readonly Size AdBanner = new Size(761, 96);
        #endregion

        #region Variables
        public static RSGame game {
            internal set;
            get;
        }

        private static Rectangle drawableRect;
        public static Rectangle screen {
            internal set { drawableRect = value; }
            get { return drawableRect; }
        }

        private static bool ad;
        public static bool HasAdBanner {
            internal set { ad = value; }
            get { return ad; }
        }
        #endregion

        public static class OSRS
        {
            #region Constants
            public static readonly Size NonResizableViewport = new Size(765, 503);

            #endregion
            #region Variables
            public static bool resizable = false;
            public static Rectangle viewport {
                get 
                {
                    if (resizable) {
                        return screen;
                    } else {
                        Rectangle rect = screen;
                        int x = rect.X + (rect.Width - NonResizableViewport.Width) / 2;
                        int y = rect.Y + (HasAdBanner ? (AdBanner.Height) : 0);
                        return new Rectangle(x, y, NonResizableViewport.Width, NonResizableViewport.Height);
                    }
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// Class which is scanned for when a script is loaded, this should be treaded as your main class. for access to the api classes see: 
    /// <seealso cref="AutoScript.api"/> and: 
    /// <seealso cref="AutoScript.config"/>
    /// </summary>
    public abstract class AutoScript
    {
        private InterractionObject apiInstance;
        private DataAccessObject configInstance;

        public InterractionObject api {
            set {
                if (apiInstance == null) 
                    apiInstance = value;
            }

            get {
                return apiInstance;
            }
        }

        public DataAccessObject config {
            set {
                if (configInstance == null)
                    configInstance = value;
            }

            get {
                return configInstance;
            }
        }

        /// <summary>
        /// This method is called at first and needs to returned for start() to be called.
        /// </summary>
        public abstract void init();

        /// <summary>
        /// This method is called after init() has returned and does not have to return for other methods to be called.
        /// </summary>
        public abstract void start();

        /// <summary>
        /// This method is continuesly called. After it is returned a 100 ms delay is added to prevent overloading. 
        /// </summary>
        public abstract void tick();

        /// <summary>
        /// This method is called when you script needs to stop. The program does not wait until this method has returned.
        /// </summary>
        public abstract void dispose();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptAttributes : Attribute
    {
        public String name { private set; get; }
        public String description { private set; get; }
        public String version { private set; get; }
        public String author { private set; get; }
        public bool pausable { private set; get; }

        public ScriptAttributes(String Name, String Description, String Version = "0.1", String Author = "", bool Pausable = false) {
            this.name = Name;
            this.description = Description;
            this.version = Version;
            this.author = Author;
            this.pausable = pausable;
        }
    }

    [Serializable]
    public class VirtualMouse
    {
        //#region Singleton
        //private static VirtualMouse instance;
        //public static VirtualMouse get {
        //    protected set { instance = value; }
        //    get {
        //        if (instance == null)
        //            instance = new VirtualMouse();
        //        return instance;
        //    }
        //}
        //#endregion
        private bool clickEnabled;
        private bool moveEnabled;

        private InterractionObject accessApi;

        public VirtualMouse(InterractionObject api) {
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

        public bool Click() {
            if (!clickEnabled)
                return false;
            //do the clickening.
            Console.WriteLine("RMB down");
            Thread.Sleep(100);
            Console.WriteLine("RMB up");

            return true;
        }

        public bool Move(Point point) {
            return Move(point.X, point.Y);
        }

        public bool Move(int x, int y) {
            if (!moveEnabled)
                return false;
            //move the mouse
            int sX = Configuration.screen.X + x;
            int sY = Configuration.screen.Y + y;
            Console.WriteLine("Moved mouse to: " + sX.ToString() + "," + sY.ToString());
            
            return accessApi.Move(sX, sY); ;
        }

        public bool Move(Pointable target) {
            if (!moveEnabled)
                return false;

            //move the mouse
            Console.WriteLine("Moving mouse to Pointable: {0}", target);
            WinAPI.SetCursorPos(target.nextPoint().X, target.nextPoint().Y);
            

            return true;
        }
    }

    [Serializable]
    public class VirtualKeyboard
    {
        //#region Singleton
        //private static VirtualKeyboard instance;
        //public static VirtualKeyboard get {
        //    set { instance = value; }
        //    get {
        //        if (instance == null)
        //            instance = new VirtualKeyboard();
        //        return instance;
        //    }
        //}
        //#endregion
        private bool canPress;

        private InterractionObject accessApi;

        public VirtualKeyboard(InterractionObject api) {
            canPress = true;

            accessApi = api;
        }

        public void Disable() {
            canPress = false;
        }

        public void Enable() {
            canPress = true;
        }

        public bool Press(String key, float holdTime = 0.1f) {
            if (!canPress)
                return false;
            //press the keys
            //SendKeys.Send(key);
            Console.WriteLine("Pressed key: " + key);

            return true;
        }
    }

}


