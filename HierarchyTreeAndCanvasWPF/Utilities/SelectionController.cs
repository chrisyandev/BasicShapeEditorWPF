using HierarchyTreeAndCanvasWPF.Controls;
using HierarchyTreeAndCanvasWPF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Utilities
{
    public class SelectionController
    {
        private Dictionary<string, Shape> shapeDictionary;
        private Dictionary<string, ShapeTreeViewItem> treeViewItemDictionary;

        public SelectionController()
        {
            shapeDictionary = new Dictionary<string, Shape>();
            treeViewItemDictionary = new Dictionary<string, ShapeTreeViewItem>();
        }

        public void Select(Shape shape)
        {
            treeViewItemDictionary[shape.GetId()].MSelected = true;
        }

        public void Select(ShapeTreeViewItem item)
        {

        }

        /*        public void RegisterShapeAndTreeViewItem(Shape shape, TreeViewItem treeViewItem)
                {
                    string shapeId = GenerateUniqueShapeId(); // Generate or obtain a unique identifier for the shape
                    shapeDictionary[shapeId] = shape;
                    treeViewItemDictionary[shapeId] = treeViewItem;
                }

                public void SelectShape(Shape shape)
                {
                    // Select the shape
                    // ...

                    string shapeId = GetShapeId(shape); // Retrieve the shape ID based on the shape
                    if (treeViewItemDictionary.TryGetValue(shapeId, out var treeViewItem))
                    {
                        // Select the TreeViewItem
                        // ...
                    }
                }

                public void SelectTreeViewItem(TreeViewItem treeViewItem)
                {
                    // Select the TreeViewItem
                    // ...

                    string shapeId = GetShapeIdFromTreeViewItem(treeViewItem); // Retrieve the shape ID based on the TreeViewItem
                    if (shapeDictionary.TryGetValue(shapeId, out var shape))
                    {
                        // Select the Shape
                        // ...
                    }
                }

                private string GetShapeId(Shape shape)
                {
                    // Retrieve the shape ID based on the shape
                    // ...
                }

                private string GetShapeIdFromTreeViewItem(TreeViewItem treeViewItem)
                {
                    // Retrieve the shape ID based on the TreeViewItem
                    // ...
                }
            }*/

    }
}