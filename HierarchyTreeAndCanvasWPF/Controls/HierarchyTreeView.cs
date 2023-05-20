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

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            (ItemsSource as ObservableCollection<ShapeTreeViewItem>).CollectionChanged += HierarchyTreeView_CollectionChanged;
        }

        private void HierarchyTreeView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine($"TREE: New items {e.NewItems}");
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);

            Debug.WriteLine($"TREE: OnSelectedItemChanged Old> {e.OldValue} New> {e.NewValue}");

            if (e.NewValue is ShapeTreeViewItem newItem)
            {
                // prevent default behavior
                newItem.IsSelected = false;

                // TODO: remove
                IEnumerable<ShapeTreeViewItem> itemsToSelect = GetBranch(newItem);
                Debug.WriteLine($"itemsToSelect >>> \n");
                foreach (ShapeTreeViewItem i in itemsToSelect)
                {
                    Debug.WriteLine(i.Header);
                }
                Debug.WriteLine("");
                // ============

                bool shiftOrCtrlPressed = Keyboard.Modifiers == ModifierKeys.Control
                        || Keyboard.Modifiers == ModifierKeys.Shift;

                foreach (ShapeTreeViewItem item in itemsToSelect)
                {
                    SelectItem(item, only: !shiftOrCtrlPressed, selectionHandled: false);
                }

                Debug.WriteLine($"TREE: Selected items count {_selectedItems.Count}");
            }
        }

        private IEnumerable<ShapeTreeViewItem> GetBranch(ShapeTreeViewItem rootItem)
        {
            Debug.WriteLine($"TREE: GetBranch()");

            yield return rootItem;

            foreach (ShapeTreeViewItem child in rootItem.Items)
            {
                foreach (ShapeTreeViewItem nestedItem in GetBranch(child))
                {
                    yield return nestedItem;
                }
            }
        }

        public void SelectItem(ShapeTreeViewItem item, bool only = true, bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: SelectItem()");

            if (!_selectedItems.Contains(item))
            {
                _selectedItems.Add(item);
                item.Select();
            }

            if (only)
            {
                DeselectAllItemsExcept(item, selectionHandled: selectionHandled);
            }

            if (!selectionHandled)
            {
                SelectionManager.SelectCanvasShape(item, only);
            }
        }

        public void DeselectItem(ShapeTreeViewItem item, bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: DeselectItem()");

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

        private void DeselectAllItemsExcept(ShapeTreeViewItem item, bool selectionHandled = true)
        {
            Debug.WriteLine($"TREE: DeselectAllItemsExcept()");

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
