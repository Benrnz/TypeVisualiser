namespace TypeVisualiser.UI
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public static class ViewportDiagnostics
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Diag code only.")]
        [Conditional("DEBUG")]
        public static void SubjectDiagnostics(FrameworkElement subjectControl)
        {
            double left = Canvas.GetLeft(subjectControl);
            double top = Canvas.GetTop(subjectControl);
            Debug.WriteLine("Subject Position is {0},{1}", left, top);
            Debug.WriteLine("Subject Size is {0},{1}", subjectControl.ActualWidth, subjectControl.ActualHeight);
            Debug.WriteLine("Subject Bottom Right is {0},{1}", left + subjectControl.ActualWidth, top + subjectControl.ActualHeight);
        }

        //        [Conditional("DEBUG")]
        //        private void DiagnosticsDrawAssociateCircle()
        //        {
        //// Draw a circle to show where associations should be placed - Diag only
        //            var associateCircle = new Ellipse
        //            {
        //                Stroke = new SolidColorBrush(Colors.Black),
        //                StrokeThickness = 1,
        //                StrokeDashArray = new DoubleCollection(new[] { 2.0, 1.0 }),
        //                Width = DataContext.Radius * 2,
        //                Height = DataContext.Radius * 2
        //            };
        //            this.DiagramSpace.Children.Add(associateCircle);
        //            var topLeft = this.subjectMetadata.Area.Centre;
        //            topLeft.Offset(-DataContext.Radius, -DataContext.Radius); // Centered
        //            Canvas.SetTop(associateCircle, topLeft.X);
        //            Canvas.SetLeft(associateCircle, topLeft.Y);

        //            var centre = this.subjectMetadata.Area.Centre;
        //            var calc = new CircleCalculator(centre, DataContext.Radius);
        //            for (int angle = 0; angle < 360; angle += 30)
        //            {
        //                Debug.WriteLine("DIag Circle: angle={0}", angle);
        //                var pointOnCircle = calc.CalculatePointOnCircle(angle);
        //                var line = new Line
        //                {
        //                    Stroke = new SolidColorBrush(Colors.Black),
        //                    StrokeThickness = 1,
        //                    StrokeDashArray = new DoubleCollection(new[] { 2.0, 1.0 }),
        //                    X2 = centre.X > pointOnCircle.X ? -(centre.X - pointOnCircle.X) : pointOnCircle.X - centre.X,
        //                    Y2 = centre.Y > pointOnCircle.Y ? -(centre.Y - pointOnCircle.Y) : pointOnCircle.Y - centre.Y,
        //                };

        //                this.DiagramSpace.Children.Add(line);
        //                Canvas.SetLeft(line, centre.X);
        //                Canvas.SetTop(line, centre.Y);

        //                var dot = new Ellipse()
        //                {
        //                    Fill = new SolidColorBrush(Colors.Black),
        //                    Height = 5,
        //                    Width = 5,
        //                    ToolTip = string.Format(CultureInfo.CurrentCulture, "Angle={0}", angle)
        //                };
        //                this.DiagramSpace.Children.Add(dot);
        //                Canvas.SetLeft(dot, pointOnCircle.X - 2.5);
        //                Canvas.SetTop(dot, pointOnCircle.Y - 2.5);
        //            }
        //        }

        //private UserControl AddLineTail(UserControl arrowHead, VisualisedTypeViewMetadata associateVisual)
        //{
        //    var tail = new ArrowTail
        //    {
        //        SnapsToDevicePixels = true,
        //    };

        //    this.DiagramSpace.Children.Add(tail);

        //    var relocateTail = new Action(
        //        () =>
        //        {
        //            ConnectionLine route = this.FindBestConnectionRoute(this.subjectMetadata, associateVisual);
        //            route.CalculateArrowHeadRotationAndTranslation(ConnectionLine.LineEnd.Tail, tail.ActualWidth, tail.ActualHeight);
        //            tail.DataContext = route;
        //            Canvas.SetLeft(tail, route.From.X); 
        //            Canvas.SetTop(tail, route.From.Y);
        //            //LineTailDiagnostics(tail, route, associateVisual);
        //        });
        //    tail.Loaded += (s, e) => relocateTail();
        //    this.subjectMetadata.Moved += (s, e) => this.Dispatcher.BeginInvoke(relocateTail);
        //    return tail;
        //}

        //[Conditional("DEBUG")]
        //private void LineTailDiagnostics(UserControl tail, ConnectionLine route, VisualisedTypeViewMetadata associateVisual)
        //{
        //    var headsOrTails = tail is ArrowTail ? "Tail" : "Arrowhead";
        //    Debug.WriteLine("{0} Placement Complete for {1}", headsOrTails, associateVisual.DataContext.Name);
        //    Debug.WriteLine("    line end {0},{1}", route.From.X, route.From.Y);
        //    var point = new Point(Canvas.GetLeft(tail), Canvas.GetTop(tail));
        //    Debug.WriteLine("    top left {0},{1}", point.X, point.Y);
        //}
    }
}
