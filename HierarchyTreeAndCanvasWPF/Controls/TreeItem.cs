using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class TreeItem : TreeViewItem
    {
        private static readonly Brush HighlightSelectedBrush = (Brush)new BrushConverter().ConvertFrom("#0078D7");

        private bool _mSelected;

        public TreeItem(string header, Shape shapeRef)
        {
            Header = header;
            ShapeRef = shapeRef;
            _mSelected = false;
        }

        public Shape ShapeRef { get; }

        public bool MSelected
        {
            get
            {
                return _mSelected;
            }
            set
            {
                if (_mSelected == value) { return; }
                _mSelected = value;

                if (value)
                {
                    HighlightItem();
                }
                else
                {
                    RemoveHighlight();
                }
            }
        }

        private void HighlightItem()
        {
            Background = Brushes.Red;
            Foreground = Brushes.White;
        }

        private void RemoveHighlight()
        {
            Background = Brushes.Transparent;
            Foreground = Brushes.Black;
        }
    }
}
