namespace TypeVisualiser.UI.Views
{
    using System.Windows;
    using Model;

    /// <summary>
    /// Code behind for UsageDialog
    /// </summary>
    public partial class UsageDialog
    {
        /// <summary>
        /// Initializes a new instance of the UsageDialog class.
        /// </summary>
        public UsageDialog()
        {
            InitializeComponent();
        }

        public void ShowDialog(string title, string subjectName, string subjectType, FieldAssociation association)
        {
            DataContext = new UsageDialogController(title, subjectName, subjectType, association);
            ShowDialog();
        }

        private void OnDefaultButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}