namespace TypeVisualiser.UI.WpfUtilities
{
    using System.Windows.Threading;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Messaging;
    using Messaging;

    public class TypeVisualiserControllerBase : ViewModelBase
    {
        private readonly Dispatcher doNotUseDispatcher;
// ReSharper disable FieldCanBeMadeReadOnly.Local
        // Required for testing
        private IMessenger doNotUseMessenger = GalaSoft.MvvmLight.Messaging.Messenger.Default;
// ReSharper restore FieldCanBeMadeReadOnly.Local
        private IUserPromptMessage doNotUseUserPrompt;

        public TypeVisualiserControllerBase()
        {
            // This relies on the Xaml being responsible for instantiating the controller.
            this.doNotUseDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected Dispatcher Dispatcher
        {
            get
            {
                return this.doNotUseDispatcher;
            }
        }

        protected IMessenger Messenger
        {
            get
            {
                return this.doNotUseMessenger;
            }
        }

        protected IUserPromptMessage UserPrompt
        {
            get
            {
                return this.doNotUseUserPrompt ?? (this.doNotUseUserPrompt = new WindowsMessageBox());
            }

            set
            {
                this.doNotUseUserPrompt = value;
            }
        }
    }
}