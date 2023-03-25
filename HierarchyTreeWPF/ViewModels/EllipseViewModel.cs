using HierarchyTreeWPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchyTreeWPF.ViewModels
{
    public class EllipseViewModel : ShapeViewModelBase
    {
        private CanvasItem _canvasItem;
        private Ellipse _ellipse;

        public EllipseViewModel(CanvasItem canvasItem)
        {
            _canvasItem = canvasItem;
            _ellipse = _canvasItem.Shape as Ellipse;
        }

        public double Width
        {
            get { return _ellipse.Width; }
            set
            {
                if (value == _ellipse.Width) { return; }
                _ellipse.Width = value;
                OnPropertyChanged();

                Debug.WriteLine(_ellipse.Width);
            }
        }

        public double Height
        {
            get { return _ellipse.Height; }
            set
            {
                if (value == _ellipse.Height) { return; }
                _ellipse.Height = value;
                OnPropertyChanged();

                Debug.WriteLine(_ellipse.Height);
            }
        }

        public Brush Fill
        {
            get { return _ellipse.Fill; }
            set
            {
                if (value == _ellipse.Fill) { return; }
                _ellipse.Fill = value;
                OnPropertyChanged();

                Debug.WriteLine(_ellipse.Fill);
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
