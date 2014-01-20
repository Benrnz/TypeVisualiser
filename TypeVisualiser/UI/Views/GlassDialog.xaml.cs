namespace TypeVisualiser.UI.Views
{
    using System.Windows;

    /// <summary>
    /// Code behind for InfoDialog
    /// </summary>
    public partial class GlassDialog
    {
        /// <summary>
        /// Initializes a new instance of the InfoDialog class.
        /// </summary>
        public GlassDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string DefaultButtonCaption { get; private set; }
        public string DialogTitle { get; private set; }

        public string HeadingCaption { get; private set; }
        public string ImageSource { get; private set; }
        public string Message { get; private set; }

        public void ShowDialog(string title, string headingCaption, string message)
        {
            DialogTitle = title;
            Message = RationaliseMessage(message);
            DefaultButtonCaption = "Close";
            ImageSource = "../Assets/MainIcon.png";
            HeadingCaption = headingCaption;
            ShowDialog();
        }

        private void OnDefaultButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private static string RationaliseMessage(string message)
        {
            if (message.Length > 1024)
            {
                return message.Substring(0, 1024) + "\n---TRUNCATED---";
            }

            return message;
        }
    }
}