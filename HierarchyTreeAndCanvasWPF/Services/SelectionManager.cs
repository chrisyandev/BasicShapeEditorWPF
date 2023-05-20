using HierarchyTreeAndCanvasWPF.Controls;
using HierarchyTreeAndCanvasWPF.Extensions;
using HierarchyTreeAndCanvasWPF.Utilities;
using HierarchyTreeAndCanvasWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Services
{
    public class SelectionManager
    {
        private Dictionary<string, Shape> _shapeDictionary;
        private Dictionary<string, ShapeTreeViewItem> _shapeTreeViewItemDictionary;

        public SelectionManager()
        {
            _shapeDictionary = new();
            _shapeTreeViewItemDictionary = new();
        }

        public IShapeCanvasViewModel VM { get; set; }
        public ShapeCanvas ShapeCanvas { get; set; }
        public HierarchyTreeView HierarchyTreeView { get; set; }

        public void SelectTreeViewItem(Shape shape, bool only)
        {
            ShapeTreeViewItem targetItem = _shapeTreeViewItemDictionary[shape.GetId()];
            HierarchyTreeView.SelectItem(targetItem, only: only, selectionHandled: true);
        }

        public void DeselectTreeViewItem(Shape shape)
        {
            ShapeTreeViewItem targetItem = _shapeTreeViewItemDictionary[shape.GetId()];
            HierarchyTreeView.DeselectItem(targetItem, selectionHandled: true);
        }

        public void RemoveTreeViewItem(Shape shape)
        {
            ShapeTreeViewItem targetItem = _shapeTreeViewItemDictionary[shape.GetId()];
            HierarchyTreeView.RemoveItem(targetItem, selectionHandled: true);
        }

        public void SelectCanvasShape(ShapeTreeViewItem shapeTreeViewItem, bool only)
        {
            Shape targetShape = _shapeDictionary[shapeTreeViewItem.Id];
            ShapeCanvas.SelectShape(targetShape, only: only, selectionHandled: true);
        }

        public void DeselectCanvasShape(ShapeTreeViewItem shapeTreeViewItem)
        {
            Shape targetShape = _shapeDictionary[shapeTreeViewItem.Id];
            ShapeCanvas.DeselectShape(targetShape, selectionHandled: true);
        }

        public void RemoveCanvasShape(ShapeTreeViewItem shapeTreeViewItem)
        {
            Shape targetShape = _shapeDictionary[shapeTreeViewItem.Id];
            ShapeCanvas.RemoveShape(targetShape, selectionHandled: true);
        }

        public void Register(Shape shape, ShapeTreeViewItem shapeTreeViewItem)
        {
            string newId = UIDGenerator.GenerateUniqueId();

            shape.SetId(newId);
            _shapeDictionary[newId] = shape;

            shapeTreeViewItem.Id = newId;
            _shapeTreeViewItemDictionary[newId] = shapeTreeViewItem;
        }
    }
}
