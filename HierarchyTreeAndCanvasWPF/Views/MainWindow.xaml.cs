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
            Dictionary<string, object> data = 
                e.Data.GetData(DataFormats.Serializable) as Dictionary<string, object>;

            if (data["shape"] is Shape shape
                && data["cursorToLeftDistance"] is double cursorToLeftDistance
                && data["cursorToTopDistance"] is double cursorToTopDistance)
            {
                Point cursorPos = e.GetPosition(canvas);

                double newLeft = cursorPos.X - cursorToLeftDistance;
                double newTop = cursorPos.Y - cursorToTopDistance;

                if (newLeft + shape.ActualWidth > canvas.ActualWidth)
                {
                    newLeft = canvas.ActualWidth - shape.ActualWidth;
                }
                if (newTop + shape.ActualHeight > canvas.ActualHeight)
                {
                    newTop = canvas.ActualHeight - shape.ActualHeight;
                }

                Canvas.SetLeft(shape, newLeft < 0 ? 0 : newLeft);
                Canvas.SetTop(shape, newTop < 0 ? 0 : newTop);
            }
        }

        private void SetupShapeEventHandlers(Shape shape)
        {
            shape.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Debug.WriteLine("drag start");

                    double cursorToLeftDistance = Mouse.GetPosition(_mainCanvas).X - Canvas.GetLeft(shape);
                    double cursorToTopDistance = Mouse.GetPosition(_mainCanvas).Y - Canvas.GetTop(shape);

                    DataObject data = new DataObject(DataFormats.Serializable,
                        new Dictionary<string, object>() {
                        { "shape", shape },
                        { "cursorToLeftDistance", cursorToLeftDistance },
                        { "cursorToTopDistance", cursorToTopDistance }
                        });
                    DragDrop.DoDragDrop(shape, data, DragDropEffects.Move);
                }
            };

            shape.MouseLeftButtonUp += (s, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    AddShapeToSelection(shape);
                }
                else
                {
                    DeselectAllShapes();
                    AddShapeToSelection(shape);
                }
            };

            shape.Drop += (s, e) =>
            {
                DeselectAllShapes();
                AddShapeToSelection(shape);
            };
        }

        private void AddShapeToSelection(Shape shape)
        {
            Adorner[] adorners = _mainCanvasAdornerLayer.GetAdorners(shape);

            if (adorners == null)
            {
                _mainCanvasAdornerLayer.Add(new ResizeAdorner(shape, _mainCanvas));
                _vm.SelectedCanvasShapes.Add(shape);

                Debug.WriteLine($"selected {shape}");
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
