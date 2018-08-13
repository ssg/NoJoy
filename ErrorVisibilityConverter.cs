using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NoJoy
{
    public class ErrorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errorMessage = (string)value;

            return string.IsNullOrEmpty(errorMessage)
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;

            return visibility == Visibility.Visible
                ? "ERROR"
                : null;
        }
    }
}