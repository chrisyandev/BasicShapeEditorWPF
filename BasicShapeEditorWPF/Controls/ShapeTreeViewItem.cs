﻿using BasicShapeEditorWPF.Services;
using BasicShapeEditorWPF.Utilities;
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

namespace BasicShapeEditorWPF.Controls
{
    public class ShapeTreeViewItem : TreeViewItem
    {
        private static readonly Brush SelectedBrush = (Brush)new BrushConverter().ConvertFrom("#32699A");
        private static readonly Brush HoverBrush = (Brush)new BrushConverter().ConvertFrom("#505050");

        private bool _mSelected;

        public ShapeTreeViewItem(string header)
        {
            Header = header;
            _mSelected = false;
            AllowDrop = true;
            Background = Brushes.Transparent;
            Foreground = Brushes.White;
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
        }

        public void Deselect()
        {
            MSelected = false;
        }

        public void Highlight()
        {
            TextBlock targetTextBlock = UIHelper.FindVisualChild<TextBlock>(this);
            targetTextBlock.Background = SelectedBrush;
        }

        public void RemoveHighlight()
        {
            TextBlock targetTextBlock = UIHelper.FindVisualChild<TextBlock>(this);
            targetTextBlock.Background = Brushes.Transparent;
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

            if (targetItem != null && !targetItem.MSelected)
            {
                targetItem.RemoveHighlight();
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            ShapeTreeViewItem droppedItem = e.Data.GetData(typeof(ShapeTreeViewItem)) as ShapeTreeViewItem;
            ShapeTreeViewItem receivingItem = e.Source as ShapeTreeViewItem;

            if (!receivingItem.MSelected)
            {
                receivingItem.RemoveHighlight();
            }

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
