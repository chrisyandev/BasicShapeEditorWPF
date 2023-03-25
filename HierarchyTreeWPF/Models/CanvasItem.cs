using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HierarchyTreeWPF.Models
{
    public class CanvasItem
    {
        public int Id {  get; set; }
        public Shape Shape { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
