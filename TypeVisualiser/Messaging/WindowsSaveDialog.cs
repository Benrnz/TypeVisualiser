namespace TypeVisualiser.Messaging
{
    using System.Windows;
    using Microsoft.Win32;

    internal class WindowsSaveDialog : IUserPromptSaveFile
    {
        public bool? AddExtension { get; set; }
        public string DefaultExt { get; set; }
        public string FileName { get; private set; }
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }

        public bool? ShowDialog()
        {
            var dialog = new SaveFileDialog();
            if (AddExtension != null)
            {
                dialog.AddExtension = AddExtension.Value;
            }

            if (DefaultExt != null)
            {
                dialog.DefaultExt = DefaultExt;
            }

            if (InitialDirectory != null)
            {
                dialog.InitialDirectory = InitialDirectory;
            }

            if (Filter != null)
            {
                dialog.Filter = Filter;
            }

            bool? result = dialog.ShowDialog(Application.Current.MainWindow);
            FileName = dialog.FileName;
            return result;
        }
    }
}