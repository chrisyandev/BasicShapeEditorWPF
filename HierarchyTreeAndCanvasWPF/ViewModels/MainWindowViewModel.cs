using HierarchyTreeAndCanvasWPF.Models;
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

namespace HierarchyTreeAndCanvasWPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged, IShapeCanvasViewModel
    {
        private string _activeTool;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            TreeItems = new ObservableCollection<TreeItem>
            {
                new TreeItem(0, "Square"),
                new TreeItem(1, "Triangle")
            };
            TreeItems[1].Items.Add(new TreeItem(2, "Circle"));

            CanvasShapes = new ObservableCollection<Shape>();
            SelectedCanvasShapes = new ObservableCollection<Shape>();

            SetActiveToolCommand = new DoActionWithParameterCommand(shapeName =>
            {
                ActiveTool = shapeName as string;
            });

            // hardcoded for now
            ShapeMinWidth = 0;
            ShapeMinHeight = 0;
        }

        public ObservableCollection<TreeItem> TreeItems { get; set; }

        public ObservableCollection<Shape> CanvasShapes { get; set; }

        public ObservableCollection<Shape> SelectedCanvasShapes { get; set; }

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

            if (shapeName == "rectangle")
            {
                Rectangle newRectangle = ShapeFactory.CreateShape(
                    ShapeType.Rectangle, 0, 0, Brushes.Blue, ShapeMinWidth, ShapeMinHeight) as Rectangle;
                Canvas.SetLeft(newRectangle, mousePosX);
                Canvas.SetTop(newRectangle, mousePosY);
                CanvasShapes.Add(newRectangle);
                return newRectangle;
            }
            else if (shapeName == "ellipse")
            {
                Ellipse newEllipse = ShapeFactory.CreateShape(
                    ShapeType.Ellipse, 0, 0, Brushes.Red, ShapeMinWidth, ShapeMinHeight) as Ellipse;
                Canvas.SetLeft(newEllipse, mousePosX);
                Canvas.SetTop(newEllipse, mousePosY);
                CanvasShapes.Add(newEllipse);
                return newEllipse;
            }
            else if (shapeName == "triangle")
            {
                Polygon newTriangle = ShapeFactory.CreateShape(
                    ShapeType.Triangle, 0, 0, Brushes.Green, ShapeMinWidth, ShapeMinHeight) as Polygon;
/*                double centroidX = (newTriangle.Points[0].X + newTriangle.Points[1].X + newTriangle.Points[2].X) / 3;
                double centroidY = (newTriangle.Points[0].Y + newTriangle.Points[1].Y + newTriangle.Points[2].Y) / 3;*/
                Canvas.SetLeft(newTriangle, mousePosX);
                Canvas.SetTop(newTriangle, mousePosY);
                CanvasShapes.Add(newTriangle);
                return newTriangle;
            }

            return null;
        }
    }
}
