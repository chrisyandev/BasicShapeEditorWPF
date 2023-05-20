using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using HierarchyTreeAndCanvasWPF.ViewModels;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using HierarchyTreeAndCanvasWPF.Services;
using System.Runtime.CompilerServices;

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class HierarchyTreeView : TreeView
    {
        public static readonly DependencyProperty SelectionManagerProperty =
            DependencyProperty.Register(
                "SelectionManager",
                typeof(SelectionManager),
                typeof(HierarchyTreeView),
                new PropertyMetadata(OnSelectionManagerPropertyChanged)
            );

        private static void OnSelectionManagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectionManager selectionManager = (SelectionManager)e.NewValue;
            selectionManager.HierarchyTreeView = (HierarchyTreeView)d;
        }

        private IShapeCanvasViewModel _vm;
        private List<ShapeTreeViewItem> _selectedItems = new();

        public SelectionManager SelectionManager
        {
            get { return (SelectionManager)GetValue(SelectionManagerProperty); }
            set { SetValue(SelectionManagerProperty, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _vm = DataContext as IShapeCanvasViewModel;
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);

            if (e.NewValue is ShapeTreeViewItem newItem)
            {
                // prevent default behavior
                newItem.IsSelected = false;

                bool shiftOrCtrlPressed = Keyboard.Modifiers == ModifierKeys.Control
                    || Keyboard.Modifiers == ModifierKeys.Shift;

                if (shiftOrCtrlPressed)
                {
                    SelectBranch(newItem);
                }
                else
                {
                    DeselectAllItems(selectionHandled: false);
                    SelectBranch(newItem);
                }
            }
        }

        private List<ShapeTreeViewItem> GetBranchItems(ShapeTreeViewItem rootItem)
        {
            List<ShapeTreeViewItem> items = new() { rootItem };

            foreach (ShapeTreeViewItem child in rootItem.Items)
            {
                foreach (ShapeTreeViewItem nestedItem in GetBranchItems(child))
                {
                    items.Add(nestedItem);
                }
            }

            return items;
        }

        private void SelectBranch(ShapeTreeViewItem rootItem)
        {
            Debug.WriteLine($"TREE: SelectBranch() on {rootItem}");
            
            if (!_selectedItems.Contains(rootItem))
            {
                rootItem.Select();
                _selectedItems.Add(rootItem);
                SelectionManager.SelectCanvasShape(rootItem, false);
            }

            foreach (ShapeTreeViewItem child in rootItem.Items)
            {
                SelectBranch(child);
            }
        }

        public void SelectItem(ShapeTreeViewItem item, bool only = true, bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: SelectItem() on {item}");

            SelectBranch(item);

            if (only)
            {
                DeselectAllExceptBranch(item, selectionHandled: selectionHandled);
            }
            Debug.WriteLine($"TREE: _selectedItems Count {_selectedItems.Count}");
        }

        public void DeselectItem(ShapeTreeViewItem item, bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: DeselectItem() on {item}");

            item.Deselect();
            _selectedItems.Remove(item);

            if (!selectionHandled)
            {
                SelectionManager.DeselectCanvasShape(item);
            }
        }

        private void DeselectAllItems(bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: DeselectAllItems()");

            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                DeselectItem(_selectedItems[i], selectionHandled: selectionHandled);
            }
        }

        private void DeselectAllExceptBranch(ShapeTreeViewItem item, bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: DeselectAllExceptBranch()");

            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                if (_selectedItems[i] == item || _selectedItems[i].IsDescendantOf(item))
                {
                    continue;
                }
                DeselectItem(_selectedItems[i], selectionHandled: selectionHandled);
            }
        }

        public void RemoveItem(ShapeTreeViewItem item, bool selectionHandled = true)
        {
            _vm.TreeItems.Remove(item);

            if (!selectionHandled)
            {
                SelectionManager.RemoveCanvasShape(item);
            }
        }
    }
}
