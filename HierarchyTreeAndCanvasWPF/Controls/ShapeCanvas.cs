using HierarchyTreeAndCanvasWPF.Adorners;
using HierarchyTreeAndCanvasWPF.Utilities;
using HierarchyTreeAndCanvasWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class ShapeCanvas : Canvas
    {
        private AdornerLayer _shapeCanvasAdornerLayer;
        private Rectangle _multiSelectionRect;
        private IShapeCanvasViewModel _vm;
        RubberbandRectangle _rubberbandRect;
        private bool _isDraggingRubberbandRect = false;

        static ShapeCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShapeCanvas), new FrameworkPropertyMetadata(typeof(ShapeCanvas)));
        }

        public ShapeCanvas()
        {
            _vm = DataContext as IShapeCanvasViewModel;

            Initialized += Canvas_Initialized;
            DragOver += Canvas_DragOver;
            MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            MouseRightButtonUp += Canvas_MouseRightButtonUp;
            MouseRightButtonDown += Canvas_MouseRightButtonDown;
            MouseMove += Canvas_MouseMove;
        }

        private void Canvas_Initialized(object sender, EventArgs e)
        {
            _vm = (IShapeCanvasViewModel)DataContext;
            _shapeCanvasAdornerLayer = AdornerLayer.GetAdornerLayer(this);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDraggingRubberbandRect)
            {
                _isDraggingRubberbandRect = false;
                DeselectAllShapes();
                _rubberbandRect.SelectShapesWithin();
                _vm.CanvasShapes.Remove(_rubberbandRect.GetRectangle());
                _rubberbandRect = null;
            }

            if (e.Source is Canvas)
            {
                DeselectAllShapes();
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;

            Shape shape = _vm.AddShapeToCanvas(_vm.ShapeToAdd, canvas);

            if (shape != null)
            {
                SetupShapeEventHandlers(shape);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDraggingRubberbandRect = true;

                if (_rubberbandRect == null)
                {
                    _rubberbandRect = new RubberbandRectangle(this);
                    _vm.CanvasShapes.Add(_rubberbandRect.GetRectangle());
                }

                _rubberbandRect.Update();
            }
        }

        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

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
            shape.MouseLeftButtonUp += Shape_MouseLeftButtonUp;
            shape.MouseMove += Shape_MouseMove;
        }

        private void Shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Shape shape = sender as Shape;

            Debug.WriteLine($"clicked {shape}");

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
        }

        private void Shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingRubberbandRect)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed
                && Keyboard.Modifiers == ModifierKeys.None)
            {
                Debug.WriteLine("drag start");

                Shape shape = sender as Shape;

                // if this shape was not selected before dragging, only drag this shape
                if (shape != _multiSelectionRect && !_vm.SelectedCanvasShapes.Contains(shape))
                {
                    DeselectAllShapes();
                    AddShapeToSelection(shape);
                }

                Point cursorStartPoint = Mouse.GetPosition(this);

                DataObject data = new DataObject(
                    new Dictionary<string, object>() {
                            { "shape", shape },
                            { "prevCursorPoint", cursorStartPoint }
                    });
                DragDrop.DoDragDrop(shape, data, DragDropEffects.Move);
            }
        }

        private void MoveSelectedShapes(double unitsX, double unitsY)
        {
            Canvas.SetLeft(_multiSelectionRect, Canvas.GetLeft(_multiSelectionRect) + unitsX);
            Canvas.SetTop(_multiSelectionRect, Canvas.GetTop(_multiSelectionRect) + unitsY);

            foreach (Shape shape in _vm.SelectedCanvasShapes)
            {
                Canvas.SetLeft(shape, Canvas.GetLeft(shape) + unitsX);
                Canvas.SetTop(shape, Canvas.GetTop(shape) + unitsY);
            }
        }

        public void AddShapeToSelection(Shape shape)
        {
            Debug.WriteLine($"selected {shape}");

            // if null, adorner layer's adorners is also null, meaning nothing is selected
            if (_multiSelectionRect == null)
            {
                _shapeCanvasAdornerLayer.Add(CreateMultiResizeAdorner(
                    ref _multiSelectionRect, _vm.CanvasShapes,
                    _vm.SelectedCanvasShapes, this, shape));
            }
            _vm.SelectedCanvasShapes.Add(shape);
        }

        public void DeselectAllShapes()
        {
            // if not null, something is selected
            if (_multiSelectionRect != null)
            {
                RemoveMultiResizeAdorner(ref _multiSelectionRect,
                    _shapeCanvasAdornerLayer, _vm.CanvasShapes);
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
                Height = basedOnShape.DesiredSize.Height,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(multiSelectionRect, Canvas.GetLeft(basedOnShape));
            Canvas.SetTop(multiSelectionRect, Canvas.GetTop(basedOnShape));

            multiSelectionRect.MouseMove += Shape_MouseMove;
            canvasShapes.Insert(0, multiSelectionRect); // important, so selection rect will be at the bottom

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
