using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Controls.CustomEventArgs
{
    public class ShapeSelectedEventArgs : EventArgs
    {
        public Shape Shape { get; }
        public SelectionType SelectionType { get; }

        public ShapeSelectedEventArgs(Shape shape, SelectionType selectionType)
        {
            Shape = shape;
            SelectionType = selectionType;
        }
    }

    public enum SelectionType
    {
        Only,
        Additional
    }
}
