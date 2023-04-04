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

namespace HierarchyTreeAndCanvasWPF.Utilities
{
    public class RubberbandRectangle
    {
        private Point _initPos;
        private Canvas _canvas;
        private ICanvasViewModel _vm;
        private Rectangle _visualRect;

        public RubberbandRectangle(Canvas canvas)
        {
            _canvas = canvas;
            _vm = canvas.DataContext as ICanvasViewModel;

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
            Rect selectingRect = new Rect(Canvas.GetLeft(_visualRect), Canvas.GetTop(_visualRect),
                _visualRect.Width, _visualRect.Height);
            foreach (Shape shape in _vm.CanvasShapes)
            {
                if (shape == _visualRect)
                {
                    return;
                }

                Rect shapeRect = new Rect(Canvas.GetLeft(shape), Canvas.GetTop(shape),
                    shape.DesiredSize.Width, shape.DesiredSize.Height);

                if (selectingRect.Contains(shapeRect))
                {
                    _vm.SelectedCanvasShapes.Add(shape);
                }
            }
        }
    }
}
