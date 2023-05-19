using HierarchyTreeAndCanvasWPF.Controls;
using HierarchyTreeAndCanvasWPF.Extensions;
using HierarchyTreeAndCanvasWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Services
{
    public static class ShapeManager
    {
        private static Dictionary<string, Shape> shapeDictionary = new();
        private static Dictionary<string, ShapeTreeViewItem> treeViewItemDictionary = new();

        static ShapeManager()
        {
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.SelectOnly,
                (id) => RaiseShapeEvent(ShapeEventMessenger.SelectOnly, id));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.SelectAdditional,
                (id) => RaiseShapeEvent(ShapeEventMessenger.SelectAdditional, id));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.Deselect,
                (id) => RaiseShapeEvent(ShapeEventMessenger.Deselect, id));
            ShapeEventMessenger.Subscribe(ShapeEventMessenger.Remove,
                (id) => RaiseShapeEvent(ShapeEventMessenger.Remove, id));
        }

        public static void Register(Shape shape, ShapeTreeViewItem item)
        {
            string newId = UIDGenerator.GenerateUniqueId();
            shape.SetId(newId);
            item.Id = newId;
            shapeDictionary[newId] = shape;
            treeViewItemDictionary[newId] = item;
        }

        private static void RaiseShapeEvent(string eventName, string id)
        {
            Debug.WriteLine($"EVENT: {eventName} on {id}");
            ShapeEventMessenger.Publish(eventName, shapeDictionary[id], treeViewItemDictionary[id]);
        }
    }
}