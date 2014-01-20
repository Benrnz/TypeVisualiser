using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.UI.WpfUtilities;

namespace TypeVisualiser.UI
{
    public class ChooseTypeController : TypeVisualiserControllerBase
    {
        private readonly Assembly assembly;
        // ReSharper disable NotAccessedField.Local
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used in testing")] private readonly Task loadTypesTask; // Used in unit testing.
        private readonly Action<Type> onTypeChosen;
        private string searchText;
        // ReSharper restore NotAccessedField.Local

        private SelectableType selectedItem;

        public ChooseTypeController(Assembly assemblyToLoadFrom, Action<Type> onTypeChosen)
        {
            this.onTypeChosen = onTypeChosen;
            this.assembly = assemblyToLoadFrom;
            Types = new ObservableCollection<SelectableType>();
            this.loadTypesTask = Task.Factory.StartNew(LoadTypes);
            this.loadTypesTask.ContinueWith(TypesLoaderErrorHandler);
        }

        public event EventHandler CloseRequested;

        public ICommand ChangeAssemblyCommand
        {
            get { return new RelayCommand(ChangeAssemblyExecuted); }
        }

        public ICommand ChooseCommand
        {
            get { return new RelayCommand<string>(TypeChosenExecuted); }
        }

        public string DialogTitle
        {
            get { return "Choose a type from " + this.assembly.FullName; }
        }

        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                this.searchText = value;
                VerifyPropertyName("SearchText");
                RaisePropertyChanged("SearchText");
                ApplyFilter();
            }
        }

        public SelectableType SelectedItem
        {
            get { return this.selectedItem; }

            set
            {
                this.selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        public ObservableCollection<SelectableType> Types { get; private set; }

        public void ApplyFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Types);
            view.Filter = text =>
                {
                    if (string.IsNullOrWhiteSpace(SearchText) || text == null)
                    {
                        return true;
                    }
                    var selectableType = text as SelectableType;
                    if (selectableType == null)
                    {
                        return true;
                    }

                    return selectableType.FullyQualifiedName.Contains(SearchText);
                };
        }

        private static string GetImagePath(Type type)
        {
            if (type.IsPublic)
            {
                if (type.IsClass)
                {
                    return "../Assets/PublicClass.png";
                }

                if (type.IsEnum)
                {
                    return "../Assets/PublicEnum.png";
                }

                if (type.IsInterface)
                {
                    return "../Assets/PublicInterface.png";
                }
            }

            if (type.IsEnum)
            {
                return "../Assets/InternalEnum.png";
            }

            if (type.IsClass || type.IsValueType)
            {
                return "../Assets/InternalClass.png";
            }

            if (type.IsInterface)
            {
                return "../Assets/InternalInterface.png";
            }

            return null;
        }

        private void ChangeAssemblyExecuted()
        {
            Messenger.Send(new ChooseAssemblyMessage());
            RequestClose();
        }

        private void LoadTypes()
        {
            IEnumerable<Type> types = this.assembly.GetTypes().OrderBy(t => t.FullName);

            foreach (Type type in types)
            {
                string name = type.Name.Contains("`") ? new TypeDescriptorHelper(type).GenerateName() : type.Name;

                Type type1 = type;
                string imagePath = GetImagePath(type);
                Dispatcher.BeginInvoke(() => Types.Add(new SelectableType { FullName = type1.Namespace + "." + name, FullyQualifiedName = type1.FullName, ImagePath = imagePath }),
                                       DispatcherPriority.Normal);
            }
        }

        private void RequestClose()
        {
            EventHandler handler = CloseRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void TypeChosenExecuted(string typeName)
        {
            Type type = this.assembly.GetType(typeName);
            this.onTypeChosen(type);
            RequestClose();
        }

        private void TypesLoaderErrorHandler(Task completedLoadTypesTask)
        {
            if (!completedLoadTypesTask.IsFaulted || completedLoadTypesTask.Exception == null)
            {
                return;
            }

            var typeLoadEx = completedLoadTypesTask.Exception.InnerExceptions[0] as ReflectionTypeLoadException;
            string errorMessage;
            if (typeLoadEx != null)
            {
                var builder = new StringBuilder();
                foreach (Exception e in typeLoadEx.LoaderExceptions)
                {
                    builder.Append(e + "\n");
                }

                errorMessage = builder.ToString();
            } else
            {
                errorMessage = "Unkown error occured attempting to list all types from assembly.\n" + completedLoadTypesTask.Exception.InnerExceptions[0].Message;
            }

            Dispatcher.BeginInvoke(() =>
                {
                    UserPrompt.Show(errorMessage);
                    RequestClose();
                },
                                   DispatcherPriority.Normal);
        }
    }
}