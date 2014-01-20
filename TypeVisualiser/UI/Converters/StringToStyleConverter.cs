using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TypeVisualiser.UI.Converters
{
    public class StringToStyleConverter : IValueConverter
    {
        private static Style defaultStyle;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var styleName = value as string;
            if (string.IsNullOrWhiteSpace(styleName))
            {
                return GetDefaultStyle();
            }

            var style = Application.Current.Resources[styleName] as Style;
            return style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static Style GetDefaultStyle()
        {
            if (defaultStyle != null)
            {
                return defaultStyle;
            }

            defaultStyle = new Style();
            defaultStyle.Setters.Add(new Setter(Shape.StrokeProperty, new SolidColorBrush(Colors.Black)));
            return defaultStyle;
        }
    }
}