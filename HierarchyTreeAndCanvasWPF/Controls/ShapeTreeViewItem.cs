using HierarchyTreeAndCanvasWPF.Services;
using HierarchyTreeAndCanvasWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class ShapeTreeViewItem : TreeViewItem
    {
        private static readonly Brush HighlightSelectedBrush = (Brush)new BrushConverter().ConvertFrom("#0078D7");

        private bool _mSelected;

        public ShapeTreeViewItem(string header)
        {
            Header = header;
            _mSelected = false;
            AllowDrop = true;
        }

        public string Id { get; set; }

        public bool MSelected
        {
            get
            {
                return _mSelected;
            }
            set
            {
                if (_mSelected == value) { return; }

                Debug.WriteLine($"{Header} _mSelected old: {_mSelected} /// new: {value}");

                _mSelected = value;

                if (value)
                {
                    Highlight();
                }
                else
                {
                    RemoveHighlight();
                }
            }
        }

        public void Select()
        {
            MSelected = true;

/*            if (Items.Count > 0)
            {
                foreach (ShapeTreeViewItem item in Items)
                {
                    item.Select();
                }
            }*/
        }

        public void Deselect()
        {
            MSelected = false;

/*            if (Items.Count > 0)
            {
                foreach (ShapeTreeViewItem item in Items)
                {
                    item.Deselect();
                }
            }*/
        }

        public void Highlight()
        {
            Background = Brushes.Red;
            Foreground = Brushes.White;
        }

        public void RemoveHighlight()
        {
            Background = Brushes.Transparent;
            Foreground = Brushes.Black;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.WriteLine("Tree: drag start");

                DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            ShapeTreeViewItem targetItem = e.Source as ShapeTreeViewItem;

            if (targetItem != null)
            {
                targetItem.Highlight();
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            ShapeTreeViewItem targetItem = e.Source as ShapeTreeViewItem;

            if (targetItem != null)
            {
                targetItem.RemoveHighlight();
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            ShapeTreeViewItem droppedItem = e.Data.GetData(typeof(ShapeTreeViewItem)) as ShapeTreeViewItem;
            ShapeTreeViewItem receivingItem = e.Source as ShapeTreeViewItem;

            receivingItem.RemoveHighlight();

            if (droppedItem == receivingItem || droppedItem.Parent == receivingItem || receivingItem.IsDescendantOf(droppedItem))
            {
                return;
            }

            Debug.WriteLine($"Tree: dropped {droppedItem} /// onto /// {receivingItem}");

            if (droppedItem.Parent is ShapeTreeViewItem parentItem)
            {
                parentItem.Items.Remove(droppedItem);
                receivingItem.Items.Add(droppedItem);
            }
            else if (UIHelper.FindTreeView(droppedItem) is TreeView treeView)
            {
                (treeView.ItemsSource as ObservableCollection<ShapeTreeViewItem>).Remove(droppedItem);
                receivingItem.Items.Add(droppedItem);
            }

            droppedItem.Highlight();
        }

    }
}
