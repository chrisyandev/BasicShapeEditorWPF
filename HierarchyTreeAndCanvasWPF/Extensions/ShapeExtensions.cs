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
using System.Printing;

namespace HierarchyTreeAndCanvasWPF.Extensions
{
    public static class ShapeExtensions
    {
        public static void ShiftLeftSide(this Shape shape, double units, double leftBoundary)
        {
            if (shape is Rectangle rectangle)
            {
                ShiftLeftSideShapeWithWidthAndHeight(rectangle, units, leftBoundary);
            }
            else if (shape is Ellipse ellipse)
            {
                ShiftLeftSideShapeWithWidthAndHeight(ellipse, units, leftBoundary);
            }
            else if (shape is Polygon triangle)
            {
                ShiftLeftSideTriangle(triangle, units);
            }
        }

        public static void ShiftTopSide(this Shape shape, double units, double topBoundary)
        {
            if (shape is Rectangle rectangle)
            {
                ShiftTopSideShapeWithWidthAndHeight(rectangle, units, topBoundary);
            }
            else if (shape is Ellipse ellipse)
            {
                ShiftTopSideShapeWithWidthAndHeight(ellipse, units, topBoundary);
            }
            else if (shape is Polygon triangle)
            {
                ShiftTopSideTriangle(triangle, units);
            }
        }

        public static void ShiftRightSide(this Shape shape, double units, double rightBoundary)
        {
            if (shape is Rectangle rectangle)
            {
                ShiftRightSideShapeWithWidthAndHeight(rectangle, units, rightBoundary);
            }
            else if (shape is Ellipse ellipse)
            {
                ShiftRightSideShapeWithWidthAndHeight(ellipse, units, rightBoundary);
            }
            else if (shape is Polygon triangle)
            {
                ShiftRightSideTriangle(triangle, units);
            }
        }

        public static void ShiftBottomSide(this Shape shape, double units, double bottomBoundary)
        {
            if (shape is Rectangle rectangle)
            {
                ShiftBottomSideShapeWithWidthAndHeight(rectangle, units, bottomBoundary);
            }
            else if (shape is Ellipse ellipse)
            {
                ShiftBottomSideShapeWithWidthAndHeight(ellipse, units, bottomBoundary);
            }
            else if (shape is Polygon triangle)
            {
                ShiftBottomSideTriangle(triangle, units);
            }
        }

        private static void ShiftLeftSideShapeWithWidthAndHeight(Shape shape, double units, double leftBoundary)
        {
            double left = Canvas.GetLeft(shape);
            double right = left + shape.Width;

            double newWidth, newLeft;

            // shape right edge is the limit
            if (left + units > right)
            {
                newWidth = 0;
                newLeft = right;
            }
            else
            {
                newWidth = shape.Width - units;
                newLeft = left + units;
            }

            // canvas left side is the limit
            if (newLeft < leftBoundary)
            {
                double unitsOverLimit = leftBoundary - newLeft;
                newWidth -= unitsOverLimit;
                newLeft += unitsOverLimit;
            }

            shape.Width = newWidth;
            Canvas.SetLeft(shape, newLeft);
        }

        private static void ShiftTopSideShapeWithWidthAndHeight(Shape shape, double units, double topBoundary)
        {
            double top = Canvas.GetTop(shape);
            double bottom = top + shape.Height;

            double newHeight, newTop;

            // shape bottom edge is the limit
            if (top + units > bottom)
            {
                newHeight = 0;
                newTop = bottom;
            }
            else
            {
                newHeight = shape.Height - units;
                newTop = top + units;
            }

            // canvas top side is the limit
            if (newTop < topBoundary)
            {
                double unitsOverLimit = topBoundary - newTop;
                newHeight -= unitsOverLimit;
                newTop += unitsOverLimit;
            }

            shape.Height = newHeight;
            Canvas.SetTop(shape, newTop);
        }

        private static void ShiftRightSideShapeWithWidthAndHeight(Shape shape, double units, double rightBoundary)
        {
            double left = Canvas.GetLeft(shape);

            double newWidth;

            // shape left edge is the limit
            if (shape.Width + units < 0)
            {
                newWidth = 0;
            }
            else
            {
                newWidth = shape.Width + units;
            }

            // canvas right side is the limit
            if (left + newWidth > rightBoundary)
            {
                double unitsOverLimit = (left + newWidth) - rightBoundary;
                newWidth -= unitsOverLimit;
            }

            shape.Width = newWidth;
        }

        private static void ShiftBottomSideShapeWithWidthAndHeight(Shape shape, double units, double bottomBoundary)
        {
            double top = Canvas.GetTop(shape);

            double newHeight;

            // shape top edge is the limit
            if (shape.Height + units < 0)
            {
                newHeight = 0;
            }
            else
            {
                newHeight = shape.Height + units;
            }

            // canvas bottom side is the limit
            if (top + newHeight > bottomBoundary)
            {
                double unitsOverLimit = (top + newHeight) - bottomBoundary;
                newHeight -= unitsOverLimit;
            }

            shape.Height = newHeight;
        }

        private static void ShiftLeftSideTriangle(Polygon triangle, double units)
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
            double minWidth = 1;
            double width = rightPoint.X - leftPoint.X;
            double newLeft = Canvas.GetLeft(triangle) + units;
            double newWidth = width - units;

            // triangle right side is limit
            if (newWidth < minWidth)
            {
                newLeft = Canvas.GetLeft(triangle) + width - minWidth;
                newWidth = minWidth;
            }

            Canvas.SetLeft(triangle, newLeft);
            newTopPoint = new Point(newWidth / 2, topPoint.Y);
            newRightPoint = new Point(newWidth, rightPoint.Y);

            newPoints.Add(newLeftPoint);
            newPoints.Add(newTopPoint);
            newPoints.Add(newRightPoint);
            triangle.Points = newPoints;
        }

        private static void ShiftTopSideTriangle(Polygon triangle, double units)
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

        private static void ShiftRightSideTriangle(Polygon triangle, double units)
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

        private static void ShiftBottomSideTriangle(Polygon triangle, double units)
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
