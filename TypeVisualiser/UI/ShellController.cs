using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using StructureMap;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.Properties;
using TypeVisualiser.RecentFiles;
using TypeVisualiser.Startup;
using TypeVisualiser.UI.Views;
using TypeVisualiser.UI.WpfUtilities;

namespace TypeVisualiser.UI
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class ShellController : TypeVisualiserControllerBase
    {
        private const int LoadingProgressMaximum = 29;
        private readonly IContainer factory;
        private readonly IFileManager fileManager;
        private bool connectorTypeDirect;
        private bool connectorTypeSnap;
        private Diagram currentView;
        private bool debugMode;
        private TypeVisualiserLayoutFile diagramFile;
        private bool isLoading;
        private int loadingProgress;
        private Timer progressTimer;
        private List<string> queuedEvents = new List<string>();
        private string title;

        public ShellController() : this(IoC.Default)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
            Justification = "Reviewed and acceptable here, VerifyPropertyName and RaisePropertyChanged are not going to create unwanted side affects")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Calling dipose on a controller is not possible within the scope of one method, it is bound to the UI.")]
        public ShellController(IContainer factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullResourceException("factory", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            MessagingGate.Register(this, new Action<ChooseAssemblyMessage>(x => ChooseAssemblyExecute()));
            MessagingGate.Register(this, new Action<ShutdownMessage>(x => Cleanup()));
            MessagingGate.Register(this, new Action<NavigateToDiagramAssociationMessage>(OnNavigateToDiagramAssociation));

            this.factory = factory;
            ConnectorTypeDirect = true;
            this.fileManager = factory.GetInstance<IFileManager>();
            this.fileManager.Initialise();
            Current = this;
            OpenViews = new ObservableCollection<Diagram>();
            OnLoadDemoTypeExecute();
        }

        public static ShellController Current { get; private set; }

        public ICommand CentreCommand
        {
            get { return new RelayCommand(OnCentreExecute, () => !IsLoading); }
        }

        public ICommand ChooseTypeCommand
        {
            get { return new RelayCommand(OnChooseTypeExecute, () => !IsLoading); }
        }

        public ChooseTypeController ChooseTypeController { get; private set; }

        public ICommand ComingSoonCommand
        {
            get { return new RelayCommand(OnComingSoonExecute, () => !IsLoading); }
        }

        public bool ConnectorTypeDirect
        {
            get { return this.connectorTypeDirect; }

            set
            {
                this.connectorTypeDirect = value;
                VerifyPropertyName("ConnectorTypeDirect");
                RaisePropertyChanged("ConnectorTypeDirect");
                if (value)
                {
                    var connectorBuilder = this.factory.TryGetInstance<IConnectorBuilder>(ConnectorType.Direct.ToString());
                    ConnectionLine.ConnectorBuilder = connectorBuilder ?? new DirectLineConnectorBuilder();
                    ConnectorTypeSnap = false;
                    if (CurrentView != null)
                    {
                        var controller = CurrentView.Controller as IDiagramCommandsNeedsRefactor;
                        if (controller != null)
                        {
                            controller.DrawConnectingLines();
                        }
                    }
                }
            }
        }

        public bool ConnectorTypeSnap
        {
            get { return this.connectorTypeSnap; }

            set
            {
                this.connectorTypeSnap = value;
                VerifyPropertyName("ConnectorTypeSnap");
                RaisePropertyChanged("ConnectorTypeSnap");
                if (value)
                {
                    ConnectionLine.ConnectorBuilder = this.factory.GetInstance<IConnectorBuilder>(ConnectorType.Snap.ToString());
                    ConnectorTypeDirect = false;
                    if (CurrentView != null)
                    {
                        var controller = CurrentView.Controller as IDiagramCommandsNeedsRefactor;
                        if (controller != null)
                        {
                            controller.DrawConnectingLines();
                        }
                    }
                }
            }
        }

        public Diagram CurrentView
        {
            get { return this.currentView; }

            set
            {
                if (value != this.currentView)
                {
                    this.currentView = value;
                    if (this.currentView != null)
                    {
                        Title = this.currentView.FullName;
                    }

                    VerifyPropertyName("CurrentView");
                    RaisePropertyChanged("CurrentView");
                    if (this.currentView != null)
                    {
                        this.currentView.Controller.ActivateDiagram();
                    }
                }
            }
        }

        public bool DebugMode
        {
            get { return this.debugMode; }
            set
            {
                this.debugMode = value;
                VerifyPropertyName("DebugMode");
                RaisePropertyChanged("DebugMode");
            }
        }

        public ICommand EditTrivialExcludeListCommand
        {
            get { return new RelayCommand(OnEditTrivialExcludeListExecute, () => !IsLoading); }
        }

        public ICommand ExitCommand
        {
            get { return new RelayCommand(Application.Current.Shutdown, () => !IsLoading); }
        }

        public ICommand HelpAboutCommand
        {
            get { return new RelayCommand(OnHelpAboutExecute, () => !IsLoading); }
        }

        public ICommand HideBackgroundCommand
        {
            get { return new RelayCommand(OnHideBackgroundExecute, () => !IsLoading); }
        }

        public bool HideSecondaryAssociations
        {
            get
            {
                return this.factory.GetInstance<ITrivialFilter>().HideSecondaryAssociations;
            }

            set
            {
                this.factory.GetInstance<ITrivialFilter>().HideSecondaryAssociations = value;
                VerifyPropertyName("HideSecondaryAssociations");
                RaisePropertyChanged("HideSecondaryAssociations");
            }
        }

        public ICommand HideSecondaryAssociationsCommand
        {
            get { return new RelayCommand(OnHideSecondaryAssociations, () => !IsLoading); }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reqd by WPF binding")]
        public bool HideSystemTypes
        {
            get { return this.factory.GetInstance<ITrivialFilter>().HideSystemTypes; }

            set
            {
                this.factory.GetInstance<ITrivialFilter>().HideSystemTypes = value;
                VerifyPropertyName("HideSystemTypes");
                RaisePropertyChanged("HideSystemTypes");
            }
        }

        public ICommand HideSystemTypesCommand
        {
            get { return new RelayCommand(OnHideSystemTypesExecute, () => !IsLoading); }
        }

        public ICommand HideTrivialExcludeCommand
        {
            get { return new RelayCommand(OnHideTrivialExcludeExecute, () => !IsLoading); }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reqd by WPF binding")]
        public bool HideTrivialTypes
        {
            get { return this.factory.GetInstance<ITrivialFilter>().HideTrivialTypes; }

            set
            {
                this.factory.GetInstance<ITrivialFilter>().HideTrivialTypes = value;
                VerifyPropertyName("HideTrivialTypes");
                RaisePropertyChanged("HideTrivialTypes");
            }
        }

        public bool IsLoading
        {
            get { return this.isLoading; }

            set
            {
                this.isLoading = value;
                VerifyPropertyName("IsLoading");
                RaisePropertyChanged("IsLoading");
            }
        }

        public ICommand LoadDiagramCommand
        {
            get { return new RelayCommand(OnLoadDiagramExecute, () => !IsLoading); }
        }

        public ICommand LoadRecentFileCommand
        {
            get { return new RelayCommand<RecentFile>(OnLoadRecentFileExecute, _ => !IsLoading); }
        }

        public int LoadingProgress
        {
            get { return this.loadingProgress; }

            set
            {
                this.loadingProgress = value;
                VerifyPropertyName("LoadingProgress");
                RaisePropertyChanged("LoadingProgress");
            }
        }

        public ObservableCollection<Diagram> OpenViews { get; private set; }

        public ObservableCollection<RecentFile> RecentFiles
        {
            get { return this.fileManager.RecentFiles.RecentlyUsedFiles; }
        }

        public ICommand RefreshCommand
        {
            get { return new RelayCommand(OnRefreshExecute, () => !IsLoading); }
        }

        public ICommand SaveDiagramCommand
        {
            get { return new RelayCommand(OnSaveDiagramExecute, () => !IsLoading); }
        }

        public ICommand ShowDemoTypeCommand
        {
            get { return new RelayCommand(OnLoadDemoTypeExecute, () => !IsLoading); }
        }

        public ICommand TabCloseCommand
        {
            get { return new RelayCommand<Diagram>(OnTabCloseExecute, _ => !IsLoading); }
        }

        public string Title
        {
            get { return string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", Resources.ApplicationName, string.IsNullOrWhiteSpace(this.title) ? string.Empty : " - ", this.title); }

            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    VerifyPropertyName("Title");
                    RaisePropertyChanged("Title");
                }
            }
        }

        public ICommand ZoomCommand
        {
            get { return new RelayCommand<double>(OnZoomExecute, _ => !IsLoading); }
        }

        public ICommand ZoomToFitCommand
        {
            get { return new RelayCommand(OnZoomToFitExecute, () => !IsLoading); }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not possible to wrap into a using block. Is disposed by OnTabCloseExecute")]
        protected virtual Diagram CreateDiagram()
        {
            return new Diagram(new ViewportController(this.factory));
        }

        protected override void RaisePropertyChanged(string propertyName)
        {
            if (propertyName != "IsLoading" && IsLoading && propertyName != "LoadingProgress")
            {
                this.queuedEvents.Add(propertyName);
                return;
            }

            base.RaisePropertyChanged(propertyName);
        }

        private void ChooseAssemblyExecute()
        {
            this.fileManager.ChooseAssembly();
            OnChooseTypeExecute();
        }

        private void OnCentreExecute()
        {
            CurrentView.CentreDiagram();
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Calling dipose on a controller is not possible within the scope of one method, it is bound to the UI.")]
        private void OnChooseTypeChosenFromDialog(Type type)
        {
            if (type == null)
            {
                return;
            }

            SetLoadingStatus(true);
            Task<IVisualisableTypeWithAssociations> task = this.fileManager.LoadTypeAsync(type);
            if (task == null)
            {
                SetLoadingStatus(false);
                return;
            }

            if (task.IsCompleted)
            {
                OnTypeFinishedLoading(task.Result);
            } else
            {
                task.ContinueWith(finishedTask => OnTypeFinishedLoading(finishedTask.Result));
            }
        }

        private void OnChooseTypeExecute()
        {
            Assembly assembly = this.fileManager.LoadAssembly();
            if (assembly == null)
            {
                // Cancel clicked by user
                return;
            }

            ChooseTypeController = new ChooseTypeController(assembly, OnChooseTypeChosenFromDialog);
            var chooseTypeDialog = new ChooseType { DataContext = ChooseTypeController, ResizeMode = ResizeMode.NoResize };
            bool? result = chooseTypeDialog.ShowDialog();
            Logger.Instance.WriteEntry("Loading types from DLL for selection. ShowDiaglog returned {0}", result);
        }

        private void OnComingSoonExecute()
        {
            UserPrompt.Show(Resources.ShellController_ComingSoonExecute_Coming_soon_in_a_future_version);
        }

        private void OnDiagramFinishedLoading(object sender, EventArgs e)
        {
            var diagram = sender as Diagram;
            if (diagram == null)
            {
                return;
            }

            diagram.Controller.DiagramLoaded -= OnDiagramFinishedLoading;
            if (diagram.Id != CurrentView.Id)
            {
                return;
            }

            OnZoomToFitExecute();
            SetLoadingStatus(false);
        }

        private void OnDiagramFinishedLoadingFromFile(object sender, EventArgs e)
        {
            var diagram = sender as Diagram;
            if (diagram == null)
            {
                return;
            }

            diagram.Controller.DiagramLoaded -= OnDiagramFinishedLoadingFromFile;
            if (diagram.Id != CurrentView.Id)
            {
                return;
            }

            OnZoomToFitExecute();
            CurrentView.Controller.LoadDiagramDataFromFile(this.diagramFile);
            this.diagramFile = null;
            SetLoadingStatus(false);
        }

        private void OnEditTrivialExcludeListExecute()
        {
            this.factory.GetInstance<ITrivialFilter>().EditTrivialList();
        }

        private void OnHelpAboutExecute()
        {
            Version version = GetType().Assembly.GetName().Version;
            DateTime datetime = File.GetLastWriteTime(GetType().Assembly.Location);
            UserPrompt.Show("About TypeVisualiser", "Version {0}.{1}.{2}.{3}\nLast Modified Date:\n{4}\nDeveloped by Ben Rees", version.Major, version.Minor, version.Build, version.Revision, datetime);
        }

        private void OnHideBackgroundExecute()
        {
            CurrentView.Controller.HideBackground = !CurrentView.Controller.HideBackground;
        }

        private void OnHideSecondaryAssociations()
        {
            var controllerCommands = CurrentView.Controller as IDiagramCommandsNeedsRefactor;
            if (controllerCommands != null)
            {
                controllerCommands.HideSecondaryAssociations();
            }
        }

        private void OnHideSystemTypesExecute()
        {
            var controllerCommands = CurrentView.Controller as IDiagramCommandsNeedsRefactor;
            if (controllerCommands != null)
            {
                controllerCommands.HideSystemTypes();
            }
        }

        private void OnHideTrivialExcludeExecute()
        {
            var controllerCommands = CurrentView.Controller as IDiagramCommandsNeedsRefactor;
            if (controllerCommands != null)
            {
                controllerCommands.HideTrivialTypes();
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Calling dipose on a controller is not possible within the scope of one method, it is bound to the UI.")]
        private void OnLoadDemoTypeExecute()
        {
            IVisualisableTypeWithAssociations subject = this.fileManager.LoadDemoType();
            Dispatcher.BeginInvoke(() =>
                {
                    Diagram diagram = CreateDiagram();
                    OpenViews.Add(diagram);
                    CurrentView = diagram;
                    diagram.Controller.DiagramLoaded += OnDiagramFinishedLoading;
                    CurrentView.Controller.AssignDiagramData(subject);
                },
                                   DispatcherPriority.Normal);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Calling dipose on a controller is not possible within the scope of one method, it is bound to the UI.")]
        private void OnLoadDiagramExecute()
        {
            this.diagramFile = this.fileManager.LoadDiagram();
            if (this.diagramFile == null)
            {
                // Cancel clicked by user
                return;
            }

            SetLoadingStatus(true);
            Task<IVisualisableTypeWithAssociations> task = this.fileManager.LoadFromDiagramFileAsync(this.diagramFile);
            if (task == null)
            {
                SetLoadingStatus(false);
                return;
            }

            if (task.IsCompleted)
            {
                OnTypeFinishedLoadingFromFile(task.Result);
            } else
            {
                task.ContinueWith(finishedTask => OnTypeFinishedLoadingFromFile(finishedTask.Result));
            }

            HideTrivialTypes = this.diagramFile.HideTrivialTypes;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Calling dipose on a controller is not possible within the scope of one method, it is bound to the UI.")]
        private void OnLoadRecentFileExecute(RecentFile recentFileData)
        {
            SetLoadingStatus(true);
            Task<IVisualisableTypeWithAssociations> task = this.fileManager.LoadFromRecentFileAsync(recentFileData);
            if (task == null)
            {
                SetLoadingStatus(false);
                return;
            }

            if (task.IsCompleted)
            {
                OnTypeFinishedLoading(task.Result);
            } else
            {
                task.ContinueWith(finishedTask => OnTypeFinishedLoading(finishedTask.Result));
            }
        }

        private void OnNavigateToDiagramAssociation(NavigateToDiagramAssociationMessage message)
        {
            // This means the user has right clicked a diagram element and request to open a new tab with to diagram the indicated element.
            SetLoadingStatus(true);
            Task<IVisualisableTypeWithAssociations> task = this.fileManager.LoadFromVisualisableTypeAsync(message.DiagramType);
            if (task == null)
            {
                SetLoadingStatus(false);
                return;
            }

            if (task.IsCompleted)
            {
                OnTypeFinishedLoading(task.Result);
            } else
            {
                task.ContinueWith(finishedTask => OnTypeFinishedLoading(finishedTask.Result));
            }
        }

        private void OnRefreshExecute()
        {
            if (!CurrentView.IsLoaded)
            {
                return;
            }

            SetLoadingStatus(true);
            CurrentView.Controller.Refresh(this.fileManager);
            SetLoadingStatus(false);
        }

        private void OnSaveDiagramExecute()
        {
            IPersistentFileData saveData = CurrentView.Controller.GatherPersistentData();
            saveData.HideTrivialTypes = HideTrivialTypes;
            this.fileManager.SaveDiagram(saveData);
        }

        private void OnTabCloseExecute(Diagram diagram)
        {
            if (OpenViews.Count == 1)
            {
                ExitCommand.Execute(null);
                return;
            }

            if (OpenViews.Contains(diagram))
            {
                OpenViews.Remove(diagram);
            }

            if (CurrentView == diagram)
            {
                CurrentView = OpenViews.First();
            }

            diagram.Controller.Cleanup();
            var disposable = diagram.Controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        private void OnTypeFinishedLoading(IVisualisableTypeWithAssociations subject)
        {
            OnTypeFinishedLoading(subject, OnDiagramFinishedLoading);
        }

        private void OnTypeFinishedLoading(IVisualisableTypeWithAssociations subject, EventHandler diagramFinishedLoadingHandler)
        {
            // At this point the expensive work is done. The VisualisableTypes have been constructed, all that remains is dumping them onto the diagram surface. 
            // This is done visually and the progress screen does not need to be displayed.
            Dispatcher.BeginInvoke(() =>
                {
                    // Ensure UI bound properties set by UI thread.
                    Diagram diagram = CreateDiagram();
                    OpenViews.Add(diagram);
                    CurrentView = diagram;
                    CurrentView.Controller.DiagramLoaded += diagramFinishedLoadingHandler;
                    ResetFilters();
                    CurrentView.Controller.AssignDiagramData(subject);
                    Title = CurrentView.FullName;
                    LoadingProgress = LoadingProgressMaximum;
                },
                                   DispatcherPriority.Normal);

            // Ensure this is at back of queue.
            Dispatcher.BeginInvoke(() => SetLoadingStatus(false), DispatcherPriority.ContextIdle);
        }

        private void OnTypeFinishedLoadingFromFile(IVisualisableTypeWithAssociations subject)
        {
            OnTypeFinishedLoading(subject, OnDiagramFinishedLoadingFromFile);
        }

        private void OnZoomExecute(double scaleFactor)
        {
            CurrentView.ContentScale = scaleFactor;
        }

        private void OnZoomToFitExecute()
        {
            CurrentView.ZoomToFit();
        }

        private void ResetFilters()
        {
            HideSystemTypes = false;
            HideTrivialTypes = false;
        }

        [SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "Recommended to only tick <1 per second. This timer only ticks once ever, so is ok."
            )]
        private void SetLoadingStatus(bool begin)
        {
            if (begin)
            {
                Mouse.SetCursor(Cursors.Wait);
                IsLoading = true;
                LoadingProgress = 1;
                this.progressTimer = new Timer(500);
                this.progressTimer.Elapsed += UpdateLoadingProgress;
                this.progressTimer.Start();
            } else
            {
                if (this.progressTimer == null)
                {
                    return;
                }

                this.progressTimer.Elapsed -= UpdateLoadingProgress;
                this.progressTimer.Stop();
                this.progressTimer.Close();
                this.progressTimer = null;
                Mouse.SetCursor(Cursors.Arrow);
                IsLoading = false;
                this.queuedEvents.ForEach(RaisePropertyChanged);
                this.queuedEvents = new List<string>();
            }
        }

        private void UpdateLoadingProgress(object sender, ElapsedEventArgs e)
        {
            // Ensure high priority on the dispatcher pipeline.
            Dispatcher.BeginInvoke(() => LoadingProgress = (LoadingProgress + 1) % LoadingProgressMaximum, DispatcherPriority.Send);
        }
    }
}