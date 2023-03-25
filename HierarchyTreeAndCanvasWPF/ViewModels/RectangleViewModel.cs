using HierarchyTreeAndCanvasWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.ViewModels
{
    public class RectangleViewModel : ShapeViewModelBase
    {
        private CanvasItem _canvasItem;
        private Rectangle _rectangle;

        public RectangleViewModel(CanvasItem canvasItem)
        {
            _canvasItem = canvasItem;
            _rectangle = canvasItem.Shape as Rectangle;
        }

        public double Width
        {
            get { return _rectangle.Width; }
            set
            {
                if (value == _rectangle.Width) { return; }
                _rectangle.Width = value;
                OnPropertyChanged();

                Debug.WriteLine(_rectangle.Width);
            }
        }

        public double Height
        {
            get { return _rectangle.Height; }
            set
            {
                if (value == _rectangle.Height) { return; }
                _rectangle.Height = value;
                OnPropertyChanged();

                Debug.WriteLine(_rectangle.Height);
            }
        }

        public Brush Fill
        {
            get { return _rectangle.Fill; }
            set
            {
                if (value == _rectangle.Fill) { return; }
                _rectangle.Fill = value;
                OnPropertyChanged();

                Debug.WriteLine(_rectangle.Fill);
            }
        }

        public double X
        {
            get { return _canvasItem.X; }
            set
            {
                if (value == _canvasItem.X) { return; }
                _canvasItem.X = value;
                OnPropertyChanged();

                Debug.WriteLine(_canvasItem.X);
            }
        }

        public double Y
        {
            get { return _canvasItem.Y; }
            set
            {
                if (value == _canvasItem.Y) { return; }
                _canvasItem.Y = value;
                OnPropertyChanged();

                Debug.WriteLine(_canvasItem.Y);
            }
        }

    }
}
