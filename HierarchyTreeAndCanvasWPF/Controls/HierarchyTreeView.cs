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
using HierarchyTreeAndCanvasWPF.Controls.CustomEventArgs;

namespace HierarchyTreeAndCanvasWPF.Controls
{
    public class HierarchyTreeView : TreeView
    {
        private static readonly Brush HighlightSelectedBrush = (Brush)new BrushConverter().ConvertFrom("#0078D7");

        private IShapeCanvasViewModel _vm;
        private List<TreeItem> _selectedItems = new();

        public event EventHandler<ShapeStateChangedEventArgs> ShapeSelected;

        public void HandleShapeSelected(object sender, ShapeStateChangedEventArgs e)
        {
            foreach (TreeItem item in _vm.TreeItems)
            {
                if (item.ShapeRef == e.Shape)
                {
                    if (e.Selected)
                    {
                        if (e.SelectionType == SelectionType.Additional)
                        {
                            SelectAdditional(item);
                        }
                        else if (e.SelectionType == SelectionType.Only)
                        {
                            SelectOnly(item);
                        }
                    }
                    else
                    {
                        RemoveHighlight(item);
                    }

                    break;
                }
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _vm = DataContext as IShapeCanvasViewModel;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            (ItemsSource as ObservableCollection<TreeItem>).CollectionChanged += HierarchyTreeView_CollectionChanged;
        }

        private void HierarchyTreeView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine($"NEW ITEMS: {e.NewItems}");
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);

            if (e.NewValue is TreeItem item)
            {
                Debug.WriteLine($"SELECTED ITEM: {item}");

                // prevent default behavior
                item.IsSelected = false;

                // prevent adding item more than once
                if (_selectedItems.Contains(item))
                {
                    return;
                }

                if (Keyboard.Modifiers == ModifierKeys.Control
                    || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    SelectAdditionalAndRaiseEvent(item);
                }
                else
                {
                    SelectOnlyAndRaiseEvent(item);
                }
            }
        }

        private void SelectOnly(TreeItem item)
        {
            foreach (TreeItem i in _selectedItems)
            {
                RemoveHighlight(i);
            }
            _selectedItems.Clear();
            _selectedItems.Add(item);
            HighlightItem(item);
        }

        private void SelectAdditional(TreeItem item)
        {
            _selectedItems.Add(item);
            HighlightItem(item);
        }

        private void SelectOnlyAndRaiseEvent(TreeItem item)
        {
            SelectOnly(item);
            ShapeSelected(this, new ShapeStateChangedEventArgs(item.ShapeRef, true, SelectionType.Only));
        }

        private void SelectAdditionalAndRaiseEvent(TreeItem item)
        {
            SelectAdditional(item);
            ShapeSelected(this, new ShapeStateChangedEventArgs(item.ShapeRef, true, SelectionType.Additional));
        }

        private void HighlightItem(TreeItem item)
        {
            item.Background = HighlightSelectedBrush;
            item.Foreground = Brushes.White;
        }

        private void RemoveHighlight(TreeItem item)
        {
            item.Background = Brushes.Transparent;
            item.Foreground = Brushes.Black;
        }
    }
}
