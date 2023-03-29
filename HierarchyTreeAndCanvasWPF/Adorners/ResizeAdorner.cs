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
            if (AdornedElement is Shape shape)
            {
                shape.ShiftLeftSide(e.HorizontalChange, 0);
                shape.ShiftTopSide(e.VerticalChange, 0);
            }
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
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

            _thumb1.Arrange(new Rect(-(thumbSize / 2), -(thumbSize / 2), thumbSize, thumbSize));

            _thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width - (thumbSize / 2),
                                    AdornedElement.DesiredSize.Height - (thumbSize / 2),
                                    thumbSize, thumbSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
