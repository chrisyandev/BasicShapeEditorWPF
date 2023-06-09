﻿using BasicShapeEditorWPF.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BasicShapeEditorWPF.Adorners
{
    public class MultiResizeAdorner : Adorner, IDisposable
    {
        private static readonly Brush ThumbBrush = Brushes.SlateBlue;
        private static readonly Brush VisualRectBrush = Brushes.White;
        private const double ThumbSize = 10;
        private const double VisualRectStrokeThickness = 1.5;

        private ObservableCollection<Shape> _selectedShapes;
        private Canvas _canvas;

        private Rectangle _multiSelectionRect;

        private VisualCollection _adornerVisuals;
        private Rectangle _visualRect;
        private Thumb _topLeftThumb, _topRightThumb, _bottomLeftThumb, _bottomRightThumb;

        public MultiResizeAdorner(UIElement adornedElement, ObservableCollection<Shape> selectedShapes, Canvas canvas) : base(adornedElement)
        {
            _selectedShapes = selectedShapes;
            _selectedShapes.CollectionChanged += SelectedShapes_CollectionChanged;
            _canvas = canvas;

            _multiSelectionRect = (Rectangle)adornedElement;
            
            _adornerVisuals = new VisualCollection(this);

            // add this first so it goes below the thumbs
            _visualRect = new Rectangle
            {
                Stroke = VisualRectBrush,
                StrokeThickness = VisualRectStrokeThickness,
                StrokeDashArray = { 2, 2 }
            };
            _adornerVisuals.Add(_visualRect);

            _topLeftThumb = new Thumb()
            {
                Background = ThumbBrush,
                Height = ThumbSize,
                Width = ThumbSize
            };
            _topRightThumb = new Thumb()
            {
                Background = ThumbBrush,
                Height = ThumbSize,
                Width = ThumbSize
            };
            _bottomLeftThumb = new Thumb()
            {
                Background = ThumbBrush,
                Height = ThumbSize,
                Width = ThumbSize
            };
            _bottomRightThumb = new Thumb()
            {
                Background = ThumbBrush,
                Height = ThumbSize,
                Width = ThumbSize
            };

            _topLeftThumb.DragDelta += TopLeftThumb_DragDelta;
            _topRightThumb.DragDelta += TopRightThumb_DragDelta;
            _bottomLeftThumb.DragDelta += BottomLeftThumb_DragDelta;
            _bottomRightThumb.DragDelta += BottomRightThumb_DragDelta;

            _adornerVisuals.Add(_topLeftThumb);
            _adornerVisuals.Add(_topRightThumb);
            _adornerVisuals.Add(_bottomLeftThumb);
            _adornerVisuals.Add(_bottomRightThumb);
        }

        private void AdjustMultiSelectionRect(Rectangle multiSelectionRect, IEnumerable<Shape> selectedShapes)
        {
            double startingPointX = selectedShapes.Min(s => Canvas.GetLeft(s));
            double startingPointY = selectedShapes.Min(s => Canvas.GetTop(s));

            Rect currentBounds = new Rect(startingPointX, startingPointY, 0, 0);
            Rect newBounds = currentBounds;

            foreach (Shape shape in selectedShapes)
            {
                double shapeWidth = shape.DesiredSize.Width; // use DesiredSize or ActualWidth for Polygon
                double shapeHeight = shape.DesiredSize.Height;
                double shapeLeft = Canvas.GetLeft(shape);
                double shapeTop = Canvas.GetTop(shape);
                double shapeRight = shapeLeft + shapeWidth;
                double shapeBottom = shapeTop + shapeHeight;

                if (shapeLeft < currentBounds.Left)
                {
                    newBounds.X = shapeLeft;
                    newBounds.Width += currentBounds.Left - shapeLeft;
                }
                if (shapeTop < currentBounds.Top)
                {
                    newBounds.Y = shapeTop;
                    newBounds.Height += currentBounds.Top - shapeTop;
                }
                if (shapeRight > currentBounds.Right)
                {
                    newBounds.Width += shapeRight - currentBounds.Right;
                }
                if (shapeBottom > currentBounds.Bottom)
                {
                    newBounds.Height += shapeBottom - currentBounds.Bottom;
                }

                currentBounds = newBounds;
            }

            Canvas.SetLeft(multiSelectionRect, newBounds.Left);
            Canvas.SetTop(multiSelectionRect, newBounds.Top);
            multiSelectionRect.Width = newBounds.Width;
            multiSelectionRect.Height = newBounds.Height;
        }

        private void CalculateMultiSelectionRectMinSize(Rectangle multiSelectionRect, IEnumerable<Shape> selectedShapes)
        {
            double smallestPossibleShiftX = double.PositiveInfinity;
            double smallestPossibleShiftY = double.PositiveInfinity;

            foreach (Shape shape in selectedShapes)
            {
                if (shape.DesiredSize.Width - shape.MinWidth < smallestPossibleShiftX)
                {
                    smallestPossibleShiftX = shape.DesiredSize.Width - shape.MinWidth;
                }
                if (shape.DesiredSize.Height - shape.MinHeight < smallestPossibleShiftY)
                {
                    smallestPossibleShiftY = shape.DesiredSize.Height - shape.MinHeight;
                }
            }

            multiSelectionRect.MinWidth = multiSelectionRect.Width - smallestPossibleShiftX;
            multiSelectionRect.MinHeight = multiSelectionRect.Height - smallestPossibleShiftY;
        }

        private void SelectedShapes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_selectedShapes.Count > 0)
            {
                AdjustMultiSelectionRect(_multiSelectionRect, _selectedShapes);
                CalculateMultiSelectionRectMinSize(_multiSelectionRect, _selectedShapes);
            }
        }

        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double shiftedX = _multiSelectionRect.ShiftLeftSide(e.HorizontalChange, 0);
            double shiftedY = _multiSelectionRect.ShiftTopSide(e.VerticalChange, 0);

            foreach (Shape shape in _selectedShapes)
            {
                shape.ShiftLeftSide(shiftedX, 0);
                shape.ShiftTopSide(shiftedY, 0);
            }
        }

        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double shiftedX = _multiSelectionRect.ShiftRightSide(e.HorizontalChange, _canvas.ActualWidth);
            double shiftedY = _multiSelectionRect.ShiftTopSide(e.VerticalChange, 0);

            foreach (Shape shape in _selectedShapes)
            {
                shape.ShiftRightSide(shiftedX, _canvas.ActualWidth);
                shape.ShiftTopSide(shiftedY, 0);
            }
        }

        private void BottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double shiftedX = _multiSelectionRect.ShiftLeftSide(e.HorizontalChange, 0);
            double shiftedY = _multiSelectionRect.ShiftBottomSide(e.VerticalChange, _canvas.ActualHeight);

            foreach (Shape shape in _selectedShapes)
            {
                shape.ShiftLeftSide(shiftedX, 0);
                shape.ShiftBottomSide(shiftedY, _canvas.ActualHeight);
            }
        }

        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double shiftedX = _multiSelectionRect.ShiftRightSide(e.HorizontalChange, _canvas.ActualWidth);
            double shiftedY = _multiSelectionRect.ShiftBottomSide(e.VerticalChange, _canvas.ActualHeight);

            foreach (Shape shape in _selectedShapes)
            {
                shape.ShiftRightSide(shiftedX, _canvas.ActualWidth);
                shape.ShiftBottomSide(shiftedY, _canvas.ActualHeight);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornerVisuals[index];
        }

        protected override int VisualChildrenCount => _adornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            // rectangle touches edges of shape
            _visualRect.Arrange(new Rect(-VisualRectStrokeThickness, -VisualRectStrokeThickness,
                                    AdornedElement.DesiredSize.Width + VisualRectStrokeThickness * 2,
                                    AdornedElement.DesiredSize.Height + VisualRectStrokeThickness * 2));

            // calculates thumb displacement so visual rect line goes through middle of thumb
            double thumbDisplacement = (ThumbSize / 2) + (VisualRectStrokeThickness / 2);

            // add stroke so thumb displacement is accurate
            double elementWidth = AdornedElement.DesiredSize.Width + VisualRectStrokeThickness;
            double elementHeight = AdornedElement.DesiredSize.Height + VisualRectStrokeThickness;

            _topLeftThumb.Arrange(new Rect(-thumbDisplacement, -thumbDisplacement,
                                    ThumbSize, ThumbSize));

            _topRightThumb.Arrange(new Rect(elementWidth - thumbDisplacement, -thumbDisplacement,
                                    ThumbSize, ThumbSize));

            _bottomLeftThumb.Arrange(new Rect(-thumbDisplacement, elementHeight - thumbDisplacement,
                                        ThumbSize, ThumbSize));

            _bottomRightThumb.Arrange(new Rect(elementWidth - thumbDisplacement, elementHeight - thumbDisplacement,
                                        ThumbSize, ThumbSize));

            return base.ArrangeOverride(finalSize);
        }

        public void Dispose()
        {
            _selectedShapes.CollectionChanged -= SelectedShapes_CollectionChanged;
        }
    }
}
