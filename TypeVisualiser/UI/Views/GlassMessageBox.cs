namespace TypeVisualiser.UI.Views
{
    using System;
    using System.Globalization;
    using Messaging;
    using Properties;

    internal class GlassMessageBox : MessageBoxBase
    {
        private static string Title
        {
            get
            {
                return Resources.ApplicationName;
            }
        }

        public override void Show(string message, string headingCaption = "")
        {
            var window = new GlassDialog();
            window.ShowDialog(Title, headingCaption, message);
        }

        public override void Show(string headingCaption, string format, object argument1, params object[] args)
        {
            var window = new GlassDialog();
            window.ShowDialog(Title, headingCaption, string.Format(CultureInfo.CurrentCulture, format, PrependElement(argument1, args)));
        }

        public override void Show(Exception ex, string message)
        {
            var window = new GlassDialog();
            window.ShowDialog(Title, string.Empty, message + "\n\n" + ex);
        }

        public override void Show(string format, object argument1, params object[] args)
        {
            var window = new GlassDialog();
            window.ShowDialog(Title, string.Empty, string.Format(CultureInfo.CurrentCulture, format, PrependElement(argument1, args)));
        }

        public override void Show(Exception ex, string format, object argument1, params object[] args)
        {
            var window = new GlassDialog();
            window.ShowDialog(Title, string.Empty, string.Format(CultureInfo.CurrentCulture, format, PrependElement(argument1, args)));
        }
    }
}