using System.Windows.Input;

namespace TypeVisualiser.UI
{
    public static class ContextMenuCommands
    {
        static ContextMenuCommands()
        {
            AnnotateCanvasCommand = new RoutedCommand("Annotate here", typeof (ContextMenuCommands));
            NavigateToTypeCommand = new RoutedCommand("Navigate to type", typeof (ContextMenuCommands));
            AddToTrivialListCommand = new RoutedCommand("Add to trivial list", typeof (ContextMenuCommands));
            ShowAllCommand = new RoutedCommand("Show all", typeof (ContextMenuCommands));
            TemporarilyHideCommand = new RoutedCommand("Temporarily hide", typeof (ContextMenuCommands));
        }

        public static ICommand AddToTrivialListCommand { get; private set; }

        public static ICommand NavigateToTypeCommand { get; private set; }

        public static ICommand ShowAllCommand { get; private set; }

        public static ICommand TemporarilyHideCommand { get; private set; }

        public static ICommand AnnotateCanvasCommand { get; private set; }
    }
}