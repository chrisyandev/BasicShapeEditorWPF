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

namespace HierarchyTreeAndCanvasWPF.Adorners
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisuals;
        Thumb thumb1, thumb2;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);

            thumb1 = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };
            thumb2 = new Thumb()
            {
                Background = Brushes.Coral,
                Height = 10,
                Width = 10
            };

            thumb1.DragDelta += Thumb1_DragDelta;
            thumb2.DragDelta += Thumb2_DragDelta;

            AdornerVisuals.Add(thumb1);
            AdornerVisuals.Add(thumb2);
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

            // right edge is the limit
            if (left + e.HorizontalChange <= right + buffer)
            {
                double newWidth = element.Width - e.HorizontalChange;
                double newLeft = left + e.HorizontalChange;

                // For fixing imprecise resize during fast dragging
                if (newWidth < 0)
                {
                    double unitsOverLimit = 0 - newWidth;
                    newWidth += unitsOverLimit;
                    newLeft -= unitsOverLimit;
                }

                element.Width = newWidth;
                Canvas.SetLeft(element, newLeft);
            }

            // bottom edge is the limit
            if (top + e.VerticalChange <= bottom + buffer)
            {
                double newHeight = element.Height - e.VerticalChange;
                double newTop = top + e.VerticalChange;

                // For fixing imprecise resize during fast dragging
                if (newHeight < 0)
                {
                    double unitsOverLimit = 0 - newHeight;
                    newHeight += unitsOverLimit;
                    newTop -= unitsOverLimit;
                }

                element.Height = newHeight;
                Canvas.SetTop(element, newTop);
            }
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)AdornedElement;

            double newHeight = element.Height + e.VerticalChange;
            double newWidth = element.Width + e.HorizontalChange;

            element.Height = newHeight < 0 ? 0 : newHeight;
            element.Width = newWidth < 0 ? 0 : newWidth;
        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            double thumbSize = 10;

            thumb1.Arrange(new Rect(-(thumbSize / 2), -(thumbSize / 2), thumbSize, thumbSize));

            thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width - (thumbSize / 2),
                                    AdornedElement.DesiredSize.Height - (thumbSize / 2),
                                    thumbSize, thumbSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
