namespace TypeVisualiser.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Model;
    using Model.Persistence;

    public class TypeToBrushConverter : IValueConverter
    {
        internal const string ClassBrushKey = "ClassBlueBrush";
        internal const string EnumBrushKey = "EnumGoldBrush";
        internal const string InterfaceBrushKey = "InterfaceGreenBrush";
        internal const string SystemClassBrushKey = "SystemClassBlueBrush";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string brushName = ConvertToBrushName(value);
            if (string.IsNullOrWhiteSpace(brushName))
            {
                return null;
            }

            return ConvertStringToBrush(brushName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        internal static string ConvertToBrushName(object value)
        {
            var type = value as VisualisableType;
            if (type == null)
            {
                return null;
            }

            if (type.Modifiers.Kind == TypeKind.Interface)
            {
                return InterfaceBrushKey;
            }

            if (type.Modifiers.Kind == TypeKind.Enum)
            {
                return EnumBrushKey;
            }

            return GetClassBrush(type);
        }

        private static Brush ConvertStringToBrush(string brushName)
        {
            return Application.Current.Resources[brushName] as Brush;
        }

        private static string GetClassBrush(VisualisableType type)
        {
            if (type.AssemblyQualifiedName.StartsWith("System.", StringComparison.Ordinal))
            {
                return SystemClassBrushKey;
            }

            return ClassBrushKey;
        }
    }
}