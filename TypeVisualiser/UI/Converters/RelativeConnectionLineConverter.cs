using System;
using System.Globalization;
using System.Windows.Data;
using TypeVisualiser.Properties;

namespace TypeVisualiser.UI.Converters
{
    /// <summary>
    /// This is required to convert absolute canvas co-ordinates to relative co-ordinates, given the connection line top left is set to the from location.
    /// This means From is always 0,0 and To needs to be calculated from the absolute co-ordinates given in the To point.
    /// This is done by subtracting To - From values.
    /// </summary>
    public class RelativeConnectionLineConverter : IMultiValueConverter 
    {
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                throw new ArgumentNullResourceException("values", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            // object[0] is expected to be the From.X value
            // object[1] is expected to be the To.X value
            return (double)values[1] - (double)values[0];
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// NOT SUPPORTED - will throw an exception.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
