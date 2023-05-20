using BasicShapeEditorWPF.Adorners;
using BasicShapeEditorWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BasicShapeEditorWPF.Services
{
    public static class MultiSelectionRectangle
    {
        static MultiSelectionRectangle()
        {
            ActualRectangle = new Rectangle
            {
                Width = 0,
                Height = 0,
                Fill = Brushes.Transparent
            };
        }

        public static Rectangle ActualRectangle { get; }

        public static bool IsShowing { get; set; } = false;

        public static void Show(Canvas canvas, IShapeCanvasViewModel _vm, Shape basedOnShape)
        {
            Debug.WriteLine("MultiSelectionRectangle: Show()");

            IsShowing = true;

            AdornerLayer.GetAdornerLayer(canvas).Add(CreateMultiResizeAdorner(
                _vm.CanvasShapes, _vm.SelectedCanvasShapes, canvas, basedOnShape));
        }

        public static void Hide(Canvas canvas, IShapeCanvasViewModel _vm)
        {
            Debug.WriteLine("MultiSelectionRectangle: Hide()");

            IsShowing = false;

            RemoveMultiResizeAdorner(AdornerLayer.GetAdornerLayer(canvas), _vm.CanvasShapes);
        }

        private static MultiResizeAdorner CreateMultiResizeAdorner(
            ObservableCollection<Shape> canvasShapes,
            ObservableCollection<Shape> selectedCanvasShapes,
            Canvas canvas,
            Shape basedOnShape)
        {
            Debug.WriteLine($"CreateMultiResizeAdorner");

            ActualRectangle.Width = basedOnShape.DesiredSize.Width;
            ActualRectangle.Height = basedOnShape.DesiredSize.Height;

            Canvas.SetLeft(ActualRectangle, Canvas.GetLeft(basedOnShape));
            Canvas.SetTop(ActualRectangle, Canvas.GetTop(basedOnShape));

            canvasShapes.Insert(0, ActualRectangle); // important, so selection rect will be at the bottom

            return new MultiResizeAdorner(ActualRectangle, selectedCanvasShapes, canvas);
        }

        private static void RemoveMultiResizeAdorner(
            AdornerLayer adornerLayer,
            ObservableCollection<Shape> canvasShapes)
        {
            Debug.WriteLine($"RemoveMultiResizeAdorner");

            Adorner[] adorners = adornerLayer.GetAdorners(ActualRectangle);

            if (adorners != null)
            {
                foreach (Adorner adorner in adorners)
                {
                    if (adorner is MultiResizeAdorner mra)
                    {
                        mra.Dispose();
                        adornerLayer.Remove(mra);
                        canvasShapes.Remove(ActualRectangle);
                        break;
                    }
                }
            }
        }
    }
}
