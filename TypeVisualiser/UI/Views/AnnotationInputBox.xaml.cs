using System.Windows;

namespace TypeVisualiser.UI.Views
{
    /// <summary>
    /// Interaction logic for AnnotationInputBox.xaml
    /// </summary>
    public partial class AnnotationInputBox 
    {
        public AnnotationInputBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string InputText { get; set; }

        public void OnOkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void OnClearClick(object sender, RoutedEventArgs e)
        {
            InputText = string.Empty;
            Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by WPF")]
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
        }
    }
}