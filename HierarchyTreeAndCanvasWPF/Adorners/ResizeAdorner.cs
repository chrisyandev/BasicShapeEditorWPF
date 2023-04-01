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
        private static readonly Brush ThumbBrush = Brushes.SlateBlue;
        private static readonly Brush VisualRectBrush = Brushes.White;
        private const double ThumbSize = 10;
        private const double VisualRectStrokeThickness = 1.5;

        private Canvas _canvas;
        private VisualCollection _adornerVisuals;
        private Rectangle _visualRect;
        private Thumb _topLeftThumb, _topRightThumb, _bottomLeftThumb, _bottomRightThumb;

        public ResizeAdorner(UIElement adornedElement, Canvas canvas) : base(adornedElement)
        {
            _canvas = canvas;
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
    }
}
