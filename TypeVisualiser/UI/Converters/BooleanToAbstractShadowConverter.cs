namespace TypeVisualiser.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanToAbstractShadowConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0 || values[0] == DependencyProperty.UnsetValue)
            {
                return null;
            }

            var isAbstract = (bool)values[0];
            if (values.Length > 1)
            {
                 var isStatic = (bool)values[1];
                 if (isAbstract && !isStatic)
                 {
                     return 0.0;
                 }

                 return 1.0;
            }

            return isAbstract ? 1.0 : 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
