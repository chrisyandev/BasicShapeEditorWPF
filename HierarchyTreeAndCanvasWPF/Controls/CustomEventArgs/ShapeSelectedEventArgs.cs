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
        public bool Removed { get; }

        public ShapeStateChangedEventArgs(Shape shape, bool selected, 
            SelectionType selectionType = SelectionType.None, bool removed = false)
        {
            Shape = shape;
            Selected = selected;
            SelectionType = selectionType;
            Removed = removed;
        }
    }

    public enum SelectionType
    {
        None,
        Only,
        Additional
    }
}
