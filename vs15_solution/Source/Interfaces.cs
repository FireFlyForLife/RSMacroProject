using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using RSMacroProgram.Api.Type.Extentions;

namespace RSMacroProgram.Api.Type
{
    public interface Paintable
    {
        void paint(Graphics g);
    }

    public interface Clickable
    {
        void Click();
    }

    public interface Pointable
    {
        Point nextPoint();
    }

    public interface Openable
    {
        void Open();
    }

    public interface Closeable
    {
        void Close();
    }

    public interface Widget : Paintable, Clickable, Pointable
    {
    }

    public abstract class SquareWidget : Paintable
    {
        private Point offset;
        private int width, height;
        private Random random;

        protected System.Drawing.Rectangle rect {
            get { return new System.Drawing.Rectangle(offset.X, offset.Y, width, height); }
            set {
                if (value == null) draw = false;
                else offset = value.Location; width = value.Width; height = value.Height; draw = true;
            }
        }

        protected Pen pen = new Pen(Color.LightPink, 1);

        private bool draw;

        public SquareWidget() {
            draw = false;
        }

        public SquareWidget(Point offset, int width, int height) {
            draw = true;
            this.offset = offset;
            this.width = width;
            this.height = height;
            this.random = new Random(DateTime.Now.Millisecond);
        }

        public void paint(Graphics g) {
            if (draw) {
                g.DrawRectangle(pen, offset.X, offset.Y, width, height);
            }
        }

        public Point nextPoint() {
            return new Point((int)(offset.X + random.NextDouble() * width), (int)(offset.Y + random.NextDouble() * height));
        }

        public void Click() {
            //Point target = nextPoint();
            //VirtualMouse m = VirtualMouse.get;
            //m.Move(target.X, target.Y);
            //m.Click();
        }
    }

    public abstract class RoundWidget : Widget
    {
        private Point offset;
        private int radius;
        private Random random;

        protected System.Drawing.Rectangle rect {
            get { return new System.Drawing.Rectangle(offset.X, offset.Y, radius, radius); }
            set {
                if (value == null) draw = false;
                else draw = true; offset = value.Location; radius = value.Width;
            }
        }

        private bool draw;

        protected Pen pen = new Pen(Color.LightPink, 1);

        public RoundWidget() {
            draw = false;
        }

        public RoundWidget(Point offset, int radius) {
            draw = true;
            this.offset = offset;
            this.radius = radius;
            this.random = new Random(DateTime.Now.Millisecond);
        }

        public void Click() {
            Point target = nextPoint();

        }

        public void paint(Graphics g) {
            if (draw) {
                g.DrawEllipse(pen, new System.Drawing.Rectangle(offset.X, offset.Y, radius, radius));
                Polygon p = new Polygon();
                
            }
        }

        public Point nextPoint() {
            var _angle = random.NextDouble() * Math.PI * 2;
            var _radius = Math.Sqrt(random.NextDouble()) * radius;
            var _x = offset.X + radius + _radius * Math.Cos(_angle);
            var _y = offset.Y + radius + _radius * Math.Sin(_angle);
            return new Point((int)_x, (int)_y);
        }
    }

    public enum CameraAngle
    {
        COMPASS_NORTH_DEFAULT,
        COMPASS_NORTH_TOP,
        COMPASS_SOUTH_DEFAULT,
        COMPASS_SOUTH_TOP
    }

    public struct GameObject
    {
        public int gameX;
        public int gameY;

        public Polygon[] bounds;

        public CameraAngle predictedAngle;
    }

    public struct Tile : Paintable, Pointable
    {
        public int gameX;
        public int gameY;

        public Polygon screenRegion;

        public Tile[] others;

        public CameraAngle predictedAngle;

        public Tile(Polygon region, Tile[] targets, int x = 0, int y = 0, CameraAngle angle = CameraAngle.COMPASS_NORTH_TOP) {
            gameX = x;
            gameY = y;
            screenRegion = region;
            predictedAngle = angle;
            
            others = targets ?? new Tile[0];
        }

        public int X { get { return gameX; } }

        public int y { get { return gameY; } }

        public void paint(Graphics g) {
            System.Windows.Point[] points = screenRegion.Points.ToArray();
            System.Drawing.Point[] po = new System.Drawing.Point[points.Length];
            for (int i = 0; i < points.Length; i++)
                po[i] = new Point((int)points[i].X, (int)points[i].Y);
            g.DrawPolygon(Pens.Black, po);
        }

        public Point nextPoint() {
            System.Drawing.Rectangle bound = screenRegion.getBoundingBox();
            Random random = new Random();
            Point p;
            do {
                p = new Point(random.Next(bound.X, bound.Right), random.Next(bound.Y, bound.Bottom));
            }while( !screenRegion.doesContain(p) );
            return new Point(p.X, p.Y);
        }
    }

    //public struct TargetTile
    //{
    //    public int gameX;
    //    public int gameY;

    //    public Polygon screenRegion;

    //    public TargetTile(Polygon bounds, int xCoord = 0, int yCoord = 0) {
    //        gameX = xCoord;
    //        gameY = yCoord;
    //        screenRegion = bounds;
    //    }
    //}

    
}
