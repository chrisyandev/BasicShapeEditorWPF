using HierarchyTreeAndCanvasWPF.Adorners;
using HierarchyTreeAndCanvasWPF.Controls.CustomEventArgs;
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

        public ShapeCanvas()
        {
            Initialized += Canvas_Initialized;
            MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            MouseRightButtonDown += Canvas_MouseRightButtonDown;
            MouseRightButtonUp += Canvas_MouseRightButtonUp;
            MouseMove += Canvas_MouseMove;
            DragOver += Canvas_DragOver;
        }

        public event EventHandler<ShapeStateChangedEventArgs> ShapeStateChanged;

        public void OnShapeStateChanged(object sender, ShapeStateChangedEventArgs e)
        {
            if (e.Selected)
            {
                if (e.SelectionType == SelectionType.Additional)
                {
                    SelectAdditional(e.Shape);
                }
                else if (e.SelectionType == SelectionType.Only)
                {
                    SelectOnly(e.Shape);
                }
            }
            else if (e.Removed)
            {
                RemoveShape(e.Shape);
            }
            else
            {
                DeselectShape(e.Shape);
            }
        }

        public void SelectOnly(Shape shape)
        {
            DeselectAllShapes();
            AddShapeToSelection(shape);
        }

        public void SelectAdditional(Shape shape)
        {
            AddShapeToSelection(shape);
        }

        public void SelectOnlyAndRaiseEvent(Shape shape)
        {
            SelectOnly(shape);
            ShapeStateChanged(this, new ShapeStateChangedEventArgs(shape, true, SelectionType.Only));
        }

        public void SelectAdditionalAndRaiseEvent(Shape shape)
        {
            SelectAdditional(shape);
            ShapeStateChanged(this, new ShapeStateChangedEventArgs(shape, true, SelectionType.Additional));
        }

        public void SelectAllShapes()
        {
            for (int i = 0; i < _vm.CanvasShapes.Count; i++)
            {
                SelectAdditionalAndRaiseEvent(_vm.CanvasShapes[i]);
            }
        }

        public void DeselectAllShapes()
        {
            Debug.WriteLine($"Canvas: DeselectAllShapes");

            for (int i = _vm.SelectedCanvasShapes.Count - 1; i >= 0; i--)
            {
                DeselectShapeAndRaiseEvent(_vm.SelectedCanvasShapes[i]);
            }
        }

        public void DeselectShape(Shape shape)
        {
            Debug.WriteLine($"Canvas: deselecting {shape}");

            _vm.SelectedCanvasShapes.Remove(shape);

            if (_vm.SelectedCanvasShapes.Count == 0 && _multiSelectionRect != null)
            {
                RemoveMultiResizeAdorner(ref _multiSelectionRect,
                    _shapeCanvasAdornerLayer, _vm.CanvasShapes);

                Debug.WriteLine($"Canvas: deselected all shapes");
            }
        }

        public void DeselectShapeAndRaiseEvent(Shape shape)
        {
            DeselectShape(shape);
            ShapeStateChanged(this, new ShapeStateChangedEventArgs(shape, false));
        }

        public void RemoveSelectedShapes()
        {
            Debug.WriteLine($"Canvas: RemoveSelectedShapes");

            for (int i = _vm.SelectedCanvasShapes.Count - 1; i >= 0; i--)
            {
                RemoveShapeAndRaiseEvent(_vm.SelectedCanvasShapes[i]);
            }
        }

        public void RemoveShape(Shape shape)
        {
            DeselectShape(shape);
            _vm.CanvasShapes.Remove(shape);
        }

        public void RemoveShapeAndRaiseEvent(Shape shape)
        {
            RemoveShape(shape);
            ShapeStateChanged(this, new ShapeStateChangedEventArgs(shape, false, removed: true));
        }

        private void AddShapeToSelection(Shape shape)
        {
            Debug.WriteLine($"AddShapeToSelection");

            // if null, adorner layer's adorners is also null, meaning nothing is selected
            if (_multiSelectionRect == null)
            {
                _shapeCanvasAdornerLayer.Add(CreateMultiResizeAdorner(
                    ref _multiSelectionRect, _vm.CanvasShapes,
                    _vm.SelectedCanvasShapes, this, shape));
            }

            if (!_vm.SelectedCanvasShapes.Contains(shape))
            {
                _vm.SelectedCanvasShapes.Add(shape);
            }
            
            Debug.WriteLine($"selected {shape}");
        }

        private void Canvas_Initialized(object sender, EventArgs e)
        {
            _vm = (IShapeCanvasViewModel)DataContext;
            _shapeCanvasAdornerLayer = AdornerLayer.GetAdornerLayer(this);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // not null means rubberband rectangle is being dragged
            if (_rubberbandRect != null)
            {
                Mouse.Capture(null);
                DeselectAllShapes();
                _rubberbandRect.DragStop();
                _rubberbandRect = null;
            }
            else if (e.Source is Canvas)
            {
                DeselectAllShapes();
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_rubberbandRect == null)
                {
                    if (_vm.ActiveTool == "rectangle" || _vm.ActiveTool == "ellipse" || _vm.ActiveTool == "triangle")
                    {
                        Shape shape = _vm.AddShapeToCanvas(_vm.ActiveTool, this);

                        if (shape != null)
                        {
                            SetupShapeEventHandlers(shape);
                        }

                        _rubberbandRect = new RubberbandRectangle(this, shape);
                    }
                    else
                    {
                        _rubberbandRect = new RubberbandRectangle(this);
                    }
                }

                Mouse.Capture(this);
                _rubberbandRect.Update();
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
                double selectionLeft = Canvas.GetLeft(_multiSelectionRect);
                double selectionTop = Canvas.GetTop(_multiSelectionRect);
                double selectionRight = selectionLeft + _multiSelectionRect.Width;
                double selectionBottom = selectionTop + _multiSelectionRect.Height;
                double horizontalChange = currCursorPoint.X - prevCursorPoint.X;
                double verticalChange = currCursorPoint.Y - prevCursorPoint.Y;

                double moveX = horizontalChange;
                double moveY = verticalChange;

                // canvas left side is limit
                if (selectionLeft + horizontalChange < 0)
                {
                    moveX = 0;
                }
                // canvas top side is limit
                if (selectionTop + verticalChange < 0)
                {
                    moveY = 0;
                }
                // canvas right side is limit
                if (selectionRight + horizontalChange > canvas.ActualWidth)
                {
                    moveX = 0;
                }
                // canvas bottom side is limit
                if (selectionBottom + verticalChange > canvas.ActualHeight)
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
                SelectAdditionalAndRaiseEvent(shape);
            }
            else
            {
                SelectOnlyAndRaiseEvent(shape);
            }
        }

        private void Shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (_rubberbandRect != null)
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
                    SelectOnlyAndRaiseEvent(shape);
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

        private MultiResizeAdorner CreateMultiResizeAdorner(
            ref Rectangle multiSelectionRect,
            ObservableCollection<Shape> canvasShapes,
            ObservableCollection<Shape> selectedCanvasShapes,
            Canvas canvas,
            Shape basedOnShape)
        {
            Debug.WriteLine($"CreateMultiResizeAdorner");

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
            Debug.WriteLine($"RemoveMultiResizeAdorner");

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
