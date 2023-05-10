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
        public TreeItem(string header, Shape shapeRef, Canvas canvasRef)
        {
            Header = header;
            ShapeRef = shapeRef;
            CanvasRef = canvasRef;

            Debug.WriteLine(Parent);
        }

        public Shape ShapeRef { get; }
        
        public Canvas CanvasRef { get; }

    }
}
