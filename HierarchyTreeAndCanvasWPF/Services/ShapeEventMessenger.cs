using HierarchyTreeAndCanvasWPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HierarchyTreeAndCanvasWPF.Services
{
    public static class ShapeEventMessenger
    {
        private static readonly Dictionary<string, Action<string>> idMessageHandlers = new();
        private static readonly Dictionary<string, Action<Shape, ShapeTreeViewItem>> shapeMessageHandlers = new();

        // pre-defined messages
        public const string SelectOnly = "SelectOnly";
        public const string SelectAdditional = "SelectAdditional";
        public const string Deselect = "Deselect";
        public const string Remove = "Remove";

        public static void Subscribe(string message, Action<string> handler)
        {
            if (!idMessageHandlers.ContainsKey(message))
            {
                idMessageHandlers[message] = handler;
            }
            else
            {
                idMessageHandlers[message] += handler;
            }
        }

        public static void Publish(string message, string id = null)
        {
            if (idMessageHandlers.TryGetValue(message, out var handler))
            {
                handler?.Invoke(id);
            }
        }

        public static void Subscribe(string message, Action<Shape, ShapeTreeViewItem> handler)
        {
            if (!shapeMessageHandlers.ContainsKey(message))
            {
                shapeMessageHandlers[message] = handler;
            }
            else
            {
                shapeMessageHandlers[message] += handler;
            }
        }

        public static void Publish(string message, Shape shape, ShapeTreeViewItem item)
        {
            if (shapeMessageHandlers.TryGetValue(message, out var handler))
            {
                handler?.Invoke(shape, item);
            }
        }
    }
}
