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
        /// <returns>How many units actually shifted</returns>
        public static double ShiftLeftSide(this Shape shape, double units, double leftBoundary, uint minWidth = 5)
        {
            if (shape is Rectangle rectangle)
            {
                return ShiftLeftSideShapeWithWidthAndHeight(rectangle, units, leftBoundary, minWidth);
            }
            else if (shape is Ellipse ellipse)
            {
                return ShiftLeftSideShapeWithWidthAndHeight(ellipse, units, leftBoundary, minWidth);
            }
            else if (shape is Polygon triangle)
            {
                return ShiftLeftSideTriangle(triangle, units, leftBoundary, minWidth);
            }

            return 0;
        }

        /// <returns>How many units actually shifted</returns>
        public static double ShiftTopSide(this Shape shape, double units, double topBoundary, uint minHeight = 5)
        {
            if (shape is Rectangle rectangle)
            {
                return ShiftTopSideShapeWithWidthAndHeight(rectangle, units, topBoundary, minHeight);
            }
            else if (shape is Ellipse ellipse)
            {
                return ShiftTopSideShapeWithWidthAndHeight(ellipse, units, topBoundary, minHeight);
            }
            else if (shape is Polygon triangle)
            {
                return ShiftTopSideTriangle(triangle, units, topBoundary, minHeight);
            }

            return 0;
        }

        /// <returns>How many units actually shifted</returns>
        public static double ShiftRightSide(this Shape shape, double units, double rightBoundary, uint minWidth = 5)
        {
            if (shape is Rectangle rectangle)
            {
                return ShiftRightSideShapeWithWidthAndHeight(rectangle, units, rightBoundary, minWidth);
            }
            else if (shape is Ellipse ellipse)
            {
                return ShiftRightSideShapeWithWidthAndHeight(ellipse, units, rightBoundary, minWidth);
            }
            else if (shape is Polygon triangle)
            {
                return ShiftRightSideTriangle(triangle, units, rightBoundary, minWidth);
            }

            return 0;
        }

        /// <returns>How many units actually shifted</returns>
        public static double ShiftBottomSide(this Shape shape, double units, double bottomBoundary, uint minHeight = 5)
        {
            if (shape is Rectangle rectangle)
            {
                return ShiftBottomSideShapeWithWidthAndHeight(rectangle, units, bottomBoundary, minHeight);
            }
            else if (shape is Ellipse ellipse)
            {
                return ShiftBottomSideShapeWithWidthAndHeight(ellipse, units, bottomBoundary, minHeight);
            }
            else if (shape is Polygon triangle)
            {
                return ShiftBottomSideTriangle(triangle, units, bottomBoundary, minHeight);
            }

            return 0;
        }

        private static double ShiftLeftSideShapeWithWidthAndHeight(Shape shape, double units, double leftBoundary, uint minWidth)
        {
            double width = shape.Width;
            double left = Canvas.GetLeft(shape);
            double right = left + width;

            double newWidth, newLeft;

            // shape right side is the limit
            if (left + units > right - minWidth)
            {
                newWidth = minWidth;
                newLeft = right - minWidth;
            }
            else
            {
                newWidth = width - units;
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
            return newWidth - width;
        }

        private static double ShiftTopSideShapeWithWidthAndHeight(Shape shape, double units, double topBoundary, uint minHeight)
        {
            double height = shape.Height;
            double top = Canvas.GetTop(shape);
            double bottom = top + height;

            double newHeight, newTop;

            // shape bottom side is the limit
            if (top + units > bottom - minHeight)
            {
                newHeight = minHeight;
                newTop = bottom - minHeight;
            }
            else
            {
                newHeight = height - units;
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
            return newHeight - height;
        }

        private static double ShiftRightSideShapeWithWidthAndHeight(Shape shape, double units, double rightBoundary, uint minWidth)
        {
            double width = shape.Width;
            double left = Canvas.GetLeft(shape);

            double newWidth;

            // shape left side is the limit
            if (width + units < minWidth)
            {
                newWidth = minWidth;
            }
            else
            {
                newWidth = width + units;
            }

            // canvas right side is the limit
            if (left + newWidth > rightBoundary)
            {
                double unitsOverLimit = (left + newWidth) - rightBoundary;
                newWidth -= unitsOverLimit;
            }

            shape.Width = newWidth;
            return newWidth - width;
        }

        private static double ShiftBottomSideShapeWithWidthAndHeight(Shape shape, double units, double bottomBoundary, uint minHeight)
        {
            double height = shape.Height;
            double top = Canvas.GetTop(shape);

            double newHeight;

            // shape top side is the limit
            if (height + units < minHeight)
            {
                newHeight = minHeight;
            }
            else
            {
                newHeight = height + units;
            }

            // canvas bottom side is the limit
            if (top + newHeight > bottomBoundary)
            {
                double unitsOverLimit = (top + newHeight) - bottomBoundary;
                newHeight -= unitsOverLimit;
            }

            shape.Height = newHeight;
            return newHeight - height;
        }

        private static double ShiftLeftSideTriangle(Polygon triangle, double units, double leftBoundary, uint minWidth)
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
            double width = rightPoint.X - leftPoint.X;
            double newLeft = Canvas.GetLeft(triangle) + units;
            double newWidth = width - units;

            // triangle right side is limit
            if (newWidth < minWidth)
            {
                newLeft = Canvas.GetLeft(triangle) + width - minWidth;
                newWidth = minWidth;
            }

            // canvas left side is limit
            if (newLeft < leftBoundary)
            {
                double unitsOverLimit = leftBoundary - newLeft;
                newWidth -= unitsOverLimit;
                newLeft += unitsOverLimit;
            }

            Canvas.SetLeft(triangle, newLeft);
            newTopPoint = new Point(newWidth / 2, topPoint.Y);
            newRightPoint = new Point(newWidth, rightPoint.Y);

            newPoints.Add(newLeftPoint);
            newPoints.Add(newTopPoint);
            newPoints.Add(newRightPoint);
            triangle.Points = newPoints;

            return newWidth - width;
        }

        private static double ShiftTopSideTriangle(Polygon triangle, double units, double topBoundary, uint minHeight)
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
            double height = leftPoint.Y - topPoint.Y;
            double newTop = Canvas.GetTop(triangle) + units;
            double newHeight = height - units;

            // triangle bottom side is limit
            if (newHeight < minHeight)
            {
                newTop = Canvas.GetTop(triangle) + height - minHeight;
                newHeight = minHeight;
            }

            // canvas top side is the limit
            if (newTop < topBoundary)
            {
                double unitsOverLimit = topBoundary - newTop;
                newHeight -= unitsOverLimit;
                newTop += unitsOverLimit;
            }

            Canvas.SetTop(triangle, newTop);
            newLeftPoint = new Point(leftPoint.X, newHeight);
            newRightPoint = new Point(rightPoint.X, newHeight);

            newPoints.Add(newLeftPoint);
            newPoints.Add(newTopPoint);
            newPoints.Add(newRightPoint);
            triangle.Points = newPoints;

            return newHeight - height;
        }

        private static double ShiftRightSideTriangle(Polygon triangle, double units, double rightBoundary, uint minWidth)
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
            double width = rightPoint.X - leftPoint.X;
            double newWidth = width + units;

            // triangle left side is limit
            if (newWidth < minWidth)
            {
                newWidth = minWidth;
            }

            // canvas right side is the limit
            if (Canvas.GetLeft(triangle) + newWidth > rightBoundary)
            {
                double unitsOverLimit = (Canvas.GetLeft(triangle) + newWidth) - rightBoundary;
                newWidth -= unitsOverLimit;
            }

            newRightPoint = new Point(newWidth, rightPoint.Y);
            newTopPoint = new Point(newWidth / 2, topPoint.Y);

            newPoints.Add(newLeftPoint);
            newPoints.Add(newTopPoint);
            newPoints.Add(newRightPoint);
            triangle.Points = newPoints;

            return newWidth - width;
        }

        private static double ShiftBottomSideTriangle(Polygon triangle, double units, double bottomBoundary, uint minHeight)
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
            double height = leftPoint.Y - topPoint.Y;
            double newHeight = height + units;

            // triangle top side is limit
            if (newHeight < minHeight)
            {
                newHeight = minHeight;
            }

            // canvas bottom side is the limit
            if (Canvas.GetTop(triangle) + newHeight > bottomBoundary)
            {
                double unitsOverLimit = (Canvas.GetTop(triangle) + newHeight) - bottomBoundary;
                newHeight -= unitsOverLimit;
            }

            newLeftPoint = new Point(leftPoint.X, newHeight);
            newRightPoint = new Point(rightPoint.X, newHeight);

            newPoints.Add(newLeftPoint);
            newPoints.Add(newTopPoint);
            newPoints.Add(newRightPoint);
            triangle.Points = newPoints;

            return newHeight - height;
        }
    }
}
