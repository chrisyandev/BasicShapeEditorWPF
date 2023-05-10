using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class TreeItem : TreeViewItem
    {
        public TreeItem(string header, Shape shapeRef)
        {
            Header = header;
            ShapeRef = shapeRef;

            Debug.WriteLine(Parent);
        }

        public Shape ShapeRef { get; }
    }
}
