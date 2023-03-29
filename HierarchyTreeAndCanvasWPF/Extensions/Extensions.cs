using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace HierarchyTreeAndCanvasWPF.Extensions
{
    public static class Extensions
    {
        public static void ShiftLeftSide(this FrameworkElement element, double units)
        {
            if (element is Polygon triangle)
            {
                List<double> pointsXAxis = triangle.Points.Select(p => p.X).ToList();
                PointCollection newPoints = new PointCollection();

                Point leftPoint;
                Point topPoint;
                Point rightPoint;

                foreach (Point point in triangle.Points)
                {
                    if (point.X == pointsXAxis.Min())
                    {
                        leftPoint = point;
                    }
                    else if (point.X > pointsXAxis.Min() && point.X < pointsXAxis.Max())
                    {
                        topPoint = point;
                    }
                    else if (point.X == pointsXAxis.Max())
                    {
                        rightPoint = point;
                    }
                }

                Point newLeftPoint = leftPoint;
                Point newTopPoint = topPoint;
                Point newRightPoint = rightPoint;

                // triangle right side is limit
                if (leftPoint.X + units < pointsXAxis.Max())
                {
                    // takes care of moving left point
                    Canvas.SetLeft(triangle, Canvas.GetLeft(triangle) + units);
                    // adjust right point based on how much left point moves
                    newRightPoint = new Point(rightPoint.X - units, rightPoint.Y);
                }
                // min width is 1 unit
                else
                {
                    Canvas.SetLeft(triangle, Canvas.GetLeft(triangle) + topPoint.X - 0.5);
                    newRightPoint = new Point(topPoint.X + 0.5, rightPoint.Y);
                }

                // accurately calculates inbetween point
                newTopPoint = new Point((newRightPoint.X - newLeftPoint.X) / 2, topPoint.Y);

                newPoints.Add(newLeftPoint);
                newPoints.Add(newTopPoint);
                newPoints.Add(newRightPoint);
                triangle.Points = newPoints;
            }
        }

        public static void ShiftTopSide(this FrameworkElement element, double units)
        {
            if (element is Polygon triangle)
            {
                List<double> pointsXAxis = triangle.Points.Select(p => p.X).ToList();
                PointCollection newPoints = new PointCollection();

                Point leftPoint;
                Point topPoint;
                Point rightPoint;

                foreach (Point point in triangle.Points)
                {
                    if (point.X == pointsXAxis.Min())
                    {
                        leftPoint = point;
                    }
                    else if (point.X > pointsXAxis.Min() && point.X < pointsXAxis.Max())
                    {
                        topPoint = point;
                    }
                    else if (point.X == pointsXAxis.Max())
                    {
                        rightPoint = point;
                    }
                }

                Point newLeftPoint = leftPoint;
                Point newTopPoint = topPoint;
                Point newRightPoint = rightPoint;
                double minHeight = 1;
                double height = leftPoint.Y - topPoint.Y;
                double newTop = Canvas.GetTop(triangle) + units;
                double newHeight = height - units;

                // triangle bottom side is limit
                if (newHeight < minHeight)
                {
                    newTop = Canvas.GetTop(triangle) + height - minHeight;
                    newHeight = minHeight;
                }

                Canvas.SetTop(triangle, newTop);
                newLeftPoint = new Point(leftPoint.X, newHeight);
                newRightPoint = new Point(rightPoint.X, newHeight);

                newPoints.Add(newLeftPoint);
                newPoints.Add(newTopPoint);
                newPoints.Add(newRightPoint);
                triangle.Points = newPoints;
            }
        }

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
