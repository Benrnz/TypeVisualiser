namespace TypeVisualiser.UI.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
        }

        private ShellController Controller
        {
            get
            {
                return DataContext as ShellController;
            }
        }

        private void OnTabCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button == null)
            {
                return;
            }

            object diagram = button.CommandParameter;
            if (diagram == null)
            {
                return;
            }

            e.Handled = true;
            Controller.TabCloseCommand.Execute(diagram);
        }
    }
}