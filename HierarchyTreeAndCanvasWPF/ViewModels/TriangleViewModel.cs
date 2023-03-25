using HierarchyTreeAndCanvasWPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.ViewModels
{
    public class TriangleViewModel : ShapeViewModelBase
    {
        private CanvasItem _canvasItem;
        private Polygon _triangle;

        public TriangleViewModel(CanvasItem canvasItem)
        {
            _canvasItem = canvasItem;
            _triangle = _canvasItem.Shape as Polygon;
        }

        /*        public double Width
                {
                    get
                    {
                        List<double> pointsXAxis = _triangle.Points.Select(p => p.X).ToList();
                        return pointsXAxis.Max() - pointsXAxis.Min();
                    }
                    set
                    {
                        List<double> pointsXAxis = _triangle.Points.Select(p => p.X).ToList();
                        PointCollection newPoints = new PointCollection();

                        foreach (Point point in _triangle.Points)
                        {
                            // leftmost point
                            if (point.X == pointsXAxis.Min())
                            {
                                newPoints.Add(new Point(point.X - (value / 2), point.Y));
                            }
                            // rightmost point
                            else if (point.X == pointsXAxis.Max())
                            {
                                newPoints.Add(new Point(point.X + (value / 2), point.Y));
                            }
                            // inbetween point
                            else
                            {
                                newPoints.Add(new Point(point.X, point.Y));
                            }
                        }

                        _triangle.Points = newPoints;
                        OnPropertyChanged();

                        Debug.WriteLine(Width);
                    }
                }

                public double Height
                {
                    get
                    {
                        List<double> pointsYAxis = _triangle.Points.Select(p => p.Y).ToList();
                        return pointsYAxis.Max() - pointsYAxis.Min();
                    }
                    set
                    {
                        List<double> pointsYAxis = _triangle.Points.Select(p => p.Y).ToList();
                        PointCollection newPoints = new PointCollection();

                        foreach (Point point in _triangle.Points)
                        {
                            // bottommost point
                            if (point.X == pointsYAxis.Min())
                            {
                                newPoints.Add(new Point(point.X, point.Y - (value / 2)));
                            }
                            // topmost point
                            else if (point.X == pointsYAxis.Max())
                            {
                                newPoints.Add(new Point(point.X, point.Y + (value / 2)));
                            }
                            // inbetween point
                            else
                            {
                                newPoints.Add(new Point(point.X, point.Y));
                            }
                        }

                        _triangle.Points = newPoints;
                        OnPropertyChanged();

                        Debug.WriteLine(Height);
                    }
                }*/

        public PointCollection Points
        {
            get
            {
                return _triangle.Points;
            }
        }

        public Brush Fill
        {
            get { return _triangle.Fill; }
            set
            {
                if (value == _triangle.Fill) { return; }
                _triangle.Fill = value;
                OnPropertyChanged();

                Debug.WriteLine(_triangle.Fill);
            }
        }

        public double X
        {
            get { return _canvasItem.X; }
            set
            {
                if (value == _canvasItem.X) { return; }
                _canvasItem.X = value;
                OnPropertyChanged();

                Debug.WriteLine(_canvasItem.X);
            }
        }

        public double Y
        {
            get { return _canvasItem.Y; }
            set
            {
                if (value == _canvasItem.Y) { return; }
                _canvasItem.Y = value;
                OnPropertyChanged();

                Debug.WriteLine(_canvasItem.Y);
            }
        }
    }
}
