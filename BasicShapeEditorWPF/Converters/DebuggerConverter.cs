using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BasicShapeEditorWPF.Converters
{
    public class DebuggerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("DebuggerConverter >> Convert()");
            // Set breakpoint here
            if (value is IEnumerable && value is not string)
            {
                foreach (var item in value as IEnumerable)
                {
                    Debug.WriteLine(item);
                }
            }
            else
            {
                Debug.WriteLine(value);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("DebuggerConverter >> ConvertBack()");
            // Set breakpoint here
            if (value is IEnumerable && value is not string)
            {
                foreach (var item in value as IEnumerable)
                {
                    Debug.WriteLine(item);
                }
            }
            else
            {
                Debug.WriteLine(value);
            }

            return value;
        }
    }
}
