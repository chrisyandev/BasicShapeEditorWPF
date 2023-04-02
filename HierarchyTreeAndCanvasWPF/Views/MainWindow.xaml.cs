using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using HierarchyTreeAndCanvasWPF.Adorners;
using HierarchyTreeAndCanvasWPF.Extensions;
using HierarchyTreeAndCanvasWPF.Models;
using HierarchyTreeAndCanvasWPF.Utilities;
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
                if (e.LeftButton == MouseButtonState.Pressed
                    && Keyboard.Modifiers == ModifierKeys.None)
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
                if (Keyboard.Modifiers == ModifierKeys.Control
                    || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    AddShapeToSelection(shape);
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
            // if null, adorner layer's adorners is also null, meaning nothing is selected
            if (_multiSelectionRect == null)
            {
                _mainCanvasAdornerLayer.Add(CreateMultiResizeAdorner(
                    ref _multiSelectionRect, _vm.CanvasShapes,
                    _vm.SelectedCanvasShapes, _mainCanvas, shape));
            }
            _vm.SelectedCanvasShapes.Add(shape);
        }

        private void DeselectAllShapes()
        {
            // if not null, something is selected
            if (_multiSelectionRect != null)
            {
                RemoveMultiResizeAdorner(ref _multiSelectionRect,
                    _mainCanvasAdornerLayer, _vm.CanvasShapes);
                _vm.SelectedCanvasShapes.Clear(); // clear only after disposing MultiResizeAdorner
            }
        }

        private MultiResizeAdorner CreateMultiResizeAdorner(
            ref Rectangle multiSelectionRect,
            ObservableCollection<Shape> canvasShapes,
            ObservableCollection<Shape> selectedCanvasShapes,
            Canvas canvas,
            Shape basedOnShape)
        {
            multiSelectionRect = new Rectangle
            {
                Width = basedOnShape.DesiredSize.Width,
                Height = basedOnShape.DesiredSize.Height
            };
            Canvas.SetLeft(multiSelectionRect, Canvas.GetLeft(basedOnShape));
            Canvas.SetTop(multiSelectionRect, Canvas.GetTop(basedOnShape));

            canvasShapes.Add(multiSelectionRect); // important

            return new MultiResizeAdorner(multiSelectionRect, selectedCanvasShapes, canvas);
        }

        private void RemoveMultiResizeAdorner(
            ref Rectangle multiSelectionRect,
            AdornerLayer adornerLayer,
            ObservableCollection<Shape> canvasShapes)
        {
            Adorner[] adorners = adornerLayer.GetAdorners(multiSelectionRect);

            foreach (Adorner adorner in adorners)
            {
                if (adorner is MultiResizeAdorner mra)
                {
                    mra.Dispose();
                    adornerLayer.Remove(mra);
                    canvasShapes.Remove(multiSelectionRect);
                    multiSelectionRect = null;
                    break;
                }
            }
        }
    }
}
