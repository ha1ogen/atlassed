using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Models.MapData
{
    public class Coordinate
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Coordinate()
            : this(0.0, 0.0)
        {
        }

        public static Coordinate Parse(string coordinate)
        {
            var split = coordinate.Split(',');
            double x, y;
            if (split.Count() != 2 || !Double.TryParse(split[0], out x) || !Double.TryParse(split[1], out y))
            {
                throw new ArgumentException("Invalid point format: " + coordinate, "point");
            }

            return new Coordinate(x, y);
        }

        public new string ToString()
        {
            return X + "," + Y;
        }

        public static string MultiToString(IEnumerable<Coordinate> points)
        {
            string output = "";
            return points.Aggregate(output, (s, p) => s + "|" + p.ToString()).TrimStart('|');
        }

        public static List<Coordinate> ParseMultiCoordinateString(string points)
        {
            if (points.Length == 0) return new List<Coordinate>();

            var list = points.Split('|');
            return list.Select(x => Coordinate.Parse(x)).ToList();
        }
    }

}