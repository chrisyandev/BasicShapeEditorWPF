using HierarchyTreeAndCanvasWPF.Controls;
using HierarchyTreeAndCanvasWPF.Utilities;
using HierarchyTreeAndCanvasWPF.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HierarchyTreeAndCanvasWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShapeCanvas _shapeCanvas;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _shapeCanvas = UIHelper.FindChild<ShapeCanvas>(this, "shapeCanvas");
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                _shapeCanvas.RemoveSelectedShapes();
            }
            else if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _shapeCanvas.SelectAllShapes();
            }
        }
    }
}
