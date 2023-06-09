﻿using BasicShapeEditorWPF.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace BasicShapeEditorWPF.ViewModels
{
    public interface IShapeCanvasViewModel
    {
        public ObservableCollection<ShapeTreeViewItem> TreeItems { get; set; }
        public ObservableCollection<Shape> CanvasShapes { get; set; }
        public ObservableCollection<Shape> SelectedCanvasShapes { get; set; }
        public string ActiveTool { get; }
        public Shape AddShapeToCanvas(string shapeName, Canvas canvas);
    }
}
