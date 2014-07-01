using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlassed.Models
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point()
            : this(0, 0)
        {
        }

        public Point(string point)
        {
            var split = point.Split(',');
            int x, y;
            if (split.Count() != 2 || !Int32.TryParse(split[0], out x) || !Int32.TryParse(split[1], out y))
            {
                throw new ArgumentException("Invalid point format: " + point, "point");
            }

            X = x;
            Y = y;
        }

        public new string ToString()
        {
            return X + "," + Y;
        }

        public static string MultiToString(IEnumerable<Point> points)
        {
            string output = "";
            return points.Aggregate(output, (s, p) => s + "|" + p.ToString()).TrimStart('|');
        }

        public static List<Point> ParseMultiPointString(string points)
        {
            if (points.Length == 0) return new List<Point>();

            var list = points.Split('|');
            return list.Select(x => new Point(x)).ToList();
        }
    }

}