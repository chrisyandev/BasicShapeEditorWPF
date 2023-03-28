using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace HierarchyTreeAndCanvasWPF.Extensions
{
    public static class Extensions
    {
        private static void ResizeTriangle(Polygon triangle, double horizontalChange, double verticalChange)
        {
            List<double> pointsXAxis = triangle.Points.Select(p => p.X).ToList();
            PointCollection newPoints = new PointCollection();

            foreach (Point point in triangle.Points)
            {
                // leftmost point
                if (point.X == pointsXAxis.Min())
                {
                    Debug.WriteLine(($"min {point.X}"));
                    newPoints.Add(new Point(point.X - (horizontalChange / 2), point.Y + verticalChange));
                }
                // rightmost point
                else if (point.X == pointsXAxis.Max())
                {
                    Debug.WriteLine(($"max {point.X}"));
                    newPoints.Add(new Point(point.X + (horizontalChange / 2), point.Y + verticalChange));
                }
                // inbetween point
                else
                {
                    Debug.WriteLine(($"between {point.X}"));
                    newPoints.Add(new Point(point.X, point.Y));
                }
            }
            triangle.Points = newPoints;
        }

        public static void Resize(this FrameworkElement element, double horizontalChange, double verticalChange)
        {
            if (element is Polygon triangle)
            {
                ResizeTriangle(triangle, horizontalChange, verticalChange);
            }
        }
    }
}
