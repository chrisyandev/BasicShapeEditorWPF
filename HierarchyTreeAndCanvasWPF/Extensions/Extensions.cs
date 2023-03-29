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
/*        private static void ResizeTriangle(Polygon triangle, double horizontalChange, double verticalChange)
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
        }*/

        public static void ShiftRightSide(this FrameworkElement element, double units)
        {
            if (element is Polygon triangle)
            {
                List<double> pointsXAxis = triangle.Points.Select(p => p.X).ToList();
                PointCollection newPoints = new PointCollection();

                foreach (Point point in triangle.Points)
                {
                    // leftmost point
                    if (point.X == pointsXAxis.Min())
                    {
                        newPoints.Add(new Point(point.X, point.Y));
                    }
                    // rightmost point
                    else if (point.X == pointsXAxis.Max())
                    {
                        // detect if right side will overlap with left, leave a 1 unit gap between left and right side
                        if (pointsXAxis.Max() + units > pointsXAxis.Min())
                        {
                            newPoints.Add(new Point(pointsXAxis.Max() + units, point.Y));
                        }
                        else
                        {
                            newPoints.Add(new Point(pointsXAxis.Min() + 1, point.Y));
                        }
                    }
                    // inbetween point
                    else if (point.X > pointsXAxis.Min() && point.X < pointsXAxis.Max())
                    {
                        // detect if right side will overlap with left, leave a 0.5 unit gap between left -> mid and mid -> right
                        if (pointsXAxis.Max() + units > pointsXAxis.Min())
                        {
                            newPoints.Add(new Point(point.X + (units / 2), point.Y));
                        }
                        else
                        {
                            newPoints.Add(new Point(pointsXAxis.Min() + 0.5, point.Y));
                        }
                    }
                }
                triangle.Points = newPoints;
            }
        }

        public static void ShiftBottomSide(this FrameworkElement element, double units)
        {
            if (element is Polygon triangle)
            {
                List<double> pointsXAxis = triangle.Points.Select(p => p.X).ToList();
                PointCollection newPoints = new PointCollection();

                foreach (Point point in triangle.Points)
                {
                    // leftmost point
                    if (point.X == pointsXAxis.Min())
                    {
                        newPoints.Add(new Point(point.X, point.Y + units));
                    }
                    // rightmost point
                    else if (point.X == pointsXAxis.Max())
                    {
                        newPoints.Add(new Point(point.X, point.Y + units));
                    }
                    // inbetween point
                    else
                    {
                        newPoints.Add(new Point(point.X, point.Y));
                    }
                }
                triangle.Points = newPoints;
            }
        }
    }
}
