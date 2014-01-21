namespace TypeVisualiser.Messaging
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Properties;
    
    internal class WindowsMessageBox : MessageBoxBase
    {
        public override void Show(string message, string headingCaption = "")
        {
            // Ensure on the UI thread.
            string content = string.Format(
                CultureInfo.CurrentCulture,
                "{0}{1}{2}",
                headingCaption,
                string.IsNullOrWhiteSpace(headingCaption) ? string.Empty : "\n\n",
                message);
            MessageBox.Show(Application.Current.MainWindow, content, Resources.ApplicationName);
        }

        public override void Show(Exception ex, string message)
        {
            if (ex == null)
            {
                Show(message);
                return;
            }

            string exText = ex.ToString();
            if (exText.Length > 400)
            {
                exText = exText.Substring(0, 400);
            }

            Show(message + "\n\n" + exText);
        }

        public override void Show(string format, object argument1, params object[] args)
        {
            Show(string.Format(CultureInfo.CurrentCulture, format, PrependElement(argument1, args)));
        }

        public override void Show(string headingCaption, string format, object argument1, params object[] args)
        {
            Show(string.Format(CultureInfo.CurrentCulture, format, PrependElement(argument1, args)), headingCaption);
        }

        public override void Show(Exception ex, string format, object argument1, params object[] args)
        {
            Show(ex, string.Format(CultureInfo.CurrentCulture, format, PrependElement(argument1, args)));
        }
    }
}