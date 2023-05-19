using HierarchyTreeAndCanvasWPF.Extensions;
using HierarchyTreeAndCanvasWPF.Services;
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

            ShapeEventMessenger.Subscribe(ShapeEventMessenger.SelectOnly, (shape, item) => SelectOnly(shape));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.SelectAdditional, (shape, item) => SelectAdditional(shape));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.Deselect, (shape, item) => DeselectShape(shape));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.Remove, (shape, item) => RemoveShape(shape));

            MultiSelectionRectangle.ActualRectangle.MouseMove += Shape_MouseMove;
        }

        private void SelectOnly(Shape shape)
        {
            DeselectAllShapesExcept(shape);
            AddShapeToSelection(shape);
        }

        private void SelectAdditional(Shape shape)
        {
            AddShapeToSelection(shape);
        }

        private void SelectOnlyAndRaiseEvent(Shape shape)
        {
            SelectOnly(shape);
            ShapeEventMessenger.Publish(ShapeEventMessenger.SelectOnly, shape.GetId());
        }

        public void SelectAdditionalAndRaiseEvent(Shape shape)
        {
            SelectAdditional(shape);
            ShapeEventMessenger.Publish(ShapeEventMessenger.SelectAdditional, shape.GetId());
        }

        public void SelectAllShapes()
        {
            for (int i = 0; i < _vm.CanvasShapes.Count; i++)
            {
                SelectAdditionalAndRaiseEvent(_vm.CanvasShapes[i]);
            }
        }

        private void DeselectAllShapes()
        {
            Debug.WriteLine($"Canvas: DeselectAllShapes");

            for (int i = _vm.SelectedCanvasShapes.Count - 1; i >= 0; i--)
            {
                DeselectShapeAndRaiseEvent(_vm.SelectedCanvasShapes[i]);
            }
        }

        private void DeselectAllShapesExcept(Shape shape)
        {
            for (int i = _vm.SelectedCanvasShapes.Count - 1; i >= 0; i--)
            {
                if (_vm.SelectedCanvasShapes[i] != shape)
                {
                    DeselectShapeAndRaiseEvent(_vm.SelectedCanvasShapes[i]);
                }
            }
        }

        private void DeselectShape(Shape shape)
        {
            Debug.WriteLine($"Canvas: deselecting {shape}");

            _vm.SelectedCanvasShapes.Remove(shape);

            if (_vm.SelectedCanvasShapes.Count == 0)
            {
                MultiSelectionRectangle.Hide(this, _vm);

                Debug.WriteLine($"Canvas: deselected all shapes");
            }
        }

        private void DeselectShapeAndRaiseEvent(Shape shape)
        {
            DeselectShape(shape);
            ShapeEventMessenger.Publish(ShapeEventMessenger.Deselect, shape.GetId());
        }

        public void RemoveSelectedShapes()
        {
            Debug.WriteLine($"Canvas: RemoveSelectedShapes");

            for (int i = _vm.SelectedCanvasShapes.Count - 1; i >= 0; i--)
            {
                RemoveShapeAndRaiseEvent(_vm.SelectedCanvasShapes[i]);
            }
        }

        private void RemoveShape(Shape shape)
        {
            DeselectShape(shape);
            _vm.CanvasShapes.Remove(shape);
        }

        private void RemoveShapeAndRaiseEvent(Shape shape)
        {
            RemoveShape(shape);
            ShapeEventMessenger.Publish(ShapeEventMessenger.Remove, shape.GetId());
        }

        private void AddShapeToSelection(Shape shape)
        {
            Debug.WriteLine($"AddShapeToSelection");

            if (!MultiSelectionRectangle.IsShowing)
            {
                MultiSelectionRectangle.Show(this, _vm, shape);
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
                double selectionLeft = Canvas.GetLeft(MultiSelectionRectangle.ActualRectangle);
                double selectionTop = Canvas.GetTop(MultiSelectionRectangle.ActualRectangle);
                double selectionRight = selectionLeft + MultiSelectionRectangle.ActualRectangle.Width;
                double selectionBottom = selectionTop + MultiSelectionRectangle.ActualRectangle.Height;
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
                if (shape != MultiSelectionRectangle.ActualRectangle && !_vm.SelectedCanvasShapes.Contains(shape))
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
            Canvas.SetLeft(MultiSelectionRectangle.ActualRectangle, Canvas.GetLeft(MultiSelectionRectangle.ActualRectangle) + unitsX);
            Canvas.SetTop(MultiSelectionRectangle.ActualRectangle, Canvas.GetTop(MultiSelectionRectangle.ActualRectangle) + unitsY);

            foreach (Shape shape in _vm.SelectedCanvasShapes)
            {
                Canvas.SetLeft(shape, Canvas.GetLeft(shape) + unitsX);
                Canvas.SetTop(shape, Canvas.GetTop(shape) + unitsY);
            }
        }
    }
}
