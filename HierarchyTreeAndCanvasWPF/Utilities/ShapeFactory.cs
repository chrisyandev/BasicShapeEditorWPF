using HierarchyTreeAndCanvasWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Utilities
{
    public enum ShapeType
    {
        Rectangle,
        Ellipse,
        Triangle
    }

    public static class ShapeFactory
    {
        public static Shape CreateShape(ShapeType shapeType, double width, double height, Brush fill)
        {
            if (shapeType == ShapeType.Rectangle)
            {
                return CreateRectangle(width, height, fill);
            }
            else if (shapeType == ShapeType.Ellipse)
            {
                return CreateEllipse(width, height, fill);
            }
            else if (shapeType == ShapeType.Triangle)
            {
                return CreateTriangle(width, height, fill);
            }

            return null;
        }

        private static Rectangle CreateRectangle(double width, double height, Brush fill)
        {
            Rectangle newRectangle = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill
            };

            return newRectangle;
        }

        private static Ellipse CreateEllipse(double width, double height, Brush fill)
        {
            Ellipse newEllipse = new Ellipse
            {
                Width = width,
                Height = height,
                Fill = fill
            };

            return newEllipse;
        }

        private static Polygon CreateTriangle(double width, double height, Brush fill)
        {
            Polygon newTriangle = new Polygon
            {
                Points = new PointCollection
                    {
                        new Point(0, height), // leftmost point
                        new Point(width / 2, 0), // topmost point
                        new Point(width, height) // rightmost point
                    },
                Fill = fill
            };

            return newTriangle;
        }
    }
}
