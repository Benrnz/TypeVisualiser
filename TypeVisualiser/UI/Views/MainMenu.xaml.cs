using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TypeVisualiser.Messaging;

namespace TypeVisualiser.UI.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
            MessagingGate.Register<RecentFileDeleteMessage>(this, OnDeleteRecentlyUsedFile);
            this.RecentlyUsedFiles.Loaded += OnRecentlyUsedFilesLoaded;
        }

        private void OnDeleteRecentlyUsedFile(RecentFileDeleteMessage message)
        {
            bool found = false;
            MenuItem menuItem = null;
            foreach (object item in this.RecentlyUsedFiles.Items)
            {
                menuItem = item as MenuItem;
                if (menuItem != null && menuItem.CommandParameter == message.Data)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                this.RecentlyUsedFiles.Items.Remove(menuItem);
            }
        }

        private void OnRecentlyUsedFilesLoaded(object sender, RoutedEventArgs e)
        {
            this.RecentlyUsedFiles.Items.SortDescriptions.Add(new SortDescription("When", ListSortDirection.Descending));
        }
    }
}