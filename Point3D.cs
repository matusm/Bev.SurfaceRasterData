using System;
using System.Globalization;

namespace Bev.SurfaceRasterData
{
    public class Point3D : IComparable<Point3D>
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Point3D point) : this(point.X, point.Y, point.Z) { }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public override string ToString()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            return $"[Point3D - X:{X} Y:{Y} Z:{Z}]";
        }

        // this is a specialized ordering schema usefull for profile data only 
        public int CompareTo(Point3D other)
        {
            if (X < other.X) return -1;
            if (X > other.X) return 1;
            return 0;
        }
    }
}
