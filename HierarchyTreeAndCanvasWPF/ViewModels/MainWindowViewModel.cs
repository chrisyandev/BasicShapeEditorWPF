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

            SetShapeToAddCommand = new DoActionWithParameterCommand(shapeName =>
            {
                ShapeToAdd = shapeName as string;
            });

            CanvasShapes = new ObservableCollection<Shape>();
        }

        public ObservableCollection<TreeItem> TreeItems { get; set; }

        public ObservableCollection<Shape> CanvasShapes { get; set; }

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

        public ICommand SetShapeToAddCommand { get; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Shape AddShape(Canvas canvas)
        {
            double mousePosX = Mouse.GetPosition(canvas).X;
            double mousePosY = Mouse.GetPosition(canvas).Y;

            if (ShapeToAdd == "rectangle")
            {
                Rectangle newRectangle = ShapeFactory.CreateShape(
                    ShapeType.Rectangle, 100, 100, Brushes.Blue) as Rectangle;

                CanvasItem newCanvasItem = new()
                {
                    Id = 1,
                    Shape = newRectangle,
                    X = mousePosX - (newRectangle.Width / 2),
                    Y = mousePosY - (newRectangle.Height / 2)
                };

                Canvas.SetLeft(newRectangle, newCanvasItem.X);
                Canvas.SetTop(newRectangle, newCanvasItem.Y);

                CanvasShapes.Add(newRectangle);
                return newRectangle;
            }
            else if (ShapeToAdd == "ellipse")
            {
                Ellipse newEllipse = ShapeFactory.CreateShape(
                    ShapeType.Ellipse, 100, 100, Brushes.Red) as Ellipse;

                CanvasItem newCanvasItem = new CanvasItem
                {
                    Id = 1,
                    Shape = newEllipse,
                    X = mousePosX - (newEllipse.Width / 2),
                    Y = mousePosY - (newEllipse.Height / 2)
                };

                Canvas.SetLeft(newEllipse, newCanvasItem.X);
                Canvas.SetTop(newEllipse, newCanvasItem.Y);

                CanvasShapes.Add(newEllipse);
                return newEllipse;
            }
            else if (ShapeToAdd == "triangle")
            {
                Polygon newTriangle = ShapeFactory.CreateShape(
                    ShapeType.Triangle, 100, 100, Brushes.Green) as Polygon;

                double centroidX = (newTriangle.Points[0].X + newTriangle.Points[1].X + newTriangle.Points[2].X) / 3;
                double centroidY = (newTriangle.Points[0].Y + newTriangle.Points[1].Y + newTriangle.Points[2].Y) / 3;

                CanvasItem newCanvasItem = new CanvasItem
                {
                    Id = 1,
                    Shape = newTriangle,
                    X = mousePosX - centroidX,
                    Y = mousePosY - centroidY
                };

                Canvas.SetLeft(newTriangle, newCanvasItem.X);
                Canvas.SetTop(newTriangle, newCanvasItem.Y);

                CanvasShapes.Add(newTriangle);
                return newTriangle;
            }

            return null;
        }
    }
}
