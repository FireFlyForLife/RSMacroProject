using RSMacroProgram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowPainterLib.Extra;
using Timer = System.Timers.Timer;

namespace WindowPainterLib
{
    public class WindowOverlay : PerPixelAlphaForm
    {

        private readonly Process process;
        private event OnOverlayPaint overlayPaint;
        private readonly Timer timer;
        private readonly Mouse mouseInstance;
        private Rectangle lastWinRect;

        public WindowOverlay(Process process) {
            this.process = process;
            this.timer = new Timer((int)(1000 / 30)); // Default 30 FPS
            this.timer.Elapsed += timer_Elapsed;
            this.mouseInstance = new Mouse(this);
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            WinAPI.SetWindowLong(this.Handle, -20, WinAPI.GetWindowLong(this.Handle, -20) | 0x80000 | 0x20);
        }

        public Process ReferencedProcess {
            get { return process; }
        }

        public Timer RefreshTimer {
            get { return timer; }
        }

        public Mouse MouseInstance {
            get { return mouseInstance; }
        }

        public event OnOverlayPaint OverlayPaint {
            add { overlayPaint += value; }
            remove { overlayPaint -= value; }
        }

        public int FPS {
            get { return (int)(1000 / timer.Interval); }
            set { timer.Interval = 1000 / value; }
        }

        public delegate void OnOverlayPaint(Graphics g);

        public new void Show() {
            base.Show();
            timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            //Console.WriteLine("Updating form");
            try {
                if (!IsDisposed)
                    UpdateForm();
            }catch(System.ObjectDisposedException ex) {
                Console.WriteLine(ex);
            }
        }

        private void UpdateForm() {
            IntPtr hWnd = process.MainWindowHandle;
            WinAPI.RECT winRect = Rectangle.Empty;

            if (WinAPI.GetForegroundWindow() == hWnd) {
                if (this.InvokeRequired) {
                    this.Invoke(new MethodInvoker(PutOnTop));
                } else {
                    PutOnTop();
                }
            }

            if (WinAPI.GetWindowRect(hWnd, ref winRect)) {
                if (!winRect.Equals(lastWinRect)) {
                    WinAPI.RECT clientRect;

                    if (WinAPI.GetClientRect(hWnd, out clientRect)) {
                        if (this.InvokeRequired) {
                            this.Invoke(new MethodInvoker(delegate {
                                UpdateLocationAndSize(winRect, clientRect);
                            }));
                        } else {
                            UpdateLocationAndSize(winRect, clientRect);
                        }

                        lastWinRect = winRect;
                    }
                }
            }

            this.Invalidate();
        }

        private void PutOnTop() {
            this.TopMost = true;
            this.TopMost = false;
        }

        private void UpdateLocationAndSize(Rectangle winRect, Rectangle clientRect) {
            int borderWidth = (winRect.Width - clientRect.Width) / 2;
            int titlebarHeight = (winRect.Height - clientRect.Height - 2 * borderWidth) + borderWidth;

            this.Width = clientRect.Width;
            this.Height = clientRect.Height;
            this.Left = winRect.X + borderWidth;
            this.Top = winRect.Y + titlebarHeight;

            
            RSMacroProgram.Api.Configuration.screen = new Rectangle(Left, Top, Width, Height);
        }

        public new Graphics CreateGraphics() {
            throw new NotImplementedException("All drawing must be done by adding an OnOverlayPaint event.");
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (overlayPaint != null) {
                using (Bitmap buffer = new Bitmap(this.Width, this.Height)) {
                    using (Graphics g = Graphics.FromImage(buffer)) {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        overlayPaint(g);
                        SetBitmap(buffer);
                    }
                }
            }
        }

        public new void Dispose() {
            if (this.InvokeRequired) {
                ExecuteDisposeOperations();
            } else {
                ExecuteDisposeOperations();
            }
        }

        private void ExecuteDisposeOperations() {
            timer.Stop();
            this.Close();
            base.Dispose();
        }

    }

    



    /// <para>Your PerPixel form should inherit this class</para>
    /// <author><name>Rui Godinho Lopes</name><email>rui@ruilopes.com</email></author>
    public class PerPixelAlphaForm : Form
    {
        public PerPixelAlphaForm() {
            // This form should not have a border or else Windows will clip it.
            FormBorderStyle = FormBorderStyle.None;
        }


        /// <para>Changes the current bitmap.</para>
        public void SetBitmap(Bitmap bitmap) {
            SetBitmap(bitmap, 255);
        }


        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        public void SetBitmap(Bitmap bitmap, byte opacity) {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                Win32.SIZE size = new Win32.SIZE(bitmap.Width, bitmap.Height);
                Win32.POINT pointSource = new Win32.POINT(0, 0);
                Win32.POINT topPos = new Win32.POINT(Left, Top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            } finally {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero) {
                    Win32.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }


        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }
    }

    public static class FPSManager
    {

        private static Thread executionThread;
        private static Stopwatch stopwatch;
        private static volatile int fps;
        private static volatile bool running;

        static FPSManager() {
            fps = 30;
            stopwatch = new Stopwatch();
        }

        public static int FPS {
            get { return fps; }
            set { fps = value; }
        }

        public static bool Running {
            get { return running; }
        }

        public static void Start(Action target) {
            if (executionThread == null) {
                running = true;
                executionThread = new Thread(new ParameterizedThreadStart(ExecutionMethod));
                executionThread.Start(target);
            }
        }

        public static void Stop() {
            running = false;
            executionThread = null;
        }

        private static void ExecutionMethod(object target) {
            while (running) {
                try {
                    stopwatch.Reset();
                    ((Action)target)();

                    long sleepMillis = 1000 / fps - stopwatch.ElapsedMilliseconds;

                    if (sleepMillis > 0) {
                        Thread.Sleep((int)sleepMillis);
                    }
                } catch (Exception) {
                    // ignored
                }
            }

            stopwatch.Stop();
        }

    }

    namespace Extra
    {
        public class Mouse
        {

            private readonly WindowOverlay overlay;
            private readonly WindMouse windMouse;
            private Point mousePosition;
            private readonly MousePath mousePath;
            private readonly LinkedList<Point> movementPositions;
            private Pen mouseCrossPen;

            public Mouse(WindowOverlay overlay) {
                this.overlay = overlay;
                this.windMouse = new WindMouse(this);
                this.mousePosition = new Point(-1, -1);
                this.mousePath = new MousePath();
                this.movementPositions = new LinkedList<Point>();
                this.mouseCrossPen = new Pen(Color.Red, 2);
            }

            public Point MousePosition {
                get { return mousePosition; }
            }

            public Pen MouseCrossPen {
                get { return mouseCrossPen; }
                set { mouseCrossPen = value; }
            }

            public void Enable() {
                overlay.OverlayPaint += Mouse_Paint;
            }

            public void Disable() {
                overlay.OverlayPaint -= Mouse_Paint;
            }

            public void Move(Point point) {
                Move(point.X, point.Y);
            }

            public void Move(int x, int y) {
                int sleepTime = 1000 / overlay.FPS;
                Thread movementThread = new Thread(delegate () {
                    windMouse.Move(x, y);
                });

                movementThread.Start();

                while (movementThread.IsAlive) {
                        if(overlay.IsHandleCreated)
                            overlay.Invoke(new MethodInvoker(Application.DoEvents));
                        else
                            Console.WriteLine("Overlay Handle is not created");
                    Thread.Sleep(sleepTime);
                }
            }

            private void Mouse_Paint(Graphics g) {
                mousePath.Draw(g, movementPositions);
                movementPositions.Clear();

                int x = mousePosition.X;
                int y = mousePosition.Y;

                if (x >= 0 && y >= 0) {
                    g.DrawLine(mouseCrossPen, x - 7, y, x + 7, y);
                    g.DrawLine(mouseCrossPen, x, y - 7, x, y + 7);
                }
            }

            private class WindMouse
            {

                private readonly Mouse mouse;
                private double mouseSpeed;
                private Point randomXY;
                private readonly Random r;

                public WindMouse(Mouse mouse) {
                    this.mouse = mouse;
                    this.mouseSpeed = 10;
                    this.randomXY = new Point(5, 5);
                    this.r = new Random();
                }

                public double MouseSpeed {
                    get { return mouseSpeed; }
                    set { mouseSpeed = value; }
                }

                public Point RandomXY {
                    get { return randomXY; }
                    set { randomXY = value; }
                }

                public void Move(int x, int y) {
                    this.MoveMouse(x, y, this.randomXY.X, this.randomXY.Y);
                }

                private void setXY(int x, int y) {
                    Point p = new Point(x, y);

                    mouse.movementPositions.AddLast(p);
                    mouse.mousePosition = p;
                }

                /// <summary>
                /// Written by Benland100.
                /// </summary>
                private void WindMouseImplementation(double xs, double ys, double xe, double ye, double gravity, double wind, double minWait, double maxWait, double maxStep, double targetArea) {
                    double sqrt2 = Math.Sqrt(2);
                    double sqrt3 = Math.Sqrt(3);
                    double sqrt5 = Math.Sqrt(5);
                    double dist = Hypotenuse(xe - xs, ye - ys);
                    double windX = 0, windY = 0, veloX = 0, veloY = 0, veloMag = 0, randomDist = 0, step = 0;
                    int lastX, lastY;
                    while ((dist = Hypotenuse(xe - xs, ye - ys)) > 1) {
                        wind = Math.Min(wind, dist);
                        if (dist >= targetArea) {
                            windX = windX / sqrt3 + (random(Math.Round(wind, MidpointRounding.AwayFromZero) * 2 + 1) - wind) / sqrt5;
                            windY = windY / sqrt3 + (random(Math.Round(wind, MidpointRounding.AwayFromZero) * 2 + 1) - wind) / sqrt5;
                        } else {
                            windX = windX / sqrt2;
                            windY = windY / sqrt2;
                            if (maxStep < 3) {
                                maxStep = random(3) + 3.0;
                            } else {
                                maxStep = maxStep / sqrt5;
                            }
                        }
                        veloX = veloX + windX;
                        veloY = veloY + windY;
                        veloX = veloX + gravity * (xe - xs) / dist;
                        veloY = veloY + gravity * (ye - ys) / dist;
                        if (Hypotenuse(veloX, veloY) > maxStep) {
                            randomDist = maxStep / 2 + random(Math.Round(maxStep, MidpointRounding.AwayFromZero) / 2);
                            veloMag = Math.Sqrt(veloX * veloX + veloY * veloY);
                            veloX = (veloX / veloMag) * randomDist;
                            veloY = (veloY / veloMag) * randomDist;
                        }
                        lastX = (int)Math.Round(xs, MidpointRounding.AwayFromZero);
                        lastY = (int)Math.Round(ys, MidpointRounding.AwayFromZero);
                        xs = xs + veloX;
                        ys = ys + veloY;
                        if (lastX != Math.Round(xs, MidpointRounding.AwayFromZero) || lastY != Math.Round(ys, MidpointRounding.AwayFromZero)) {
                            this.setXY((int)Math.Round(xs, MidpointRounding.AwayFromZero), (int)Math.Round(ys, MidpointRounding.AwayFromZero));
                        }
                        step = Hypotenuse(xs - lastX, ys - lastY);
                        Thread.Sleep((int)(Math.Round((maxWait - minWait) * (step / maxStep) + minWait, MidpointRounding.AwayFromZero)));
                    }
                    if (Math.Round(xe, MidpointRounding.AwayFromZero) != Math.Round(xs, MidpointRounding.AwayFromZero) || Math.Round(ye, MidpointRounding.AwayFromZero) != Math.Round(ys, MidpointRounding.AwayFromZero)) {
                        this.setXY((int)Math.Round(xe, MidpointRounding.AwayFromZero), (int)Math.Round(ye, MidpointRounding.AwayFromZero));
                    }
                }

                /// <summary>
                /// Written by Benland100.
                /// </summary>
                private void MoveMouse(int dx, int dy, int rx, int ry) {
                    int sx = mouse.MousePosition.X;
                    int sy = mouse.MousePosition.Y;
                    if (sx < 0) {
                        sx = 0;
                    }
                    if (sy < 0) {
                        sy = 0;
                    }
                    double randSpeed = (random(this.mouseSpeed) / 2.0 + this.mouseSpeed) / 10;
                    if (randSpeed == 0.0) {
                        randSpeed = 0.1;
                    }
                    dx = (int)Math.Round(dx + random(rx), MidpointRounding.AwayFromZero);
                    dy = (int)Math.Round(dy + random(ry), MidpointRounding.AwayFromZero);
                    this.WindMouseImplementation(sx, sy, dx, dy, 9.0, 3.0, 10.0 / randSpeed, 15.0 / randSpeed, 10.0 * randSpeed, 10.0 * randSpeed);
                }

                private double random(double max) {
                    return r.NextDouble() * max;
                }

                private static double Hypotenuse(double a, double b) {
                    return Math.Sqrt(a * a + b * b);
                }

            }

        }

        public class MousePath
        {

            public LinkedList<MousePathPoint> MousePathPoints { get; private set; }
            private readonly Color trailColor;
            private double rainbowIndex;

            public MousePath() {
                this.MousePathPoints = new LinkedList<MousePathPoint>();
                this.trailColor = Color.Red;
            }

            public MousePath(Color trailColor) {
                this.MousePathPoints = new LinkedList<MousePathPoint>();
                this.trailColor = trailColor;
            }

            public void Draw(Graphics g, LinkedList<Point> additionalPoints) {
                while (MousePathPoints.Count > 0 && MousePathPoints.First.Value.IsUp) {
                    MousePathPoints.RemoveFirst();
                }

                foreach (Point p in additionalPoints.ToArray()) { // Prevent an exception if the list is modified while being iterated over
                    rainbowIndex += 0.02;

                    if (rainbowIndex > 1) {
                        rainbowIndex = 0;
                    }

                    MousePathPoint mpp = new MousePathPoint(p.X, p.Y, 350, HSL2RGB(rainbowIndex, 0.68, 0.68));

                    if (MousePathPoints.Count == 0 || !MousePathPoints.Last.Value.Equals(mpp)) {
                        MousePathPoints.AddLast(mpp);
                    }
                }

                if (MousePathPoints.Count > 0) {
                    MousePathPoint lastPoint = MousePathPoints.First.Value;

                    foreach (MousePathPoint p in MousePathPoints) {
                        g.DrawLine(new Pen(Color.FromArgb(p.Alpha, p.BaseColor), 1), p.X, p.Y, lastPoint.X, lastPoint.Y);
                        lastPoint = p;
                    }
                }
            }

            /// <summary>
            /// Code from StackOverflow. Given H,S,L in range of 0-1.
            /// </summary>
            /// <param name="h"></param>
            /// <param name="sl"></param>
            /// <param name="l"></param>
            /// <returns>A Color (RGB struct) in range of 0-255</returns>
            private ColorRGB HSL2RGB(double h, double sl, double l) {
                double r = l;
                double g = l;
                double b = l;
                double v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
                if (v > 0) {
                    double m = l + l - v;
                    double sv = (v - m) / v;
                    h *= 6.0;
                    int sextant = (int)h;
                    double fract = h - sextant;
                    double vsf = v * sv * fract;
                    double mid1 = m + vsf;
                    double mid2 = v - vsf;
                    switch (sextant) {
                        case 0:
                            r = v;
                            g = mid1;
                            b = m;
                            break;
                        case 1:
                            r = mid2;
                            g = v;
                            b = m;
                            break;
                        case 2:
                            r = m;
                            g = v;
                            b = mid1;
                            break;
                        case 3:
                            r = m;
                            g = mid2;
                            b = v;
                            break;
                        case 4:
                            r = mid1;
                            g = m;
                            b = v;
                            break;
                        case 5:
                            r = v;
                            g = m;
                            b = mid2;
                            break;
                    }
                }
                ColorRGB rgb;
                rgb.R = Convert.ToByte(r * 255.0f);
                rgb.G = Convert.ToByte(g * 255.0f);
                rgb.B = Convert.ToByte(b * 255.0f);
                return rgb;
            }

            public struct MousePathPoint
            {

                private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                private readonly int x, y, lastingTime, alpha;
                private readonly long finishTime;
                private readonly Color baseColor;

                public MousePathPoint(int x, int y, int lastingTime, Color baseColor) {
                    this.x = x;
                    this.y = y;
                    this.lastingTime = lastingTime;
                    this.baseColor = baseColor;
                    this.finishTime = CurrentTimeMillis() + lastingTime;
                    this.alpha = 255;
                }

                public int X {
                    get { return x; }
                }

                public int Y {
                    get { return y; }
                }

                public int LastingTime {
                    get { return lastingTime; }
                }

                public Color BaseColor {
                    get { return baseColor; }
                }

                public long FinishTime {
                    get { return finishTime; }
                }

                public int Alpha {
                    get {
                        int newAlpha = ((int)((finishTime - CurrentTimeMillis()) / (lastingTime / alpha)));

                        if (newAlpha > 255) {
                            newAlpha = 255;
                        }

                        if (newAlpha < 0) {
                            newAlpha = 0;
                        }

                        return newAlpha;
                    }
                }

                public bool IsUp {
                    get { return CurrentTimeMillis() >= finishTime; }
                }

                private static long CurrentTimeMillis() {
                    return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
                }

            }

        }

        // Code from StackOverflow
        public struct ColorRGB
        {

            public byte R;
            public byte G;
            public byte B;

            public ColorRGB(Color value) {
                this.R = value.R;
                this.G = value.G;
                this.B = value.B;
            }

            public static implicit operator Color(ColorRGB rgb) {
                return Color.FromArgb(rgb.R, rgb.G, rgb.B);
            }

            public static explicit operator ColorRGB(Color c) {
                return new ColorRGB(c);
            }

        }
    }
}
