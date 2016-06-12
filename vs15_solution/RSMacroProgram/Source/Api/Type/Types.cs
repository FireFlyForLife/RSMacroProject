using RSMacroProgramApi.MacroApi.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RSMacroProgram.Api.Type
{
    public class Triangle
    {
        Point[] points;
        public Triangle(params Point[] _points) {
            points = _points;
        }

        public double Area() {
            if(points.Length == 2) {
                Point a = points[0];
                Point b = points[1];
                return Math.Abs((a.X - b.X)*(a.Y - b.Y));
            }else if (points.Length == 3) {
                Point a = points[0];
                Point b = points[1];
                Point c = points[2];
                return Math.Abs((a.X - c.X) * (b.Y - a.Y) - (a.X - b.X) * (c.Y - a.Y));
            }
            return 0;
        }

        public Polygon ToPolygon() {
            Polygon poly = new Polygon();
            PointCollection pCol = new PointCollection(points.Length);
            foreach(Point p in points)
                pCol.Add(p);
            poly.Points = pCol;
            return poly;
        }

        public static implicit operator Polygon(Triangle t) {
            Polygon poly = new Polygon();
            Point[] points = t.points;
            PointCollection pCol = new PointCollection(points.Length);
            foreach (Point p in points)
                pCol.Add(p);
            poly.Points = pCol;
            return poly;
        }
    }

    //public struct Point
    //{
    //    double x, y;

    //    public Point(double x, double y) {
    //        this.x = x;
    //        this.y = y;
    //    }

    //    public Point(int x, int y) : this((double)x, y) { }

    //    //public Point(System.Drawing.Point p) : this(p.X, p.Y) { }

    //    //public Point(System.Windows.Point p) : this(p.X, p.Y) { }

    //    //public Point() : this(0, 0) { }

    //    public double DoubleX { get { return x; } }

    //    public double DoubleY { get { return y; } }

    //    public int X { get { return (int)x; } }

    //    public int Y { get { return (int)y; } }

    //    public static implicit operator Point(System.Drawing.Point p) {
    //        return new Point(p.X, p.Y);
    //    }

    //    public static implicit operator Point(System.Windows.Point p) {
    //        return new Point(p.X, p.Y);
    //    }

    //    public static implicit operator System.Drawing.Point(Point p) {
    //        return new System.Drawing.Point((int)p.X, (int)p.Y);
    //    }

    //    public static implicit operator System.Windows.Point(Point p) {
    //        return new System.Windows.Point(p.X, p.Y);
    //    }
    //}
}
