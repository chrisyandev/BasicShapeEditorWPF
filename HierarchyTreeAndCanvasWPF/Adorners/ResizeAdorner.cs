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
        private Thumb _thumb1, _thumb2;

        public ResizeAdorner(UIElement adornedElement, Canvas canvas) : base(adornedElement)
        {
            _canvas = canvas;

            _adornerVisuals = new VisualCollection(this);

            _thumb1 = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };
            _thumb2 = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };

            _thumb1.DragDelta += Thumb1_DragDelta;
            _thumb2.DragDelta += Thumb2_DragDelta;

            _adornerVisuals.Add(_thumb1);
            _adornerVisuals.Add(_thumb2);
        }

        private void Thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;
            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);
            double right = left + element.Width;
            double bottom = top + element.Height;

            // Dragging really quickly past the limits resulted in
            // resize being a few pixels off. Set to 0 to see what happens.
            double buffer = 10;

            double newWidth = element.Width;
            double newLeft = left;
            double newHeight = element.Height;
            double newTop = top;

            // shape right edge is the limit
            if (left + e.HorizontalChange <= right + buffer)
            {
                newWidth = element.Width - e.HorizontalChange;
                newLeft = left + e.HorizontalChange;

                // For fixing imprecise resize during fast dragging
                if (newWidth < 0)
                {
                    double unitsOverLimit = 0 - newWidth;
                    newWidth += unitsOverLimit;
                    newLeft -= unitsOverLimit;
                }
            }

            // shape bottom edge is the limit
            if (top + e.VerticalChange <= bottom + buffer)
            {
                newHeight = element.Height - e.VerticalChange;
                newTop = top + e.VerticalChange;

                // For fixing imprecise resize during fast dragging
                if (newHeight < 0)
                {
                    double unitsOverLimit = 0 - newHeight;
                    newHeight += unitsOverLimit;
                    newTop -= unitsOverLimit;
                }
            }

            // canvas left side is the limit
            if (newLeft < 0)
            {
                double unitsOverLimit = 0 - newLeft;
                newWidth -= unitsOverLimit;
                newLeft += unitsOverLimit;
            }

            // canvas top side is the limit
            if (newTop < 0)
            {
                double unitsOverLimit = 0 - newTop;
                newHeight -= unitsOverLimit;
                newTop += unitsOverLimit;
            }

            element.Width = newWidth;
            Canvas.SetLeft(element, newLeft);
            element.Height = newHeight;
            Canvas.SetTop(element, newTop);
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;

            if (element is Polygon)
            {
                element.Resize(e.HorizontalChange, e.VerticalChange);
                return;
            }

            double left = Canvas.GetLeft(element);
            double top = Canvas.GetTop(element);

            // Dragging really quickly past the limits resulted in
            // resize being a few pixels off. Set to 0 to see what happens.
            double buffer = 10;

            double newWidth = element.Width;
            double newHeight = element.Height;

            // shape left edge is the limit
            if (element.Width + e.HorizontalChange >= 0 - buffer)
            {
                newWidth = element.Width + e.HorizontalChange;

                // For fixing imprecise resize during fast dragging
                if (newWidth < 0)
                {
                    double unitsOverLimit = 0 - newWidth;
                    newWidth += unitsOverLimit;
                }
            }

            // shape top edge is the limit
            if (element.Height + e.VerticalChange >= 0 - buffer)
            {
                newHeight = element.Height + e.VerticalChange;

                // For fixing imprecise resize during fast dragging
                if (newHeight < 0)
                {
                    double unitsOverLimit = 0 - newHeight;
                    newHeight += unitsOverLimit;
                }
            }

            // canvas right side is the limit
            if (left + newWidth > _canvas.ActualWidth)
            {
                double unitsOverLimit = (left + newWidth) - _canvas.ActualWidth;
                newWidth -= unitsOverLimit;
            }

            // canvas bottom side is the limit
            if (top + newHeight > _canvas.ActualHeight)
            {
                double unitsOverLimit = (top + newHeight) - _canvas.ActualHeight;
                newHeight -= unitsOverLimit;
            }

            element.Width = newWidth;
            element.Height = newHeight;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornerVisuals[index];
        }

        protected override int VisualChildrenCount => _adornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double thumbSize = 10;

            _thumb1.Arrange(new Rect(-(thumbSize / 2), -(thumbSize / 2), thumbSize, thumbSize));

            _thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width - (thumbSize / 2),
                                    AdornedElement.DesiredSize.Height - (thumbSize / 2),
                                    thumbSize, thumbSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
