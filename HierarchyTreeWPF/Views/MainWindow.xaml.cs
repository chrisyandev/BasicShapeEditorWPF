using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HierarchyTreeWPF.Extensions;
using HierarchyTreeWPF.Models;
using HierarchyTreeWPF.ViewModels;

namespace HierarchyTreeWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CanvasLeftClicked(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("canvas clicked");

            Canvas canvas = sender as Canvas;

            Rectangle newRectangle = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.Red
            };

            CanvasItem newCanvasItem = new CanvasItem
            {
                Id = 1,
                Shape = newRectangle,
                X = Mouse.GetPosition(canvas).X,
                Y = Mouse.GetPosition(canvas).Y
            };

            RectangleViewModel newRectangleViewModel = new(newCanvasItem);

            (DataContext as MainWindowViewModel).CanvasItems.Add(newRectangleViewModel);

        }
    }
}
