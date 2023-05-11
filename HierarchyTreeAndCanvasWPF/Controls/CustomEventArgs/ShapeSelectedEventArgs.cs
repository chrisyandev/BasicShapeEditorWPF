using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Controls.CustomEventArgs
{
    public class ShapeStateChangedEventArgs : EventArgs
    {
        public Shape Shape { get; }
        public bool Selected { get; }
        public SelectionType SelectionType { get; }

        public ShapeStateChangedEventArgs(Shape shape, bool selected, SelectionType selectionType = SelectionType.None)
        {
            Shape = shape;
            Selected = selected;
            SelectionType = selectionType;
        }
    }

    public enum SelectionType
    {
        None,
        Only,
        Additional
    }
}
