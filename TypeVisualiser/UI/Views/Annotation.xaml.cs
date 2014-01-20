using System.Windows;
using System.Windows.Input;

namespace TypeVisualiser.UI.Views
{
    /// <summary>
    /// Interaction logic for Annotation.xaml
    /// </summary>
    public partial class Annotation
    {
        public static readonly RoutedEvent EditRequestedEvent = EventManager.RegisterRoutedEvent("EditRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Annotation));

        public Annotation()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler EditRequested
        {
            add { AddHandler(EditRequestedEvent, value); }
            remove { RemoveHandler(EditRequestedEvent, value); }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var newEventArgs = new RoutedEventArgs(EditRequestedEvent);
            RaiseEvent(newEventArgs);
        }
    }
}