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

namespace HierarchyTreeAndCanvasWPF.Services
{
    public class RubberbandRectangle
    {
        private Point _initPos;
        private ShapeCanvas _canvas;
        private Shape _shapePreview;
        private IShapeCanvasViewModel _vm;
        private Rectangle _visualRect;

        public RubberbandRectangle(ShapeCanvas canvas, Shape shapePreview = null)
        {
            _canvas = canvas;
            _shapePreview = shapePreview;
            _vm = canvas.DataContext as IShapeCanvasViewModel;

            _initPos = Mouse.GetPosition(_canvas);

            _visualRect = new Rectangle
            {
                Width = 0,
                Height = 0,
                MinWidth = 0,
                MinHeight = 0,
                Fill = Brushes.Black,
                Opacity = 0.5
            };
            Canvas.SetLeft(_visualRect, _initPos.X);
            Canvas.SetTop(_visualRect, _initPos.Y);
            _vm.CanvasShapes.Add(_visualRect);
        }

        public Rectangle GetRectangle()
        {
            return _visualRect;
        }

        /// <summary>
        /// Updates rubberband rectangle size and preview shape if exists.
        /// Handles dragging in the negative direction so width and height will never be negative.
        /// </summary>
        public void Update()
        {
            Point newPos = Mouse.GetPosition(_canvas);
            double newWidth = Math.Abs(newPos.X - _initPos.X);
            double newHeight = Math.Abs(newPos.Y - _initPos.Y);

            if (newPos.X < _initPos.X)
            {
                _visualRect.ShiftLeftSide(_visualRect.DesiredSize.Width - newWidth, 0);
                if (_shapePreview != null)
                {
                    _shapePreview.ShiftLeftSide(_shapePreview.DesiredSize.Width - newWidth, 0);
                }
            }
            else if (newPos.X > _initPos.X)
            {
                _visualRect.ShiftRightSide(newWidth - _visualRect.DesiredSize.Width, _canvas.ActualWidth);
                if (_shapePreview != null)
                {
                    _shapePreview.ShiftRightSide(newWidth - _shapePreview.DesiredSize.Width, _canvas.ActualWidth);
                }
            }

            if (newPos.Y < _initPos.Y)
            {
                _visualRect.ShiftTopSide(_visualRect.DesiredSize.Height - newHeight, 0);
                if (_shapePreview != null)
                {
                    _shapePreview.ShiftTopSide(_shapePreview.DesiredSize.Height - newHeight, 0);
                }
            }
            else if (newPos.Y > _initPos.Y)
            {
                _visualRect.ShiftBottomSide(newHeight - _visualRect.DesiredSize.Height, _canvas.ActualHeight);
                if (_shapePreview != null)
                {
                    _shapePreview.ShiftBottomSide(newHeight - _shapePreview.DesiredSize.Height, _canvas.ActualHeight);
                }
            }
        }

        public void DragStop()
        {
            if (_shapePreview == null)
            {
                SelectShapesWithin();
            }
            _vm.CanvasShapes.Remove(_visualRect);
        }

        private void SelectShapesWithin()
        {
            Debug.WriteLine("SelectShapesWithin");

            Rect selectingRect = new(Canvas.GetLeft(_visualRect), Canvas.GetTop(_visualRect),
                _visualRect.DesiredSize.Width, _visualRect.DesiredSize.Height);

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
                _canvas.SelectAdditionalAndRaiseEvent(shape);
            }
        }
    }
}
