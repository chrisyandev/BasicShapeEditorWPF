using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HierarchyTreeAndCanvasWPF.Adorners;
using HierarchyTreeAndCanvasWPF.Extensions;
using HierarchyTreeAndCanvasWPF.Models;
using HierarchyTreeAndCanvasWPF.ViewModels;

namespace HierarchyTreeAndCanvasWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Canvas _mainCanvas;
        private AdornerLayer _mainCanvasAdornerLayer;
        private MainWindowViewModel _vm;
        private Rectangle _multiSelectionRect;

        public MainWindow()
        {
            InitializeComponent();

            _vm = DataContext as MainWindowViewModel;
        }

        private void Canvas_Initialized(object sender, EventArgs e)
        {
            _mainCanvas = sender as Canvas;
            _mainCanvasAdornerLayer = AdornerLayer.GetAdornerLayer(_mainCanvas);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Canvas)
            {
                DeselectAllShapes();
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;

            Shape shape = _vm.AddShape(canvas);

            if (shape != null)
            {
                SetupShapeEventHandlers(shape);
            }
        }

        private void Canvas_DragOver(object sender, DragEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            Dictionary<string, object> data = (Dictionary<string, object>)e.Data.GetData(typeof(Dictionary<string, object>));

            if (data["shape"] is Shape shape
                && data["prevCursorPoint"] is Point prevCursorPoint)
            {
                Point currCursorPoint = e.GetPosition(canvas);
                double shapeLeft = Canvas.GetLeft(shape);
                double shapeTop = Canvas.GetTop(shape);
                double horizontalChange = currCursorPoint.X - prevCursorPoint.X;
                double verticalChange = currCursorPoint.Y - prevCursorPoint.Y;

                double moveX = horizontalChange;
                double moveY = verticalChange;

                // canvas left side is limit
                if (shapeLeft + horizontalChange < 0)
                {
                    moveX = 0;
                }
                // canvas top side is limit
                if (shapeTop + verticalChange < 0)
                {
                    moveY = 0;
                }
                // canvas right side is limit
                if (shapeLeft + shape.ActualWidth + horizontalChange > canvas.ActualWidth)
                {
                    moveX = 0;
                }
                // canvas bottom side is limit
                if (shapeTop + shape.ActualHeight + verticalChange > canvas.ActualHeight)
                {
                    moveY = 0;
                }

                MoveSelectedShapes(moveX, moveY);

                data["prevCursorPoint"] = currCursorPoint;
                e.Data.SetData(data);
            }
        }

        private void SetupShapeEventHandlers(Shape shape)
        {
            shape.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.None)
                {
                    Debug.WriteLine("drag start");

                    // if this shape was not selected before dragging, only drag this shape
                    if (!_vm.SelectedCanvasShapes.Contains(shape))
                    {
                        DeselectAllShapes();
                        AddShapeToSelection(shape);
                    }

                    Point cursorStartPoint = Mouse.GetPosition(_mainCanvas);

                    DataObject data = new DataObject(
                        new Dictionary<string, object>() {
                            { "shape", shape },
                            { "prevCursorPoint", cursorStartPoint }
                        });
                    DragDrop.DoDragDrop(shape, data, DragDropEffects.Move);
                }
            };

            shape.MouseLeftButtonUp += (s, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    AddShapeToSelection(shape);
                    AdjustSelectionBounds(shape);
                }
                else
                {
                    DeselectAllShapes();
                    AddShapeToSelection(shape);
                }
            };
        }

        private void MoveSelectedShapes(double unitsX, double unitsY)
        {
            foreach (Shape shape in _vm.SelectedCanvasShapes)
            {
                Canvas.SetLeft(shape, Canvas.GetLeft(shape) + unitsX);
                Canvas.SetTop(shape, Canvas.GetTop(shape) + unitsY);
            }
        }

        private void AddShapeToSelection(Shape shape)
        {
            Adorner[] adorners = _mainCanvasAdornerLayer.GetAdorners(shape);

            // we rely on this check to determine if SelectedCanvasShapes already contains shape
            if (adorners == null)
            {
                _mainCanvasAdornerLayer.Add(new ResizeAdorner(shape, _mainCanvas));
                _vm.SelectedCanvasShapes.Add(shape);

                Debug.WriteLine($"selected {shape}");
            }
        }

        private void AdjustSelectionBounds(Shape addedShape)
        {
            double shapeWidth = addedShape.DesiredSize.Width; // use DesiredSize or ActualWidth for Polygon
            double shapeHeight = addedShape.DesiredSize.Height;
            double shapeLeft = Canvas.GetLeft(addedShape);
            double shapeTop = Canvas.GetTop(addedShape);
            double shapeRight = shapeLeft + shapeWidth;
            double shapeBottom = shapeTop + shapeHeight;

            if (_multiSelectionRect == null)
            {
                _multiSelectionRect = new Rectangle
                {
                    Width = shapeWidth,
                    Height = shapeHeight
                };
                Canvas.SetLeft(_multiSelectionRect, shapeLeft);
                Canvas.SetTop(_multiSelectionRect, shapeTop);

                _vm.CanvasShapes.Add(_multiSelectionRect);
                _mainCanvasAdornerLayer.Add(new MultiResizeAdorner(_multiSelectionRect, _vm.SelectedCanvasShapes, _mainCanvas));
            }
            else
            {
                Rect currentBounds = new Rect(
                    Canvas.GetLeft(_multiSelectionRect), Canvas.GetTop(_multiSelectionRect),
                    _multiSelectionRect.Width, _multiSelectionRect.Height);
                Rect newBounds = currentBounds;

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

                Canvas.SetLeft(_multiSelectionRect, newBounds.Left);
                Canvas.SetTop(_multiSelectionRect, newBounds.Top);
                _multiSelectionRect.Width = newBounds.Width;
                _multiSelectionRect.Height = newBounds.Height;
            }
        }

        private void DeselectAllShapes()
        {
            List<Shape> deselectShapes = new();

            foreach (Shape s in _vm.SelectedCanvasShapes)
            {
                Adorner[] shapeAdorners = _mainCanvasAdornerLayer.GetAdorners(s);

                if (shapeAdorners != null && shapeAdorners.Length > 0)
                {
                    _mainCanvasAdornerLayer.Remove(shapeAdorners[0]);
                    deselectShapes.Add(s);
                }
            }

            foreach (Shape s in deselectShapes)
            {
                _vm.SelectedCanvasShapes.Remove(s);
            }
        }

    }
}
