using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TypeVisualiser.Geometry;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.UI.WpfUtilities;

namespace TypeVisualiser.UI.Views
{
    /// <summary>
    /// Interaction logic for Viewport.xaml
    /// </summary>
    public partial class Viewport
    {
        private Timer connectorLineFixTimer;
        private bool pauseRearrangeEvents;
        private VisualisableTypeSubject subjectDataContext;
        private VisualisedTypeViewMetadata subjectMetadata;

        public Viewport()
        {
        }

        private void AddArrowHeadToDiagramSpace(VisualisedTypeViewMetadata associate, UserControl head)
        {
            // Arrow head is added to diagram before the connector line.
            //this.DiagramSpace.Children.Add(head);
            //head.DataContext = null;
            //var relocateHeadArrow = new Action(() =>
            //                                       {
            //                                           if (this.pauseRearrangeEvents)
            //                                           {
            //                                               return;
            //                                           }
            //                                           ConnectionRoute route = ConnectionRoute.FindBestConnectionRoute(this.subjectMetadata.Area, associate.Area, IsOverlappingWithOtherControls);
            //                                           associate.LineRoute = route;
            //                                           route.CalculateArrowHeadRotationAndTranslation(ConnectionRoute.LineEnd.ArrowHead, head.ActualWidth, head.ActualHeight, associate.DataContext);
            //                                           // move the arrow head to position into gap between line end and the target.
            //                                           head.DataContext = route;
            //                                           Canvas.SetLeft(head, route.To.X);
            //                                           Canvas.SetTop(head, route.To.Y);
            //                                           // this.LineTailDiagnostics(head, route, associate);
            //                                       });

            //head.Loaded += (s, e) => relocateHeadArrow();
            //associate.Moved += (s, e) => Dispatcher.BeginInvoke(relocateHeadArrow);
            //this.subjectMetadata.Moved += (s, e) => Dispatcher.BeginInvoke(relocateHeadArrow);
        }

        private void AddAssociatesToDiagram(VisualisableTypeSubject dataContext)
        {
            if (dataContext.Parent != null)
            {
                CreateAssociate(dataContext.Parent);
            }

            foreach (ParentAssociation implementedInterface in dataContext.ThisTypeImplements)
            {
                CreateAssociate(implementedInterface);
            }

            foreach (FieldAssociation associate in dataContext.FilteredAssociations)
            {
                CreateAssociate(associate);
            }

            // this.DiagnosticsDrawAssociateCircle();
        }

        private byte AddByteValue(byte value, int addend)
        {
            var add = (byte)addend;
            var result = (byte)(value + add);
            if (result > 255)
            {
                result = 255;
            }

            return result;
        }

        private void ClearDiagram()
        {
            // Clear diagram except main subject
            //Trace.WriteLine("ClearDiagram:");
            //this.pauseRearrangeEvents = true;
            //this.allVisualisedTypes = new Dictionary<string, VisualisedTypeViewMetadata>();
            //var copyOfChildren = new object[this.DiagramSpace.Children.Count];
            //this.DiagramSpace.Children.CopyTo(copyOfChildren, 0);
            //int clearCount = 0;
            //foreach (object drawingItem in copyOfChildren)
            //{
            //    var uiElement = drawingItem as UIElement;
            //    if (uiElement != null && uiElement != this.Subject)
            //    {
            //        this.DiagramSpace.Children.Remove(uiElement);
            //        clearCount++;
            //    } else
            //    {
            //        Trace.WriteLine("    Not clearing: " + drawingItem);
            //    }
            //}

            //Trace.WriteLine("    Cleared " + clearCount);
            //this.pauseRearrangeEvents = false;
        }

        private void CreateAssociate(Association associate)
        {
            //var control = new ContentPresenter
            //                  {
            //                      DataContext = associate.AssociatedTo,
            //                      Content = associate.AssociatedTo,
            //                      Tag = VisualisedTypeViewMetadata.AssociateIdentifier + Guid.NewGuid()
            //                      // To distinguish this content presenter from other usages
            //                  };

            //this.DiagramSpace.Children.Add(control);
            //var metadata = new VisualisedTypeViewMetadata(control, associate);
            //this.allVisualisedTypes.Add(control.Tag.ToString(), metadata);
            //control.Loaded += (s, e) => OnAssociateControlLoaded(metadata);
        }

        private void DrawLine(VisualisedTypeViewMetadata associate)
        {
            //UserControl head = associate.DataContext.CreateLineHead();
            //AddArrowHeadToDiagramSpace(associate, head);
            //// UserControl tail = this.AddLineTail(head, associate);

            //var line = new Line { SnapsToDevicePixels = true, DataContext = associate.DataContext, };
            //associate.DataContext.StyleLine(line);
            //this.DiagramSpace.Children.Add(line);

            //var visibilityBinding = new Binding { Path = new PropertyPath("Show"), Converter = new BooleanToVisibilityConverter() };
            //BindingOperations.SetBinding(line, VisibilityProperty, visibilityBinding);

            //var relocateLine = new Action(() =>
            //                                  {
            //                                      if (this.pauseRearrangeEvents)
            //                                      {
            //                                          return;
            //                                      }

            //                                      ConnectionRoute route = associate.LineRoute; // Set by adding the arrow head.
            //                                      Point position = route.To.Clone();
            //                                      Canvas.SetLeft(line, position.X);
            //                                      Canvas.SetTop(line, position.Y);
            //                                      // var endPoint = new Point(Canvas.GetLeft(tail), Canvas.GetTop(tail));
            //                                      Point endPoint = route.From.Clone();
            //                                      line.X2 = position.X > endPoint.X ? -(position.X - endPoint.X) : endPoint.X - position.X;
            //                                      line.Y2 = position.Y > endPoint.Y ? -(position.Y - endPoint.Y) : endPoint.Y - position.Y;
            //                                  });
            //line.Loaded += (s, e) => relocateLine();
            //line.MouseLeftButtonDown += OnLineClicked;
            //line.MouseEnter += OnMouseOverLine;
            //line.MouseLeave += OnMouseLeaveLine;
            //associate.Moved += (s, e) => Dispatcher.BeginInvoke(relocateLine);
            //this.subjectMetadata.Moved += (s, e) => Dispatcher.BeginInvoke(relocateLine);
        }

        /// <summary>
        /// Determines whether the given area is overlapping with other areas occupied by controls.
        /// </summary>
        /// <param name="proposedArea">The proposed area to compare with all others.</param>
        /// <returns>A result object indicating if an overlap exists or the closest object and distance to it.</returns>
        private ProximityTestResult IsOverlappingWithOtherControls(Area proposedArea)
        {
            //IEnumerable<ProximityTestResult> proximities = this.allVisualisedTypes.Values.Select(knownAssociation => knownAssociation.Area.OverlapsWith(proposedArea)).ToList();

            //bool overlapsWith = proximities.Any(x => x.Proximity == Proximity.Overlapping);
            //if (overlapsWith)
            //{
            //    return new ProximityTestResult(Proximity.Overlapping);
            //}

            //IOrderedEnumerable<ProximityTestResult> veryClose = proximities.Where(x => x.Proximity == Proximity.VeryClose).OrderBy(x => x.DistanceToClosestObject);

            //if (veryClose.Any())
            //{
            //    return veryClose.First();
            //}

            //return new ProximityTestResult(Proximity.NotOverlapping);
            return null;
        }

        private void OnAssociateControlLoaded(VisualisedTypeViewMetadata control)
        {
            PositionTheAssociateControl(control.Container, control.DataContext);
            control.Area.Update(control.Container);
            DrawLine(control);
        }

        private void OnConnectorLineFixTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //// This fixes the connector lines being drawn out of co-ord 0,0 of subject. Caused by the actual width and height not being available soon enough.
            //this.connectorLineFixTimer.Elapsed -= OnConnectorLineFixTimerElapsed;
            //this.connectorLineFixTimer.Stop();
            //this.connectorLineFixTimer.Dispose();
            //this.connectorLineFixTimer = null;
            //// TODO hack to fix line connectors by moving the subject a fraction has been removed.
            //// Dispatcher.BeginInvoke(() => OnSubjectDragDelta(this, new DragDeltaEventArgs(0.01, 0.01) { Source = this.Subject }), DispatcherPriority.Normal);
            //Dispatcher.BeginInvoke(() => Controller.ViewReady(), DispatcherPriority.Normal);
        }

        private void OnLineClicked(object sender, MouseButtonEventArgs e)
        {
            var association = ((Line)sender).DataContext as FieldAssociation;
            if (association != null)
            {
                new UsageDialog().ShowDialog(Properties.Resources.ApplicationName, this.subjectDataContext.Name, this.subjectDataContext.Modifiers.TypeTextName, association);
                e.Handled = true;
            }
        }

        private void OnMainPresenterDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (e.NewValue == null)
            //{
            //    return;
            //}

            //Dispatcher.BeginInvoke(new Action(() =>
            //                                      {
            //                                          if (Controller.Subject != null)
            //                                          {
            //                                              this.Subject.DataContext = Controller.Subject;
            //                                              OnSubjectLoaded(null, null);
            //                                          }
            //                                      }));
        }

        private void OnMouseLeaveLine(object sender, MouseEventArgs e)
        {
            var line = e.OriginalSource as Line;
            if (line == null)
            {
                return;
            }

            var brush = (Brush)line.Tag;
            line.Tag = line.Stroke;
            line.Stroke = brush;
        }

        private void OnMouseOverLine(object sender, MouseEventArgs e)
        {
            var line = e.OriginalSource as Line;
            if (line == null)
            {
                return;
            }

            SolidColorBrush highlightBrush;
            if (line.Tag == null)
            {
                const byte Highlight = 60;
                var brush = line.Stroke as SolidColorBrush;
                line.Tag = brush;
                if (brush == null)
                {
                    throw new NotSupportedException("Line stroke brush must be a Solid Color Brush.");
                }

                var highlightColor = new Color { A = 255, R = AddByteValue(brush.Color.R, Highlight), G = AddByteValue(brush.Color.G, Highlight), B = AddByteValue(brush.Color.B, Highlight) };
                highlightBrush = new SolidColorBrush(highlightColor);
            } else
            {
                highlightBrush = (SolidColorBrush)line.Tag;
            }

            line.Tag = line.Stroke;
            line.Stroke = highlightBrush;
        }

        private void OnSubjectLoaded(object sender, RoutedEventArgs e)
        {
            //if (this.Subject.DataContext == null)
            //{
            //    // DataContext of subject hasn't been set yet.
            //    return;
            //}

            //if (this.allVisualisedTypes.Any())
            //{
            //    // Already loaded. Diagram is immutable, create a new tab. Add to stop reloading on tab change.
            //    return;
            //}

            //this.Subject.Tag = VisualisedTypeViewMetadata.SubjectIdentifier;

            //// Identify this usage of the Data Template / Content Presenter as the main subject.
            //this.subjectMetadata = new VisualisedTypeViewMetadata(this.Subject, Association.Subject);
            //if (this.subjectMetadata.Area.Height < 1)
            //{
            //    UpdateLayout(); // I know this is not a good idea to overuse, however its necessary here to ensure the layout system has calculated the actual width and height.
            //    this.subjectMetadata = new VisualisedTypeViewMetadata(this.Subject, Association.Subject);
            //}

            //this.allVisualisedTypes.Add(VisualisedTypeViewMetadata.SubjectIdentifier, this.subjectMetadata);

            //this.subjectDataContext = this.Subject.DataContext as VisualisableTypeSubject;
            //if (this.subjectDataContext == null)
            //{
            //    throw new InvalidCastException("Subject is not a " + typeof(VisualisableTypeSubject).Name);
            //}

            //// new ViewportDiagnostics().SubjectDiagnostics(this.Subject);
            //AddAssociatesToDiagram(this.subjectDataContext);

            //// To fix the connector lines problem a timer is used to cause a drag event on the subject which then redraws the lines.
            //// Tried queueing a job on the dispatcher to wait for actual width and height to be updated from 0, but it doesnt update until all drawing is finished.
            //// Also tried changing the onloaded event from the subject content presenter to the canvas with no effect.
            //this.connectorLineFixTimer = new Timer(500);
            //this.connectorLineFixTimer.Elapsed += OnConnectorLineFixTimerElapsed;
            //this.connectorLineFixTimer.Start();
        }


        private void PositionTheAssociateControl(FrameworkElement control, Association associate)
        {
            Area proposedArea = associate.ProposePosition(control.ActualWidth, control.ActualHeight, this.subjectMetadata.Area, IsOverlappingWithOtherControls);
            Canvas.SetLeft(control, proposedArea.TopLeft.X);
            Canvas.SetTop(control, proposedArea.TopLeft.Y);
        }
    }
}