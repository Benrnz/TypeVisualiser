namespace TypeVisualiser.Messaging
{
    using System.Windows;
    using Microsoft.Win32;

    internal class WindowsOpenFileDialog : IUserPromptOpenFile
    {
        public bool? AddExtension { get; set; }
        public bool? CheckFileExists { get; set; }
        public bool? CheckPathExists { get; set; }
        public string DefaultExt { get; set; }
        public string FileName { get; private set; }
        public string Filter { get; set; }
        public int? FilterIndex { get; set; }
        public string Title { get; set; }

        public bool? ShowDialog()
        {
            var dialog = new OpenFileDialog();
            if (AddExtension != null)
            {
                dialog.AddExtension = AddExtension.Value;
            }

            if (CheckFileExists != null)
            {
                dialog.CheckFileExists = CheckFileExists.Value;
            }

            if (CheckPathExists != null)
            {
                dialog.CheckPathExists = CheckPathExists.Value;
            }

            if (DefaultExt != null)
            {
                dialog.DefaultExt = DefaultExt;
            }

            if (Title != null)
            {
                dialog.Title = Title;
            }

            if (Filter != null)
            {
                dialog.Filter = Filter;
            }

            if (FilterIndex != null)
            {
                dialog.FilterIndex = FilterIndex.Value;
            }

            bool? result = dialog.ShowDialog(Application.Current.MainWindow);
            FileName = dialog.FileName;
            return result;
        }
    }
}