using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HierarchyTreeAndCanvasWPF.Converters
{
    internal class ActiveButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string shapeToAdd = value as string;
            string buttonName = parameter as string;

            // If shape to add matches button name, make button 'active'
            if (shapeToAdd == buttonName)
            {
                return new SolidColorBrush(Colors.Purple);
            }
            else
            {
                return new SolidColorBrush(Colors.Beige);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("convert back");
            return DependencyProperty.UnsetValue;
        }
    }
}
