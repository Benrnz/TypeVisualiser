using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using StructureMap;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.Properties;
using TypeVisualiser.Startup;
using TypeVisualiser.UI.Views;
using TypeVisualiser.UI.WpfUtilities;

namespace TypeVisualiser.UI
{
    public class ViewportController : TypeVisualiserControllerBase, IDiagramController, IDiagramCommandsNeedsRefactor
    {
        private readonly ObservableCollection<DiagramElement> diagramElements = new ObservableCollection<DiagramElement>();
        private bool clean;
        private IVisualisableTypeWithAssociations currentSubject;
        private IContainer doNotUseFactory;
        private ClassUmlDrawingEngine drawingEngine;
        private ViewportControllerFilter filter;
        private bool hideBackground;
        private Diagram hostDiagram;

        private DiagramElement subjectDiagramElement;

        public ViewportController(IContainer factory)
        {
            this.doNotUseFactory = factory;
        }

        /// <summary>
        /// Raised when the diagram has finished loading and positioning all diagram elements.
        /// </summary>
        public event EventHandler DiagramLoaded;

        /// <summary>
        /// Raised when data bound objects have changed that affect the overall size of the diagram.
        /// This signals the view to recalculate the canvas size.
        /// </summary>
        public event EventHandler ExpandCanvasRequested;

        /// <summary>
        /// Gets the diagram caption to appear in the diagram tab.
        /// </summary>
        public string DiagramCaption
        {
            get { return Subject != null ? Subject.Name : "[New]"; }
        }

        public ObservableCollection<DiagramElement> DiagramElements
        {
            get { return this.diagramElements; }
        }

        /// <summary>
        /// Gets the full name of the diagram to appear as the tool tip for the diagram tab.
        /// </summary>
        /// <value>
        /// The full name of the diagram.
        /// </value>
        public string DiagramFullName
        {
            get { return Subject != null ? Subject.AssemblyQualifiedName : string.Empty; }
        }

        /// <summary>
        /// A unique identifier for this diagram instance.
        /// </summary>
        public Guid DiagramId { get; set; }

        public bool HideBackground
        {
            get { return this.hideBackground; }

            set
            {
                if (this.hideBackground != value)
                {
                    this.hideBackground = value;
                    VerifyPropertyName("HideBackground");
                    RaisePropertyChanged("HideBackground");
                }
            }
        }

        public IVisualisableTypeWithAssociations Subject
        {
            get { return this.currentSubject; }

            private set
            {
                if (this.currentSubject != value)
                {
                    this.currentSubject = value;
                    VerifyPropertyName("Subject");
                    OnMySubjectChanged();
                    RaisePropertyChanged("Subject");
                }
            }
        }

        public DiagramElement SubjectDiagramElement
        {
            get { return this.subjectDiagramElement; }
        }

        protected IContainer Factory
        {
            get { return this.doNotUseFactory ?? (this.doNotUseFactory = IoC.Default); }
        }

        private static bool IsLoading
        {
            get { return ShellController.Current.IsLoading; }
        }

        public static bool AddToTrivialListCanExecute(object parameter)
        {
            return !IsLoading && parameter is Association && !(parameter is SubjectAssociation);
        }

        public static bool AnnotateCanExecute()
        {
            return true;
        }

        public static bool NavigateToAssociationCanExecute(object parameter)
        {
            return !IsLoading && parameter != null && !(parameter is IVisualisableTypeWithAssociations);
        }

        public static bool ShowAllAssociationsCanExecute()
        {
            return !IsLoading;
        }

        public static bool TemporarilyHideAssociationCanExecute(object parameter)
        {
            var type = parameter as Association;
            return !IsLoading && type != null && !(type is SubjectAssociation);
        }

        public override void Cleanup()
        {
            if (this.clean)
            {
                return;
            }

            this.clean = true;
            DiagramElements.ToList().ForEach(x => x.Cleanup());
            DiagramElements.Clear();
            this.filter.Clear();
            Messenger.Send(new CloseDiagramMessage(DiagramId));
            base.Cleanup();
        }

        /// <summary>
        /// Activates the diagram after it has been in the background of the tab control and now has been selected by the user.
        /// </summary>
        public void ActivateDiagram()
        {
            if (this.filter == null)
            {
                return;
            }

            this.filter.ApplyTypeFilter(this.currentSubject);
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Multi-CLR Language not needed")]
        public void AddAnnotation(Point where, AnnotationData annotation = null)
        {
            if (where == default(Point))
            {
                return;
            }

            if (annotation == null)
            {
                annotation = new AnnotationData();
            }

            annotation.Text = EditAnnotationText(annotation);
            if (string.IsNullOrWhiteSpace(annotation.Text))
            {
                return;
            }

            var diagramElement = new DiagramElement(DiagramId, annotation) { TopLeft = where };
            DiagramElements.Add(diagramElement);
        }

        public void AddToTrivialList(Association association)
        {
            if (association == null)
            {
                throw new ArgumentNullResourceException("association", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            TrivialFilter.Current.AddToTrivialTypeList(association.AssociatedTo);
            this.filter.ApplyTypeFilter(this.currentSubject);
        }

        /// <summary>
        /// Assigns the diagram data.
        /// This is the main entry point to display something into the diagram visual tree.
        /// </summary>
        /// <param name="bindableDiagramData">The data to load into the diagram visual.</param>
        public void AssignDiagramData(object bindableDiagramData)
        {
            var subject = bindableDiagramData as IVisualisableTypeWithAssociations;
            if (subject == null)
            {
                throw new InvalidOperationException("Code Error: Invalid type given to Viewport Controller: " + bindableDiagramData);
            }

            Subject = subject;
            Subject.DiscoverSecondaryAssociationsInModel(); // This enables lines on the diagram other than those involving the subject.
            DiagramElements.Clear();
            this.drawingEngine = new ClassUmlDrawingEngine(DiagramId, subject);
            foreach (DiagramElement element in this.drawingEngine.DrawAllBoxes())
            {
                DiagramElements.Add(element);
            }
        }

        /// <summary>
        /// Clears all diagram visual elements.
        /// </summary>
        public void Clear()
        {
            Subject = null;
            this.subjectDiagramElement = null;
            this.drawingEngine = null;
            DiagramElements.ToList().ForEach(x => x.Cleanup());
            DiagramElements.Clear();
        }

        public DiagramElement DeleteAnnotation(AnnotationData annotation)
        {
            DiagramElement found = DiagramElements.FirstOrDefault(x => x.DiagramContent is AnnotationData && ((AnnotationData) x.DiagramContent).Id == annotation.Id);

            if (found != null)
            {
                DiagramElements.Remove(found);
                return found;
            }

            return null;
        }

        public void EditAnnotation(AnnotationData annotation)
        {
            if (annotation == null)
            {
                throw new ArgumentNullResourceException("annotation", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            annotation.Text = EditAnnotationText(annotation);
            if (string.IsNullOrWhiteSpace(annotation.Text))
            {
                DeleteAnnotation(annotation);
            }
        }

        public IPersistentFileData GatherPersistentData()
        {
            var canvasLayoutData = new CanvasLayoutData();
            foreach (DiagramElement element in DiagramElements)
            {
                if (Attribute.GetCustomAttribute(element.DiagramContent.GetType(), typeof (PersistentAttribute)) == null)
                {
                    // Only Diagram Content that is decorated with Persistent will get saved into the Layout collection.
                    continue;
                }

                var layoutData = new TypeLayoutData { ContentType = element.DiagramContent.GetType().FullName, TopLeft = element.TopLeft, Id = element.DiagramContent.Id, Visible = element.Show, };
                if (element.DiagramContent is AnnotationData)
                {
                    layoutData.Data = ((AnnotationData) element.DiagramContent).Text;
                }

                canvasLayoutData.Types.Add(layoutData);
            }

            var saveData = new TypeVisualiserLayoutFile
                {
                    CanvasLayout = canvasLayoutData,
                    Subject = (VisualisableTypeSubjectData) Subject.ExtractPersistentData(),
                    AssemblyFullName = Subject.AssemblyFullName,
                    AssemblyFileName = Subject.AssemblyFileName,
                    FileVersion = GetType().Assembly.GetName().Version.ToString(),
                };
            saveData.AssemblyFileName = saveData.Subject.AssemblyFileName ?? GetType().Assembly.Location;
            return saveData;
        }

        public void HideSecondaryAssociations()
        {
            this.filter.ApplyTypeFilter(this.currentSubject);
        }

        public void HideSystemTypes()
        {
            this.filter.ApplyTypeFilter(this.currentSubject);
        }

        public void HideTrivialTypes()
        {
            this.filter.ApplyTypeFilter(this.currentSubject);
        }

        /// <summary>
        /// Loads the diagram data from file after the subject has already been set by <see cref="AssignDiagramData"/>.
        /// At this stage the diagram has already been loaded and is displayed on screen.  
        /// This method ensures all diagram elements have been positioned where the file specifies their co-ordinates.
        /// Also will provide feedback on any mismatches, ie the type has changed and there are more or less associations.
        /// </summary>
        /// <param name="deserializedDataFile">The deserialised data file.</param>
        public void LoadDiagramDataFromFile(object deserializedDataFile)
        {
            var data = deserializedDataFile as TypeVisualiserLayoutFile;
            if (data == null)
            {
                throw new InvalidOperationException("Code Error: Attempt to load data from a type which is not an instance of Type Visualiser Layout File.");
            }

            var typesNoLongerInDiagram = new List<VisualisableTypeData>();
            foreach (TypeLayoutData layoutData in data.CanvasLayout.Types)
            {
                // Deal with annotations first - these have not been loaded by setting the subject.
                if (layoutData.ContentType == typeof (AnnotationData).FullName)
                {
                    var annotation = new AnnotationData { Id = layoutData.Id, Text = layoutData.Data, };
                    var annotationElement = new DiagramElement(DiagramId, annotation) { Show = layoutData.Visible, TopLeft = layoutData.TopLeft };
                    DiagramElements.Add(annotationElement);
                    continue;
                }

                DiagramElement element = DiagramElements.FirstOrDefault(x => x.DiagramContent.Id == layoutData.Id);
                if (element == null)
                {
                    VisualisableTypeData type = FindTypeInFile(data, layoutData.Id);
                    if (type == null)
                    {
                        throw new InvalidOperationException("Code error: there is a layout data element without a matching association. " + layoutData.Id);
                    }

                    typesNoLongerInDiagram.Add(type);
                    continue;
                }

                element.TopLeft = layoutData.TopLeft;
                element.Show = layoutData.Visible;
            }

            RaiseExpandCanvasRequested();

            if (typesNoLongerInDiagram.Any())
            {
                UserPrompt.Show(Resources.ViewportController_Error_in_diagram_file,
                                Resources.ViewportController_Some_types_are_not_in_assembly_anymore + string.Join(", ", typesNoLongerInDiagram.Select(x => x.Name)));
            }
        }

        /// <summary>
        /// Begins the navigating to the chosen type asynchronously.
        /// Called by the view.
        /// </summary>
        /// <param name="association">The association.</param>
        public void NavigateToAssociation(IVisualisableType association)
        {
            // Notify the File Manager that a new type needs to be loaded.
            Messenger.Send(new NavigateToDiagramAssociationMessage(DiagramId, association));
        }

        /// <summary>
        /// Positions the diagram elements. Is called by the View when all elements have been loaded onto the canvas.
        /// </summary>
        public void PositionDiagramElements()
        {
            // It is invalid to invoke this when lines have already been drawn.
            if (DiagramElements.Any(x => x.DiagramContent is ConnectionLine))
            {
                throw new InvalidOperationException("Code error: it is invalid to call View Loaded twice for one diagram.");
            }

            // By default the view will have loaded all visual controls for the diagram elements and positioned them at 0,0.  All Actual width and height properties are now set.
            this.drawingEngine.PositionMainSubject(this.hostDiagram);
            this.drawingEngine.PositionAllOtherAssociations(DiagramElements);

            // This needs to be queued to allow the expand canvas to fire and reposition all controls in positive space. After that then connect them with lines.
            this.filter = new ViewportControllerFilter { Dispatcher = Dispatcher, };
            Dispatcher.BeginInvoke(() =>
                {
                    var secondaryElements = DrawConnectingLines(false);
                    this.filter.SecondaryAssociationElements = secondaryElements;
                    this.filter.DiagramElements = DiagramElements;
                    this.filter.ApplyTypeFilter(this.currentSubject);
                    EventHandler handler = DiagramLoaded; // intended for the ShellController
                    if (handler != null)
                    {
                        handler(this.hostDiagram, EventArgs.Empty);
                    }
                },
                DispatcherPriority.ContextIdle);
        }

        private Dictionary<string, DiagramElement> DrawConnectingLines(bool redraw)
        {
            if (redraw)
            {
                // First remove the existing lines from the Diagram Element collection.
                foreach (var element in DiagramElements.Where(e => !(e.DiagramContent is Association)).ToList())
                {
                    DiagramElements.Remove(element);
                }
            }

            Dictionary<string, DiagramElement> secondaryElements;
            IEnumerable<DiagramElement> drawnElements = this.drawingEngine.DrawConnectingLines(
                DiagramElements,
                this.filter.ShouldThisSecondaryElementBeVisible,
                out secondaryElements);
            drawnElements.ToList().ForEach(e => DiagramElements.Add(e));
            return secondaryElements;
        }

        public void DrawConnectingLines()
        {
            if (DiagramElements.Any())
            {
                // Otherwise nothing to redraw
                DrawConnectingLines(true);
            }
        }

        public void Refresh(IFileManager fileManager)
        {
            if (fileManager == null)
            {
                throw new ArgumentNullResourceException("fileManager", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            Type t = fileManager.RefreshType(Subject.AssemblyQualifiedName, Subject.AssemblyFileName);
            IVisualisableTypeWithAssociations subject = fileManager.RefreshSubject(t);
            Clear();
            AssignDiagramData(subject);
            Dispatcher.BeginInvoke(() =>
                {
                    PositionDiagramElements();
                    RaiseExpandCanvasRequested();
                },
                                   DispatcherPriority.ContextIdle);
        }

        /// <summary>
        /// Gives a reference of the diagram container to the controller. This must be called prior to loading any content in the diagram.
        /// Should not be used externally, the diagram constructor must be the only caller of this.
        /// </summary>
        /// <param name="diagram">The new host diagram.</param>
        public void SetHostDiagram(Diagram diagram)
        {
            if (this.hostDiagram != null)
            {
                throw new InvalidOperationException("Code Error: You cannot call Set Host Diagram twice.");
            }

            this.hostDiagram = diagram;
        }

        public void ShowAllAssociations()
        {
            this.filter.ApplyTypeFilter(this.currentSubject);
        }

        public void TemporarilyHideAssociation(Association association)
        {
            DiagramElement diagramElement = DiagramElements.FirstOrDefault(x => x.DiagramContent.Equals(association));
            if (diagramElement != null)
            {
                diagramElement.Show = false;
            }
        }

        internal void ShowLineDetails(DiagramElement arrowheadOrLine)
        {
            FieldAssociation pointingAtAssociation = ClassDiagramSearchTool.FindAssociationTarget(arrowheadOrLine);
            if (pointingAtAssociation == null)
            {
                return;
            }

            new UsageDialog().ShowDialog(Resources.ApplicationName, Subject.Name, Subject.Modifiers.TypeTextName, pointingAtAssociation);
        }

        private static string EditAnnotationText(AnnotationData annotation)
        {
            var inputBox = new AnnotationInputBox { InputText = annotation.Text };
            inputBox.ShowDialog();

            if (string.IsNullOrWhiteSpace(inputBox.InputText))
            {
                return null;
            }

            return inputBox.InputText;
        }

        private static VisualisableTypeData FindTypeInFile(TypeVisualiserLayoutFile data, string id)
        {
            if (data.Subject.Id == id)
            {
                return data.Subject;
            }

            if (data.Subject.Parent != null && data.Subject.Parent.AssociatedTo.Id == id)
            {
                return data.Subject;
            }

            AssociationData type = data.Subject.Associations.FirstOrDefault(x => x.AssociatedTo.Id == id);
            if (type != null)
            {
                return type.AssociatedTo;
            }

            type = data.Subject.Implements.FirstOrDefault(x => x.AssociatedTo.Id == id);
            if (type != null)
            {
                return type.AssociatedTo;
            }

            return null;
        }

        private void OnMySubjectChanged()
        {
            if (Subject != null)
            {
                Factory.GetInstance<IDiagramDimensions>().Initialise(Subject.FilteredAssociations.Count(), true);
            }
        }

        private void RaiseExpandCanvasRequested()
        {
            // Expand canvas needs to be manually triggered when navigating to a type on an open diagram. When a new tab opens expand canvas is triggered by view events otherwise.
            Dispatcher.BeginInvoke(() =>
                {
                    EventHandler handler = ExpandCanvasRequested;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                },
                                   DispatcherPriority.ContextIdle);
        }
    }
}