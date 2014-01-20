using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;

namespace TypeVisualiser.UI.Views
{
    /// <summary>
    /// Interaction logic for ShellTabControl.xaml
    /// </summary>
    public partial class ShellTabControl
    {
        private DiagramElement currentlyDraggingElement;
        private Point lastRightMouseButtonClickPosition;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        public ShellTabControl()
        {
            InitializeComponent();
            MessagingGate.Register<CloseDiagramMessage>(this, OnCloseDiagram);
        }

        private ViewportController Controller
        {
            get
            {
                if (Diagram == null)
                {
                    return null;
                }

                return (ViewportController) Diagram.Controller;
            }
        }

        private Diagram Diagram
        {
            get { return DataContext as Diagram; }
        }

        private static bool OnlyAssociationsShouldDrag(object sender, out Grid grid)
        {
            grid = sender as Grid;
            if (grid == null)
            {
                return false;
            }

            var diagramElement = grid.DataContext as DiagramElement;
            if (diagramElement == null)
            {
                return false;
            }

            if (diagramElement.DiagramContent is Association || diagramElement.DiagramContent is AnnotationData)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Expand the content area to fit the diagram-elements.
        /// </summary>
        private void ExpandContent()
        {
            double smallestX = 0;
            double smallestY = 0;
            var contentRect = new Rect(0, 0, 0, 0);
            foreach (DiagramElement diagramElement in Controller.DiagramElements)
            {
                if (diagramElement.TopLeft.X < smallestX)
                {
                    smallestX = diagramElement.TopLeft.X;
                }

                if (diagramElement.TopLeft.Y < smallestY)
                {
                    smallestY = diagramElement.TopLeft.Y;
                }

                contentRect.Union(new Rect(diagramElement.TopLeft.X, diagramElement.TopLeft.Y, diagramElement.Width, diagramElement.Height));
            }

            // Translate all diagram-elements so they are in positive space.
            smallestX = Math.Abs(smallestX);
            smallestY = Math.Abs(smallestY);

            if (smallestX > 0.01 || smallestY > 0.01)
            {
                Controller.DiagramElements.ToList().ForEach(element => element.AdjustCoordinatesAfterCanvasExpansion(smallestX, smallestY));
            }

            Diagram.ContentWidth = contentRect.Width;
            Diagram.ContentHeight = contentRect.Height;
        }

        private void OnAddToTrivialListCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewportController.AddToTrivialListCanExecute(e.Parameter);
            e.Handled = true;
        }

        private void OnAddToTrivialListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var type = e.Parameter as Association;
            if (type == null || type is SubjectAssociation)
            {
                return;
            }

            Controller.AddToTrivialList(type);
        }

        private void OnAnnotateCanvasCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewportController.AnnotateCanExecute();
            e.Handled = true;
        }

        private void OnAnnotateCanvasExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Controller.AddAnnotation(this.lastRightMouseButtonClickPosition);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by Wpf")]
        private void OnAnnotationEditRequested(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var control = e.OriginalSource as FrameworkElement;
            if (control == null)
            {
                return;
            }

            Controller.EditAnnotation(control.DataContext as AnnotationData);
        }

        /// <summary>
        /// Called when explicitly closed by the user.
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnCloseDiagram(CloseDiagramMessage message)
        {
            if (message.DiagramId != Diagram.Id)
            {
                return;
            }

            MessagingGate.Unregister<CloseDiagramMessage>(this);
            //MessagingGate.Unregister<NotifyNewDiagramDisplayedMessage>(this);
            DataContext = null;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldDiagram = e.OldValue as Diagram;
            if (oldDiagram != null)
            {
                oldDiagram.Controller.ExpandCanvasRequested -= OnExpandCanvasRequested;
                oldDiagram.Controller.Cleanup();
            }

            var newDiagram = e.NewValue as Diagram;
            if (newDiagram != null)
            {
                newDiagram.Controller.ExpandCanvasRequested += OnExpandCanvasRequested;
            }
        }

        /// <summary>
        /// Event raised when a mouse button is clicked down over a diagram-element.
        /// </summary>
        private void OnDiagramElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Grid grid;
            if (!OnlyAssociationsShouldDrag(sender, out grid))
            {
                return;
            }

            this.DiagramSpace.Focus();
            Keyboard.Focus(this.DiagramSpace);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                // When the shift key is held down special logic may be applied,
                // so don't handle mouse input here.
                return;
            }

            if (this.mouseHandlingMode != MouseHandlingMode.None)
            {
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            this.mouseHandlingMode = MouseHandlingMode.DraggingDiagramElements;
            this.currentlyDraggingElement = grid.DataContext as DiagramElement;
            if (this.currentlyDraggingElement == null)
            {
                throw new InvalidOperationException("Code Error: all data contexts should be Diagram Elements");
            }

            this.currentlyDraggingElement.ZOrder = 99;
            this.origContentMouseDownPoint = e.GetPosition(this.DiagramSpace);
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Diagram-Element.
        /// </summary>
        private void OnDiagramElementMouseMove(object sender, MouseEventArgs e)
        {
            Grid grid;
            if (!OnlyAssociationsShouldDrag(sender, out grid))
            {
                return;
            }

            if (this.mouseHandlingMode != MouseHandlingMode.DraggingDiagramElements)
            {
                // We are not in Diagram-Element dragging mode, so don't do anything.
                return;
            }

            e.Handled = true;
            Point curContentPoint = e.GetPosition(this.DiagramSpace);
            Vector rectangleDragVector = curContentPoint - this.origContentMouseDownPoint;
            if (Math.Abs(rectangleDragVector.Length) < 0.001)
            {
                return;
            }

            // When in 'dragging Diagram-Elements' mode update the position of the Diagram-Element as the user drags it.
            this.origContentMouseDownPoint = curContentPoint;
            this.currentlyDraggingElement.TopLeft = new Point(this.currentlyDraggingElement.TopLeft.X + rectangleDragVector.X, this.currentlyDraggingElement.TopLeft.Y + rectangleDragVector.Y);

            ExpandContent();
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Diagram element.
        /// </summary>
        private void OnDiagramElementMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Grid grid;
            if (!OnlyAssociationsShouldDrag(sender, out grid))
            {
                return;
            }

            if (this.mouseHandlingMode != MouseHandlingMode.DraggingDiagramElements)
            {
                // We are not in Diagram-Element dragging mode.
                return;
            }

            this.mouseHandlingMode = MouseHandlingMode.None;
            this.currentlyDraggingElement = null;
            e.Handled = true;
        }

        private void OnDiagramElementSizeChanged(object sender, EventArgs e)
        {
            // This is basically manual binding back to the height and width properties.
            // It cannot use automatic binding because ActualWidth is a readonly property, and
            // even when set using OneWayToSource it will still not bind.  It will not work
            // with the Width property because this must be set manually for it to have a value.
            var grid = sender as Grid;
            if (grid == null)
            {
                return;
            }

            var diagramElement = grid.DataContext as DiagramElement;
            if (diagramElement == null)
            {
                return;
            }

            diagramElement.Width = grid.ActualWidth;
            diagramElement.Height = grid.ActualHeight;
        }

        private void OnExpandCanvasRequested(object sender, EventArgs e)
        {
            ExpandContent();
        }

        private void OnLineClicked(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var lineOrArrowhead = sender as DependencyObject;
            DiagramElement element;
            do
            {
                if (lineOrArrowhead == null)
                {
                    throw new InvalidCastException("Unable to cast sender to Dependency Object");
                }

                var bindable = lineOrArrowhead as FrameworkElement;
                if (bindable != null && bindable.DataContext is DiagramElement)
                {
                    element = bindable.DataContext as DiagramElement;
                    break;
                }

                lineOrArrowhead = VisualTreeHelper.GetParent(lineOrArrowhead);
            } while (true);

            Controller.ShowLineDetails(element);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Controller.PositionDiagramElements();
            ExpandContent();
        }

        private void OnNavigateToTypeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewportController.NavigateToAssociationCanExecute(e.Parameter);
            e.Handled = true;
        }

        private void OnNavigateToTypeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var type = e.Parameter as Association;
            if (type == null)
            {
                return;
            }

            e.Handled = true;
            Controller.NavigateToAssociation(type.AssociatedTo);
        }

        private void OnRightMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.lastRightMouseButtonClickPosition = Mouse.GetPosition(this.DiagramSpace);
        }

        private void OnShowAllCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewportController.ShowAllAssociationsCanExecute();
            e.Handled = true;
        }

        private void OnShowAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Controller.ShowAllAssociations();
        }

        private void OnTemporarilyHideCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewportController.TemporarilyHideAssociationCanExecute(e.Parameter);
            e.Handled = true;
        }

        private void OnTemporarilyHideExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var type = e.Parameter as Association;
            if (type == null)
            {
                return;
            }

            e.Handled = true;
            Controller.TemporarilyHideAssociation(type);
        }

        private void OnUnloaded(object sender, EventArgs e)
        {
            MessagingGate.Unregister<CloseDiagramMessage>(this);
            if (Controller != null)
            {
                Controller.ExpandCanvasRequested -= OnExpandCanvasRequested;
                Controller.Cleanup();
            }
        }

        /// <summary>
        /// Event raised when the user has double clicked in the zoom and pan control.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf")]
        private void OnZoomAndPanControlMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                Point doubleClickPoint = e.GetPosition(this.DiagramSpace);
                this.zoomAndPanControl.AnimatedSnapTo(doubleClickPoint);
            }
        }

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf")]
        private void OnZoomAndPanControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            this.DiagramSpace.Focus();
            Keyboard.Focus(this.DiagramSpace);

            this.origContentMouseDownPoint = e.GetPosition(this.DiagramSpace);

            // Just a plain old left-down initiates panning mode.
            this.mouseHandlingMode = MouseHandlingMode.Panning;
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf")]
        private void OnZoomAndPanControlMouseMove(object sender, MouseEventArgs e)
        {
            UpdateMouseCoordinates(e);

            if (this.mouseHandlingMode == MouseHandlingMode.Panning)
            {
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                Point curContentMousePoint = e.GetPosition(this.DiagramSpace);
                Vector dragOffset = curContentMousePoint - this.origContentMouseDownPoint;

                this.zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                this.zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf")]
        private void OnZoomAndPanControlMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.mouseHandlingMode != MouseHandlingMode.None)
            {
                this.mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf")]
        private void OnZoomAndPanControlMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(this.DiagramSpace);
                ZoomIn(curContentMousePoint);
            } else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(this.DiagramSpace);
                ZoomOut(curContentMousePoint);
            }
        }

        //private void OnCanvasLayoutDataRequested(GetCanvasLayoutDataMessage message)
        //{
        //    if (message.DiagramId != Controller.DiagramId)
        //    {
        //        return;
        //    }

        //    foreach (VisualisedTypeViewMetadata metadata in this.allVisualisedTypes.Values.Where(m => !m.IsSubject))
        //    {
        //        IVisualisableType type = metadata.DataContext.AssociatedTo;
        //        message.Layout.Types.Add(new TypeLayoutData { FullName = type.FullName, Id = type.Id, Visible = type.Show, TopLeft = metadata.Area.TopLeft });
        //    }

        //    message.Layout.Subject = new TypeLayoutData
        //    {
        //        FullName = this.subjectDataContext.FullName,
        //        Id = this.subjectDataContext.Id,
        //        Visible = this.subjectDataContext.Show,
        //        TopLeft = this.subjectMetadata.Area.TopLeft,
        //    };
        //}

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void OnZoomToFitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.zoomAndPanControl.AnimatedScaleToFit(30);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)",
            Justification = "Nothing to localise here.")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf")]
        private void UpdateMouseCoordinates(MouseEventArgs e)
        {
            Point point = e.GetPosition(e.Source as FrameworkElement);
            this.MouseCoordinates.Text = string.Format(CultureInfo.CurrentCulture, "{0:F1},{1:F1}", point.X, point.Y);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf via OnZoomAndPanControlMouseWheel")]
        private void ZoomIn(Point contentZoomCenter)
        {
            this.zoomAndPanControl.ZoomAboutPoint(this.zoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "called by Wpf via OnZoomAndPanControlMouseWheel")]
        private void ZoomOut(Point contentZoomCenter)
        {
            this.zoomAndPanControl.ZoomAboutPoint(this.zoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
        }

        private IUserPromptMessage doNotUseMessageBoxService;
        private IUserPromptMessage MessageBoxService
        {
            get { return this.doNotUseMessageBoxService ?? (this.doNotUseMessageBoxService = new GlassMessageBox()); }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "TypeVisualiser.Messaging.IUserPromptMessage.Show(System.String,System.String)", Justification = "Debug info only")]
        private void OnMoreInfoClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            var element = button.DataContext as DiagramElement;
            if (element == null)
            {
                return;
            }

            string message = string.Format(
                CultureInfo.CurrentCulture, 
                "{0}\nArea.Width: {1:F1}\nArea.Height: {2:F1}\n\nTop Left: {3:F1}, {4:F1}\nWidth: {5:F1}\nHeight: {6:F1}\n\n{7}", 
                element.Area, 
                element.Area.Width, 
                element.Area.Height,
                element.TopLeft.X,
                element.TopLeft.Y,
                element.Width,
                element.Height,
                element.DiagramContent);
            MessageBoxService.Show(message, element.DiagramContent.ToString());
        }
    }
}