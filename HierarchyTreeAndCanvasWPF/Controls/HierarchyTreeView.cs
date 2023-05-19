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

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class HierarchyTreeView : TreeView
    {
        private IShapeCanvasViewModel _vm;
        private List<ShapeTreeViewItem> _selectedItems = new();

        public HierarchyTreeView()
        {
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.SelectOnly, (shape, item) => SelectOnly(item));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.SelectAdditional, (shape, item) => SelectAdditional(item));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.Deselect, (shape, item) => DeselectItem(item));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.Remove, (shape, item) => RemoveItem(item));
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
                    if (shiftOrCtrlPressed)
                    {
                        SelectAdditionalAndRaiseEvent(item);
                    }
                    else
                    {
                        SelectOnlyAndRaiseEvent(item);
                    }
                }

                Debug.WriteLine($"TREE: Selected items count {_selectedItems.Count}");
            }
        }

        private IEnumerable<ShapeTreeViewItem> GetBranch(ShapeTreeViewItem rootItem)
        {
            yield return rootItem;

            foreach (ShapeTreeViewItem child in rootItem.Items)
            {
                foreach (ShapeTreeViewItem nestedItem in GetBranch(child))
                {
                    yield return nestedItem;
                }
            }
        }

        private void SelectOnly(ShapeTreeViewItem item)
        {
            DeselectAllItemsExcept(item);

            if (!_selectedItems.Contains(item))
            {
                _selectedItems.Add(item);
                item.Select();
            }
        }

        private void SelectAdditional(ShapeTreeViewItem item)
        {
            if (!_selectedItems.Contains(item))
            {
                _selectedItems.Add(item);
                item.Select();
            }
        }

        private void SelectOnlyAndRaiseEvent(ShapeTreeViewItem item)
        {
            SelectOnly(item);
            ShapeEventMessenger.Publish(ShapeEventMessenger.SelectOnly, item.Id);
        }

        private void SelectAdditionalAndRaiseEvent(ShapeTreeViewItem item)
        {
            SelectAdditional(item);
            ShapeEventMessenger.Publish(ShapeEventMessenger.SelectAdditional, item.Id);
        }

        private void DeselectAllItems()
        {
            Debug.WriteLine($"TREE: Deselecting all items");

            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                DeselectItemAndRaiseEvent(_selectedItems[i]);
            }
        }

        private void DeselectAllItemsExcept(ShapeTreeViewItem item)
        {
            IEnumerable<ShapeTreeViewItem> doNotDeselect = GetBranch(item);

            for (int i = _selectedItems.Count - 1; i >= 0; i--)
            {
                if (!doNotDeselect.Contains(_selectedItems[i]))
                {
                    DeselectItemAndRaiseEvent(_selectedItems[i]);
                }
            }
        }

        private void DeselectItem(ShapeTreeViewItem item)
        {
            _selectedItems.Remove(item);
            item.Deselect();

            Debug.WriteLine($"TREE: Deselected {item.Header}");
        }

        private void DeselectItemAndRaiseEvent(ShapeTreeViewItem item)
        {
            DeselectItem(item);
            ShapeEventMessenger.Publish(ShapeEventMessenger.Deselect, item.Id);
        }

        private void RemoveItem(ShapeTreeViewItem item)
        {
            _vm.TreeItems.Remove(item);
        }

        private void RemoveItemAndRaiseEvent(ShapeTreeViewItem item)
        {
            RemoveItem(item);
            ShapeEventMessenger.Publish(ShapeEventMessenger.Remove, item.Id);
        }
    }
}
