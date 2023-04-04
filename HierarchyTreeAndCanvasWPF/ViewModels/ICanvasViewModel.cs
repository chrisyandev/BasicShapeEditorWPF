using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.ViewModels
{
    public interface ICanvasViewModel
    {
        public ObservableCollection<Shape> CanvasShapes { get; set; }
        public ObservableCollection<Shape> SelectedCanvasShapes { get; set; }
        public string ShapeToAdd { get; }
        public Shape AddShapeToCanvas(string shapeName, Canvas canvas);
    }
}
