using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HierarchyTreeWPF.Extensions
{
    // Just some practice using extension methods
    public static class Extensions
    {
        public static void Resize(this Rectangle rect, float width, float height)
        {
            rect.Width = width;
            rect.Height = height;
        }
    }
}
