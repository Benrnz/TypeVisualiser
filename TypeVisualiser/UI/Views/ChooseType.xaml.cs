using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace TypeVisualiser.UI.Views
{
    /// <summary>
    /// Description for ChooseType.
    /// </summary>
    public partial class ChooseType
    {
        /// <summary>
        /// Initializes a new instance of the ChooseType class.
        /// </summary>
        public ChooseType()
        {
            InitializeComponent();
        }

        private ChooseTypeController Controller
        {
            get { return DataContext as ChooseTypeController; }
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(Close));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by WPF")]
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldController = e.OldValue as ChooseTypeController;
            if (oldController != null)
            {
                oldController.CloseRequested -= OnCloseRequested;
            }

            Controller.CloseRequested += OnCloseRequested;
        }

        private void OnListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Controller.SelectedItem == null)
            {
                return;
            }

            Controller.ChooseCommand.Execute(Controller.SelectedItem.FullyQualifiedName);
        }
    }
}