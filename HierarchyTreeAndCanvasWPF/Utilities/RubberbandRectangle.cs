using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using HierarchyTreeAndCanvasWPF.Extensions;
using System.Diagnostics;
using HierarchyTreeAndCanvasWPF.ViewModels;
using HierarchyTreeAndCanvasWPF.Controls;

namespace HierarchyTreeAndCanvasWPF.Utilities
{
    public class RubberbandRectangle
    {
        private Point _initPos;
        private ShapeCanvas _canvas;
        private IShapeCanvasViewModel _vm;
        private Rectangle _visualRect;

        public RubberbandRectangle(ShapeCanvas canvas)
        {
            _canvas = canvas;
            _vm = canvas.DataContext as IShapeCanvasViewModel;

            _initPos = Mouse.GetPosition(_canvas);

            _visualRect = new Rectangle
            {
                Width = 5,
                Height = 5,
                MinWidth = 0,
                MinHeight = 0,
                Fill = Brushes.Black,
                Opacity = 0.5
            };

            Canvas.SetLeft(_visualRect, _initPos.X);
            Canvas.SetTop(_visualRect, _initPos.Y);
        }

        public Rectangle GetRectangle()
        {
            return _visualRect;
        }

        public void Update()
        {
            Point newPos = Mouse.GetPosition(_canvas);
            double newWidth = Math.Abs(newPos.X - _initPos.X);
            double newHeight = Math.Abs(newPos.Y - _initPos.Y);

            if (newPos.X < _initPos.X)
            {
                _visualRect.ShiftLeftSide(_visualRect.Width - newWidth, 0);
            }
            else if (newPos.X > _initPos.X)
            {
                _visualRect.ShiftRightSide(newWidth - _visualRect.Width, _canvas.ActualWidth);
            }

            if (newPos.Y < _initPos.Y)
            {
                _visualRect.ShiftTopSide(_visualRect.Height - newHeight, 0);
            }
            else if (newPos.Y > _initPos.Y)
            {
                _visualRect.ShiftBottomSide(newHeight - _visualRect.Height, _canvas.ActualHeight);
            }
        }
        
        public void SelectShapesWithin()
        {
            Debug.WriteLine("SelectShapesWithin");

            Rect selectingRect = new(Canvas.GetLeft(_visualRect), Canvas.GetTop(_visualRect),
                _visualRect.Width, _visualRect.Height);
            
            List<Shape> shapesToAdd = new();

            foreach (Shape shape in _vm.CanvasShapes)
            {
                if (shape == _visualRect)
                {
                    continue;
                }

                Rect shapeRect = new(Canvas.GetLeft(shape), Canvas.GetTop(shape),
                    shape.DesiredSize.Width, shape.DesiredSize.Height);

                if (selectingRect.Contains(shapeRect))
                {
                    shapesToAdd.Add(shape);
                }
            }

            foreach (Shape shape in shapesToAdd)
            {
                _canvas.AddShapeToSelection(shape);
            }
        }
    }
}
