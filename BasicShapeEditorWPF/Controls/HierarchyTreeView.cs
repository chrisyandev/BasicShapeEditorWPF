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
using BasicShapeEditorWPF.ViewModels;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BasicShapeEditorWPF.Services;
using System.Runtime.CompilerServices;
using BasicShapeEditorWPF.Utilities;

namespace BasicShapeEditorWPF.Controls
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

        private static TextBlock _previouslyHoveredOver;

        private IShapeCanvasViewModel _vm;
        private List<ShapeTreeViewItem> _selectedItems = new();

        public SelectionManager SelectionManager
        {
            get { return (SelectionManager)GetValue(SelectionManagerProperty); }
            set { SetValue(SelectionManagerProperty, value); }
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

        public void RemoveItem(ShapeTreeViewItem item, bool selectionHandled = true)
        {
            _vm.TreeItems.Remove(item);

            if (!selectionHandled)
            {
                SelectionManager.RemoveCanvasShape(item);
            }
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.OriginalSource is TextBlock currentlyHoveringOver)
            {
                if (currentlyHoveringOver != _previouslyHoveredOver)
                {
                    RemoveHoverBackground(_previouslyHoveredOver);
                    SetHoverBackground(currentlyHoveringOver);
                    _previouslyHoveredOver = currentlyHoveringOver;
                }
            }
            else if (_previouslyHoveredOver != null)
            {
                RemoveHoverBackground(_previouslyHoveredOver);
                _previouslyHoveredOver = null;
            }
        }

        private void SetHoverBackground(TextBlock textBlock)
        {
            if (textBlock != null)
            {
                ShapeTreeViewItem targetItem = UIHelper.FindVisualParent<ShapeTreeViewItem>(textBlock);

                // If item is selected, let it keep its own styling
                if (!targetItem.MSelected)
                {
                    Brush HoverBrush = (Brush)new BrushConverter().ConvertFrom("#505050");
                    textBlock.Background = HoverBrush;
                }
            }
        }

        private void RemoveHoverBackground(TextBlock textBlock)
        {
            if (textBlock != null)
            {
                ShapeTreeViewItem targetItem = UIHelper.FindVisualParent<ShapeTreeViewItem>(textBlock);

                // If item is selected, let it keep its own styling
                if (!targetItem.MSelected)
                {
                    textBlock.Background = Brushes.Transparent;
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
    }
}
