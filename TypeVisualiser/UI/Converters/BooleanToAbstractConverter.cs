using System.Windows;

namespace TypeVisualiser.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Properties;

    /// <summary>
    /// Converts boolean values IsAbstract and IsStatic to opacity. Gives the ghost effect on abstract classes.
    /// </summary>
    public class BooleanToAbstractConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return null;
            }

            if (values.Length < 2)
            {
                throw new ArgumentNullResourceException("values", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (values[0] == DependencyProperty.UnsetValue)
            {
                return null;
            }

            var isAbstract = (bool)values[0];
            var isStatic = (bool)values[1];
            if (isAbstract && !isStatic)
            {
                return 0.7;
            }

            return 1.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}