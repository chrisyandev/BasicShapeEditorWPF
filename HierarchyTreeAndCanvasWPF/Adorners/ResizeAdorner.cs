using HierarchyTreeAndCanvasWPF.Extensions;
using System;
using System.Collections.Generic;
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

namespace HierarchyTreeAndCanvasWPF.Adorners
{
    public class ResizeAdorner : Adorner
    {
        private Canvas _canvas;
        private VisualCollection _adornerVisuals;
        private Thumb _topLeftThumb, _topRightThumb, _bottomLeftThumb, _bottomRightThumb;

        public ResizeAdorner(UIElement adornedElement, Canvas canvas) : base(adornedElement)
        {
            _canvas = canvas;

            _adornerVisuals = new VisualCollection(this);

            _topLeftThumb = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };
            _topRightThumb = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };
            _bottomLeftThumb = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };
            _bottomRightThumb = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
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

        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is Shape shape)
            {
                shape.ShiftLeftSide(e.HorizontalChange, 0);
                shape.ShiftTopSide(e.VerticalChange, 0);
            }
        }

        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is Shape shape)
            {
                shape.ShiftRightSide(e.HorizontalChange, _canvas.ActualWidth);
                shape.ShiftTopSide(e.VerticalChange, 0);
            }
        }

        private void BottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is Shape shape)
            {
                shape.ShiftLeftSide(e.HorizontalChange, 0);
                shape.ShiftBottomSide(e.VerticalChange, _canvas.ActualHeight);
            }
        }

        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (AdornedElement is Shape shape)
            {
                shape.ShiftRightSide(e.HorizontalChange, _canvas.ActualWidth);
                shape.ShiftBottomSide(e.VerticalChange, _canvas.ActualHeight);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornerVisuals[index];
        }

        protected override int VisualChildrenCount => _adornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double thumbSize = 10;

            _topLeftThumb.Arrange(new Rect(-(thumbSize / 2), -(thumbSize / 2), thumbSize, thumbSize));

            _topRightThumb.Arrange(new Rect(AdornedElement.DesiredSize.Width - (thumbSize / 2),
                                    -(thumbSize / 2), thumbSize, thumbSize));

            _bottomLeftThumb.Arrange(new Rect(-(thumbSize / 2),
                                        AdornedElement.DesiredSize.Height - (thumbSize / 2),
                                        thumbSize, thumbSize));

            _bottomRightThumb.Arrange(new Rect(AdornedElement.DesiredSize.Width - (thumbSize / 2),
                                        AdornedElement.DesiredSize.Height - (thumbSize / 2),
                                        thumbSize, thumbSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
