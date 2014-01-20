﻿namespace TypeVisualiser.UI.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;

    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if (DEBUG)
            Debugger.Break();
#endif
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if (DEBUG)
            Debugger.Break();
#endif
            return value;
        }
    }
}
