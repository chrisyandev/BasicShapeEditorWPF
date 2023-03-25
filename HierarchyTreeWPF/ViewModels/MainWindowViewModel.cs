using HierarchyTreeWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace HierarchyTreeWPF.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            TreeItems = new ObservableCollection<TreeItem>
            {
                new TreeItem(0, "Square"),
                new TreeItem(1, "Triangle")
            };
            TreeItems[1].Items.Add(new TreeItem(2, "Circle"));

            CanvasItems = new ObservableCollection<ShapeViewModelBase>();
        }

        public ObservableCollection<TreeItem> TreeItems { get; set; }

        public ObservableCollection<ShapeViewModelBase> CanvasItems { get; set; }
    }
}
