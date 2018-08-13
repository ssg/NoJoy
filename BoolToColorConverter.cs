using System;
using System.Globalization;
using System.Windows.Data;

namespace NoJoy
{
    class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return boolValue ? "#00DD00" : "Gray";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Green" ? true : false;
        }
    }
}