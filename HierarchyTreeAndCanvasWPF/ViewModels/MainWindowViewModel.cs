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
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _shapeToAdd;

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

            SetShapeToAddCommand = new DoActionWithParameterCommand(shapeName =>
            {
                ShapeToAdd = shapeName as string;
            });

            // hardcoded for now
            ShapeMinWidth = 10;
            ShapeMinHeight = 10;
        }

        public ObservableCollection<TreeItem> TreeItems { get; set; }

        public ObservableCollection<Shape> CanvasShapes { get; set; }

        public ObservableCollection<Shape> SelectedCanvasShapes { get; set; }

        public string ShapeToAdd
        {
            get { return _shapeToAdd; }
            set
            {
                if (value == _shapeToAdd) { return; }
                _shapeToAdd = value;
                OnPropertyChanged();

                Debug.WriteLine(_shapeToAdd);
            }
        }

        public double ShapeMinWidth { get; }

        public double ShapeMinHeight { get; }

        public ICommand SetShapeToAddCommand { get; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Shape AddShapeToCanvas(Canvas canvas)
        {
            double mousePosX = Mouse.GetPosition(canvas).X;
            double mousePosY = Mouse.GetPosition(canvas).Y;

            if (ShapeToAdd == "rectangle")
            {
                Rectangle newRectangle = ShapeFactory.CreateShape(
                    ShapeType.Rectangle, 100, 100, Brushes.Blue, ShapeMinWidth, ShapeMinHeight) as Rectangle;
                Canvas.SetLeft(newRectangle, mousePosX - (newRectangle.Width / 2));
                Canvas.SetTop(newRectangle, mousePosY - (newRectangle.Height / 2));
                CanvasShapes.Add(newRectangle);
                return newRectangle;
            }
            else if (ShapeToAdd == "ellipse")
            {
                Ellipse newEllipse = ShapeFactory.CreateShape(
                    ShapeType.Ellipse, 100, 100, Brushes.Red, ShapeMinWidth, ShapeMinHeight) as Ellipse;
                Canvas.SetLeft(newEllipse, mousePosX - (newEllipse.Width / 2));
                Canvas.SetTop(newEllipse, mousePosY - (newEllipse.Height / 2));
                CanvasShapes.Add(newEllipse);
                return newEllipse;
            }
            else if (ShapeToAdd == "triangle")
            {
                Polygon newTriangle = ShapeFactory.CreateShape(
                    ShapeType.Triangle, 100, 100, Brushes.Green, ShapeMinWidth, ShapeMinHeight) as Polygon;
                double centroidX = (newTriangle.Points[0].X + newTriangle.Points[1].X + newTriangle.Points[2].X) / 3;
                double centroidY = (newTriangle.Points[0].Y + newTriangle.Points[1].Y + newTriangle.Points[2].Y) / 3;
                Canvas.SetLeft(newTriangle, mousePosX - centroidX);
                Canvas.SetTop(newTriangle, mousePosY - centroidY);
                CanvasShapes.Add(newTriangle);
                return newTriangle;
            }

            return null;
        }
    }
}
