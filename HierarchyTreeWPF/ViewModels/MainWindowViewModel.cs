using HierarchyTreeWPF.Models;
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
using HierarchyTreeWPF.Commands;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HierarchyTreeWPF.ViewModels
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

            CanvasItems = new ObservableCollection<ShapeViewModelBase>();

            SetShapeToAddCommand = new DoActionWithParameterCommand(shapeName =>
            {
                ShapeToAdd = shapeName as string;
            });
        }

        public ObservableCollection<TreeItem> TreeItems { get; set; }

        public ObservableCollection<ShapeViewModelBase> CanvasItems { get; set; }

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

        public void AddShape(Canvas canvas)
        {
            if (ShapeToAdd == "rectangle")
            {
                Rectangle newRectangle = new Rectangle
                {
                    Width = 50,
                    Height = 50,
                    Fill = Brushes.Red
                };

                CanvasItem newCanvasItem = new CanvasItem
                {
                    Id = 1,
                    Shape = newRectangle,
                    X = Mouse.GetPosition(canvas).X - (newRectangle.Width / 2),
                    Y = Mouse.GetPosition(canvas).Y - (newRectangle.Height / 2)
                };

                CanvasItems.Add(new RectangleViewModel(newCanvasItem));
            }
            else if (ShapeToAdd == "ellipse")
            {
                Ellipse newEllipse = new Ellipse
                {
                    Width = 50,
                    Height = 50,
                    Fill = Brushes.Blue
                };

                CanvasItem newCanvasItem = new CanvasItem
                {
                    Id = 1,
                    Shape = newEllipse,
                    X = Mouse.GetPosition(canvas).X - (newEllipse.Width / 2),
                    Y = Mouse.GetPosition(canvas).Y - (newEllipse.Height / 2)
                };

                CanvasItems.Add(new EllipseViewModel(newCanvasItem));
            }
            else if (ShapeToAdd == "triangle")
            {
                Polygon newTriangle = new Polygon
                {
                    Fill = Brushes.Green,
                    Points = new PointCollection
                    {
                        new Point(0, 50),
                        new Point(25, 0),
                        new Point(50, 50)
                    }
                };

                double centroidX = (newTriangle.Points[0].X + newTriangle.Points[1].X + newTriangle.Points[2].X) / 3;
                double centroidY = (newTriangle.Points[0].Y + newTriangle.Points[1].Y + newTriangle.Points[2].Y) / 3;

                CanvasItem newCanvasItem = new CanvasItem
                {
                    Id = 1,
                    Shape = newTriangle,
                    X = Mouse.GetPosition(canvas).X - centroidX,
                    Y = Mouse.GetPosition(canvas).Y - centroidY
                };

                CanvasItems.Add(new TriangleViewModel(newCanvasItem));
            }
        }
    }
}
