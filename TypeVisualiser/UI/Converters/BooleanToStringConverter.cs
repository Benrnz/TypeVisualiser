namespace TypeVisualiser.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            return bool.Parse(value.ToString());
        }
    }
}
