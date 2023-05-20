using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using HierarchyTreeAndCanvasWPF.Commands;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HierarchyTreeAndCanvasWPF.Utilities;
using HierarchyTreeAndCanvasWPF.Controls;
using HierarchyTreeAndCanvasWPF.Extensions;
using HierarchyTreeAndCanvasWPF.Services;

namespace HierarchyTreeAndCanvasWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged, IShapeCanvasViewModel
    {
        private string _activeTool;
        private SelectionManager _selectionManager;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            TreeItems = new ObservableCollection<ShapeTreeViewItem>();
            CanvasShapes = new ObservableCollection<Shape>();
            SelectedCanvasShapes = new ObservableCollection<Shape>();

            SetActiveToolCommand = new DoActionWithParameterCommand(shapeName =>
            {
                ActiveTool = shapeName as string;
            });

            // hardcoded for now
            ShapeMinWidth = 0;
            ShapeMinHeight = 0;

            SelectionManager = new SelectionManager();
            SelectionManager.VM = this;
        }

        public ObservableCollection<ShapeTreeViewItem> TreeItems { get; set; }

        public ObservableCollection<Shape> CanvasShapes { get; set; }

        public ObservableCollection<Shape> SelectedCanvasShapes { get; set; }

        public SelectionManager SelectionManager
        {
            get
            {
                return _selectionManager;
            }
            set
            {
                if (value == _selectionManager) return;
                _selectionManager = value;
                OnPropertyChanged();

                Debug.WriteLine(_selectionManager);
            }
        }

        public string ActiveTool
        {
            get { return _activeTool; }
            set
            {
                if (value == _activeTool) { return; }
                _activeTool = value;
                OnPropertyChanged();

                Debug.WriteLine(_activeTool);
            }
        }

        public double ShapeMinWidth { get; }

        public double ShapeMinHeight { get; }

        public ICommand SetActiveToolCommand { get; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Shape AddShapeToCanvas(string shapeName, Canvas canvas)
        {
            double mousePosX = Mouse.GetPosition(canvas).X;
            double mousePosY = Mouse.GetPosition(canvas).Y;

            Shape newShape = null;

            if (shapeName == "rectangle")
            {
                newShape = ShapeFactory.CreateShape(
                    ShapeType.Rectangle, 0, 0, Brushes.Blue, ShapeMinWidth, ShapeMinHeight) as Rectangle;
            }
            else if (shapeName == "ellipse")
            {
                newShape = ShapeFactory.CreateShape(
                    ShapeType.Ellipse, 0, 0, Brushes.Red, ShapeMinWidth, ShapeMinHeight) as Ellipse;

            }
            else if (shapeName == "triangle")
            {
                newShape = ShapeFactory.CreateShape(
                    ShapeType.Triangle, 0, 0, Brushes.Green, ShapeMinWidth, ShapeMinHeight) as Polygon;
            }

            if (newShape != null)
            {
                Canvas.SetLeft(newShape, mousePosX);
                Canvas.SetTop(newShape, mousePosY);
                CanvasShapes.Add(newShape);

                ShapeTreeViewItem shapeTreeViewItem = new ShapeTreeViewItem(shapeName);
                TreeItems.Add(shapeTreeViewItem);

                SelectionManager.Register(newShape, shapeTreeViewItem);
            }

            return newShape;
        }
    }
}
