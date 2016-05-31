using System;
using System.Collections.Generic;
using System.Text;

namespace RSMacroProgramApi.MacroApi.Generic
{
    public struct Point
    {
        double x, y;

        public Point(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public Point(int x, int y) : this((double)x, y) { }

        //public Point(System.Drawing.Point p) : this(p.X, p.Y) { }

        //public Point(System.Windows.Point p) : this(p.X, p.Y) { }

        //public Point() : this(0, 0) { }

        public double DoubleX { get { return x; } }

        public double DoubleY { get { return y; } }

        public int X { get { return (int)x; } }

        public int Y { get { return (int)y; } }

        public static implicit operator Point(System.Drawing.Point p) {
            return new Point(p.X, p.Y);
        }

        public static implicit operator Point(System.Windows.Point p) {
            return new Point(p.X, p.Y);
        }

        public static implicit operator System.Drawing.Point(Point p) {
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }

        public static implicit operator System.Windows.Point(Point p) {
            return new System.Windows.Point(p.X, p.Y);
        }
    }
}
