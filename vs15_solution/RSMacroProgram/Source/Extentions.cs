using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace RSMacroProgram.Api.Type.Extentions
{
    public static class PolygonExtention
    {
        public static bool doesContain(this Polygon polygon, Point p) {
            Point p1, p2;

            System.Windows.Point[] poly = polygon.Points.ToArray();

            bool inside = false;

            if (poly.Length < 3) {
                return inside;
            }

            Point oldPoint = new Point(
            poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++) {
                Point newPoint = new Point(poly[i].X, poly[i].Y);

                if (newPoint.X > oldPoint.X) {
                    p1 = oldPoint;
                    p2 = newPoint;
                } else {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)
                 < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X)) {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }
        
        public static System.Drawing.Rectangle getBoundingBox(this Polygon polygon) {
            int boundsMinX = Int32.MaxValue;
            int boundsMinY = Int32.MaxValue;
            int boundsMaxX = Int32.MinValue;
            int boundsMaxY = Int32.MinValue;
            foreach (System.Windows.Point p in polygon.Points) {
                Point point = p;
                int x = (int)p.X;
                boundsMinX = Math.Min(boundsMinX, x);
                boundsMaxX = Math.Max(boundsMaxX, x);
                int y = (int)p.Y;
                boundsMinY = Math.Min(boundsMinY, y);
                boundsMaxY = Math.Max(boundsMaxY, y);
            }
            return new System.Drawing.Rectangle(boundsMinX, boundsMinY,
                               boundsMaxX - boundsMinX,
                               boundsMaxY - boundsMinY);
        }
    }
}
