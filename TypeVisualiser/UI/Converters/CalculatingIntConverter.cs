namespace TypeVisualiser.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class CalculatingIntConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var intValue = (int)value;
            if (intValue < 0)
            {
                return "Calculating...";
            }

            return intValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
