namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// A class that wraps up zooming and panning of it's content.
    /// </summary>
    public partial class ZoomAndPanControl : ContentControl, IScrollInfo
    {
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(
            "AnimationDuration", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.4));

        public static readonly DependencyProperty ContentOffsetXProperty = DependencyProperty.Register(
            "ContentOffsetX", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0, ContentOffsetXPropertyChanged, ContentOffsetXCoerce));

        public static readonly DependencyProperty ContentOffsetYProperty = DependencyProperty.Register(
            "ContentOffsetY", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0, ContentOffsetYPropertyChanged, ContentOffsetYCoerce));

        public static readonly DependencyProperty ContentScaleProperty = DependencyProperty.Register(
            "ContentScale", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(1.0, ContentScalePropertyChanged, ContentScaleCoerce));

        public static readonly DependencyProperty ContentViewportHeightProperty = DependencyProperty.Register(
            "ContentViewportHeight", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentViewportWidthProperty = DependencyProperty.Register(
            "ContentViewportWidth", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentZoomFocusXProperty = DependencyProperty.Register(
            "ContentZoomFocusX", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentZoomFocusYProperty = DependencyProperty.Register(
            "ContentZoomFocusY", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.Register(
            "IsMouseWheelScrollingEnabled", typeof(bool), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MaxContentScaleProperty = DependencyProperty.Register(
            "MaxContentScale", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(10.0, MinOrMaxContentScalePropertyChanged));

        public static readonly DependencyProperty MinContentScaleProperty = DependencyProperty.Register(
            "MinContentScale", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.01, MinOrMaxContentScalePropertyChanged));

        public static readonly DependencyProperty ViewportZoomFocusXProperty = DependencyProperty.Register(
            "ViewportZoomFocusX", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ViewportZoomFocusYProperty = DependencyProperty.Register(
            "ViewportZoomFocusY", typeof(double), typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// The height of the viewport in content coordinates, clamped to the height of the content.
        /// </summary>
        private double constrainedContentViewportHeight;

        /// <summary>
        /// The width of the viewport in content coordinates, clamped to the width of the content.
        /// </summary>
        private double constrainedContentViewportWidth;

        /// <summary>
        /// Reference to the underlying content, which is named PART_Content in the template.
        /// </summary>
        private FrameworkElement content;

        /// <summary>
        /// The transform that is applied to the content to offset it by 'ContentOffsetX' and 'ContentOffsetY'.
        /// </summary>
        private TranslateTransform contentOffsetTransform;

        /// <summary>
        /// The transform that is applied to the content to scale it by 'ContentScale'.
        /// </summary>
        private ScaleTransform contentScaleTransform;

        /// <summary>
        /// Normally when content offsets changes the content focus is automatically updated.
        /// This synchronization is disabled when 'disableContentFocusSync' is set to 'true'.
        /// When we are zooming in or out we 'disableContentFocusSync' is set to 'true' because 
        /// we are zooming in or out relative to the content focus we don't want to update the focus.
        /// </summary>
        private bool disableContentFocusSync;

        /// <summary>
        /// Used to disable synchronization between IScrollInfo interface and ContentOffsetX/ContentOffsetY.
        /// </summary>
        private bool disableScrollOffsetSync;

        /// <summary>
        /// Enable the update of the content offset as the content scale changes.
        /// This enabled for zooming about a point (google-maps style zooming) and zooming to a rectangle.
        /// </summary>
        private bool enableContentOffsetUpdateFromScale;

        /// <summary>
        /// Records the un-scaled extent of the content.
        /// This is calculated during the measure and arrange.
        /// </summary>
        private Size unscaledExtent = new Size(0, 0);

        /// <summary>
        /// Records the size of the viewport (in viewport coordinates) onto the content.
        /// This is calculated during the measure and arrange.
        /// </summary>
        private Size viewport = new Size(0, 0);

        /// <summary>
        /// Initializes static members of the <see cref="ZoomAndPanControl"/> class. 
        /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Established patter for dependency properties.")]
        static ZoomAndPanControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanControl), new FrameworkPropertyMetadata(typeof(ZoomAndPanControl)));
        }

        /// <summary>
        /// Event raised when the ContentOffsetX property has changed.
        /// </summary>
        public event EventHandler ContentOffsetXChanged;

        /// <summary>
        /// Event raised when the ContentOffsetY property has changed.
        /// </summary>
        public event EventHandler ContentOffsetYChanged;

        /// <summary>
        /// Event raised when the ContentScale property has changed.
        /// </summary>
        public event EventHandler ContentScaleChanged;

        /// <summary>
        /// Gets or sets the duration of the animations (in seconds) started by calling AnimatedZoomTo and the other animation methods.
        /// </summary>
        public double AnimationDuration
        {
            get
            {
                return (double)this.GetValue(AnimationDurationProperty);
            }

            set
            {
                this.SetValue(AnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X offset (in content coordinates) of the view on the content.
        /// </summary>
        public double ContentOffsetX
        {
            get
            {
                return (double)this.GetValue(ContentOffsetXProperty);
            }

            set
            {
                this.SetValue(ContentOffsetXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y offset (in content coordinates) of the view on the content.
        /// </summary>
        public double ContentOffsetY
        {
            get
            {
                return (double)this.GetValue(ContentOffsetYProperty);
            }

            set
            {
                this.SetValue(ContentOffsetYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current scale (or zoom factor) of the content.
        /// </summary>
        public double ContentScale
        {
            get
            {
                return (double)this.GetValue(ContentScaleProperty);
            }

            set
            {
                this.SetValue(ContentScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view port height, in content coordinates.
        /// </summary>
        public double ContentViewportHeight
        {
            get
            {
                return (double)this.GetValue(ContentViewportHeightProperty);
            }

            set
            {
                this.SetValue(ContentViewportHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view port width, in content coordinates.
        /// </summary>
        public double ContentViewportWidth
        {
            get
            {
                return (double)this.GetValue(ContentViewportWidthProperty);
            }

            set
            {
                this.SetValue(ContentViewportWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the content focus, this is the point that we are focusing on when zooming.
        /// </summary>
        public double ContentZoomFocusX
        {
            get
            {
                return (double)this.GetValue(ContentZoomFocusXProperty);
            }

            set
            {
                this.SetValue(ContentZoomFocusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the content focus, this is the point that we are focusing on when zooming.
        /// </summary>
        public double ContentZoomFocusY
        {
            get
            {
                return (double)this.GetValue(ContentZoomFocusYProperty);
            }

            set
            {
                this.SetValue(ContentZoomFocusYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates if mouse wheel scrolling on the zoom and pan control is allowed.
        /// This is set to 'false' by default.
        /// </summary>
        public bool IsMouseWheelScrollingEnabled
        {
            get
            {
                return (bool)this.GetValue(IsMouseWheelScrollingEnabledProperty);
            }

            set
            {
                this.SetValue(IsMouseWheelScrollingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for 'Content Scale'.
        /// </summary>
        public double MaxContentScale
        {
            get
            {
                return (double)this.GetValue(MaxContentScaleProperty);
            }

            set
            {
                this.SetValue(MaxContentScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for 'ContentScale'.
        /// </summary>
        public double MinContentScale
        {
            get
            {
                return (double)this.GetValue(MinContentScaleProperty);
            }

            set
            {
                this.SetValue(MinContentScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
        /// that the content focus point is locked to while zooming in.
        /// </summary>
        public double ViewportZoomFocusX
        {
            get
            {
                return (double)this.GetValue(ViewportZoomFocusXProperty);
            }

            set
            {
                this.SetValue(ViewportZoomFocusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the view port focus, this is the point in the view port (in viewport coordinates) 
        /// that the content focus point is locked to while zooming in.
        /// </summary>
        public double ViewportZoomFocusY
        {
            get
            {
                return (double)this.GetValue(ViewportZoomFocusYProperty);
            }

            set
            {
                this.SetValue(ViewportZoomFocusYProperty, value);
            }
        }

        /// <summary>
        /// Do animation that scales the content so that it fits completely in the control.
        /// </summary>
        /// <param name="padding">
        /// The padding.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PARTContent", Justification = "Known context word")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Multi-CLR Language not needed")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ZoomAndPanControl", Justification = "Known context word")]
        public void AnimatedScaleToFit(double padding = 0)
        {
            if (this.content == null)
            {
                throw new InvalidOperationException("Code Error: PART_Content was not found in the ZoomAndPanControl visual template!");
            }

            this.AnimatedZoomTo(new Rect(0, 0, this.content.ActualWidth + padding, this.content.ActualHeight + padding));
        }

        /// <summary>
        /// Use animation to center the view on the specified point (in content coordinates).
        /// </summary>
        /// <param name="contentPoint">
        /// The content Point.
        /// </param>
        public void AnimatedSnapTo(Point contentPoint)
        {
            double newX = contentPoint.X - (this.ContentViewportWidth / 2);
            double newY = contentPoint.Y - (this.ContentViewportHeight / 2);

            AnimationHelper.StartAnimation(this, ContentOffsetXProperty, newX, this.AnimationDuration);
            AnimationHelper.StartAnimation(this, ContentOffsetYProperty, newY, this.AnimationDuration);
        }

        /// <summary>
        /// Zoom in/out centered on the specified point (in content coordinates).
        /// The focus point is kept locked to it's on screen position (like google maps).
        /// </summary>
        /// <param name="newContentScale">
        /// The new Content Scale.
        /// </param>
        /// <param name="contentZoomFocus">
        /// The content Zoom Focus.
        /// </param>
        public void AnimatedZoomAboutPoint(double newContentScale, Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, this.MinContentScale), this.MaxContentScale);

            AnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

            this.ContentZoomFocusX = contentZoomFocus.X;
            this.ContentZoomFocusY = contentZoomFocus.Y;
            this.ViewportZoomFocusX = (this.ContentZoomFocusX - this.ContentOffsetX) * this.ContentScale;
            this.ViewportZoomFocusY = (this.ContentZoomFocusY - this.ContentOffsetY) * this.ContentScale;

            // When zooming about a point make updates to ContentScale also update content offset.
            this.enableContentOffsetUpdateFromScale = true;

            AnimationHelper.StartAnimation(
                this, 
                ContentScaleProperty, 
                newContentScale, 
                this.AnimationDuration, 
                delegate
                    {
                        this.enableContentOffsetUpdateFromScale = false;

                        this.ResetViewportZoomFocus();
                    });
        }

        /// <summary>
        /// Do an animated zoom to view a specific scale and rectangle (in content coordinates).
        /// </summary>
        /// <param name="newScale">
        /// The new Scale.
        /// </param>
        /// <param name="contentRect">
        /// The content rectangle.
        /// </param>
        public void AnimatedZoomTo(double newScale, Rect contentRect)
        {
            this.AnimatedZoomPointToViewportCenter(
                newScale, 
                new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)), 
                delegate
                    {
                        // At the end of the animation, ensure that we are snapped to the specified content offset.
                        // Due to zooming in on the content focus point and rounding errors, the content offset may
                        // be slightly off what we want at the end of the animation and this bit of code corrects it.
                        this.ContentOffsetX = contentRect.X;
                        this.ContentOffsetY = contentRect.Y;
                    });
        }

        /// <summary>
        /// Do an animated zoom to the specified rectangle (in content coordinates).
        /// </summary>
        /// <param name="contentRect">
        /// The content rectangle.
        /// </param>
        public void AnimatedZoomTo(Rect contentRect)
        {
            double scaleX = this.ContentViewportWidth / contentRect.Width;
            double scaleY = this.ContentViewportHeight / contentRect.Height;
            double newScale = this.ContentScale * Math.Min(scaleX, scaleY);

            this.AnimatedZoomPointToViewportCenter(newScale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)), null);
        }

        /// <summary>
        /// Zoom in/out centered on the viewport center.
        /// </summary>
        /// <param name="contentScale">
        /// The content Scale.
        /// </param>
        public void AnimatedZoomTo(double contentScale)
        {
            var zoomCenter = new Point(this.ContentOffsetX + (this.ContentViewportWidth / 2), this.ContentOffsetY + (this.ContentViewportHeight / 2));
            this.AnimatedZoomAboutPoint(contentScale, zoomCenter);
        }

        /// <summary>
        /// Called when a template has been applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.content = this.Template.FindName("PART_Content", this) as FrameworkElement;
            if (this.content != null)
            {
                // Setup the transform on the content so that we can scale it by 'ContentScale'.
                this.contentScaleTransform = new ScaleTransform(this.ContentScale, this.ContentScale);

                // Setup the transform on the content so that we can translate it by 'ContentOffsetX' and 'ContentOffsetY'.
                this.contentOffsetTransform = new TranslateTransform();
                this.UpdateTranslationX();
                this.UpdateTranslationY();

                // Setup a transform group to contain the translation and scale transforms, and then
                // assign this to the content's 'RenderTransform'.
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(this.contentOffsetTransform);
                transformGroup.Children.Add(this.contentScaleTransform);
                this.content.RenderTransform = transformGroup;
            }
        }

        /// <summary>
        /// Instantly scale the content so that it fits completely in the control.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PARTContent", Justification = "Known context word")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ZoomAndPanControl", Justification = "Known context word")]
        public void ScaleToFit()
        {
            if (this.content == null)
            {
                throw new InvalidOperationException("Code Error: PART_Content was not found in the ZoomAndPanControl visual template!");
            }

            this.ZoomTo(new Rect(0, 0, this.content.ActualWidth, this.content.ActualHeight));
        }

        /// <summary>
        /// Instantly center the view on the specified point (in content coordinates).
        /// </summary>
        /// <param name="contentOffset">
        /// The content Offset.
        /// </param>
        public void SnapContentOffsetTo(Point contentOffset)
        {
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            this.ContentOffsetX = contentOffset.X;
            this.ContentOffsetY = contentOffset.Y;
        }

        /// <summary>
        /// Instantly center the view on the specified point (in content coordinates).
        /// </summary>
        /// <param name="contentPoint">
        /// The content Point.
        /// </param>
        public void SnapTo(Point contentPoint)
        {
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            this.ContentOffsetX = contentPoint.X - (this.ContentViewportWidth / 2);
            this.ContentOffsetY = contentPoint.Y - (this.ContentViewportHeight / 2);
        }

        /// <summary>
        /// Zoom in/out centered on the specified point (in content coordinates).
        /// The focus point is kept locked to it's on screen position (like google maps).
        /// </summary>
        /// <param name="newContentScale">
        /// The new Content Scale.
        /// </param>
        /// <param name="contentZoomFocus">
        /// The content Zoom Focus.
        /// </param>
        public void ZoomAboutPoint(double newContentScale, Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, this.MinContentScale), this.MaxContentScale);

            double screenSpaceZoomOffsetX = (contentZoomFocus.X - this.ContentOffsetX) * this.ContentScale;
            double screenSpaceZoomOffsetY = (contentZoomFocus.Y - this.ContentOffsetY) * this.ContentScale;
            double contentSpaceZoomOffsetX = screenSpaceZoomOffsetX / newContentScale;
            double contentSpaceZoomOffsetY = screenSpaceZoomOffsetY / newContentScale;
            double newContentOffsetX = contentZoomFocus.X - contentSpaceZoomOffsetX;
            double newContentOffsetY = contentZoomFocus.Y - contentSpaceZoomOffsetY;

            AnimationHelper.CancelAnimation(this, ContentScaleProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            this.ContentScale = newContentScale;
            this.ContentOffsetX = newContentOffsetX;
            this.ContentOffsetY = newContentOffsetY;
        }

        /// <summary>
        /// Instantly zoom to the specified rectangle (in content coordinates).
        /// </summary>
        /// <param name="contentRect">
        /// The content rectangle.
        /// </param>
        public void ZoomTo(Rect contentRect)
        {
            double scaleX = this.ContentViewportWidth / contentRect.Width;
            double scaleY = this.ContentViewportHeight / contentRect.Height;
            double newScale = this.ContentScale * Math.Min(scaleX, scaleY);

            this.ZoomPointToViewportCenter(newScale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)));
        }

        /// <summary>
        /// Zoom in/out centered on the viewport center.
        /// </summary>
        /// <param name="contentScale">
        /// The content Scale.
        /// </param>
        public void ZoomTo(double contentScale)
        {
            var zoomCenter = new Point(this.ContentOffsetX + (this.ContentViewportWidth / 2), this.ContentOffsetY + (this.ContentViewportHeight / 2));
            this.ZoomAboutPoint(contentScale, zoomCenter);
        }

        /// <summary>
        /// Arrange the control and it's children.
        /// </summary>
        /// <param name="arrangeBounds">
        /// The arrange Bounds.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(this.DesiredSize);

            if (this.content.DesiredSize != this.unscaledExtent)
            {
                // Use the size of the child as the un-scaled extent content.
                this.unscaledExtent = this.content.DesiredSize;

                if (this.ScrollOwner != null)
                {
                    this.ScrollOwner.InvalidateScrollInfo();
                }
            }

            // Update the size of the viewport onto the content based on the passed in 'arrangeBounds'.
            this.UpdateViewportSize(arrangeBounds);

            return size;
        }

        /// <summary>
        /// Measure the control and it's children.
        /// </summary>
        /// <param name="constraint">
        /// The constraint.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Size childSize;
            try
            {
                childSize = base.MeasureOverride(infiniteSize);
            }
            catch (XamlParseException)
            {
                // ignore - no size given assume 0
                childSize = Size.Empty;
            }

            if (childSize != this.unscaledExtent)
            {
                // Use the size of the child as the un-scaled extent content.
                this.unscaledExtent = childSize;

                if (this.ScrollOwner != null)
                {
                    this.ScrollOwner.InvalidateScrollInfo();
                }
            }

            // Update the size of the viewport onto the content based on the passed in 'constraint'.
            this.UpdateViewportSize(constraint);

            double width = constraint.Width;
            double height = constraint.Height;

            if (double.IsInfinity(width))
            {
                // Make sure we don't return infinity!
                width = childSize.Width;
            }

            if (double.IsInfinity(height))
            {
                // Make sure we don't return infinity!
                height = childSize.Height;
            }

            this.UpdateTranslationX();
            this.UpdateTranslationY();

            return new Size(width, height);
        }

        /// <summary>
        /// Method called to clamp the 'ContentOffsetX' value to its valid range.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="baseValue">
        /// The base Value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object ContentOffsetXCoerce(DependencyObject d, object baseValue)
        {
            var c = (ZoomAndPanControl)d;
            var value = (double)baseValue;
            const double MinOffsetX = 0.0;
            double maxOffsetX = Math.Max(0.0, c.unscaledExtent.Width - c.constrainedContentViewportWidth);
            value = Math.Min(Math.Max(value, MinOffsetX), maxOffsetX);
            return value;
        }

        /// <summary>
        /// Event raised when the 'ContentOffsetX' property has changed value.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ContentOffsetXPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;

            c.UpdateTranslationX();

            if (!c.disableContentFocusSync)
            {
                // Normally want to automatically update content focus when content offset changes.
                // Although this is disabled using 'disableContentFocusSync' when content offset changes due to in-progress zooming.
                c.UpdateContentZoomFocusX();
            }

            if (c.ContentOffsetXChanged != null)
            {
                // Raise an event to let users of the control know that the content offset has changed.
                c.ContentOffsetXChanged(c, EventArgs.Empty);
            }

            if (!c.disableScrollOffsetSync && c.ScrollOwner != null)
            {
                // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
                c.ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <summary>
        /// Method called to clamp the 'ContentOffsetY' value to its valid range.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="baseValue">
        /// The base Value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object ContentOffsetYCoerce(DependencyObject d, object baseValue)
        {
            var c = (ZoomAndPanControl)d;
            var value = (double)baseValue;
            const double MinOffsetY = 0.0;
            double maxOffsetY = Math.Max(0.0, c.unscaledExtent.Height - c.constrainedContentViewportHeight);
            value = Math.Min(Math.Max(value, MinOffsetY), maxOffsetY);
            return value;
        }

        /// <summary>
        /// Event raised when the 'ContentOffsetY' property has changed value.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ContentOffsetYPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;

            c.UpdateTranslationY();

            if (!c.disableContentFocusSync)
            {
                // Normally want to automatically update content focus when content offset changes.
                // Although this is disabled using 'disableContentFocusSync' when content offset changes due to in-progress zooming.
                c.UpdateContentZoomFocusY();
            }

            if (c.ContentOffsetYChanged != null)
            {
                // Raise an event to let users of the control know that the content offset has changed.
                c.ContentOffsetYChanged(c, EventArgs.Empty);
            }

            if (!c.disableScrollOffsetSync && c.ScrollOwner != null)
            {
                // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
                c.ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <summary>
        /// Method called to clamp the 'ContentScale' value to its valid range.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="baseValue">
        /// The base Value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object ContentScaleCoerce(DependencyObject d, object baseValue)
        {
            var c = (ZoomAndPanControl)d;
            var value = (double)baseValue;
            value = Math.Min(Math.Max(value, c.MinContentScale), c.MaxContentScale);
            return value;
        }

        /// <summary>
        /// Event raised when the 'ContentScale' property has changed value.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ContentScalePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;

            if (c.contentScaleTransform != null)
            {
                // Update the content scale transform whenever 'ContentScale' changes.
                c.contentScaleTransform.ScaleX = c.ContentScale;
                c.contentScaleTransform.ScaleY = c.ContentScale;
            }

            // Update the size of the viewport in content coordinates.
            c.UpdateContentViewportSize();

            if (c.enableContentOffsetUpdateFromScale)
            {
                try
                {
                    // Disable content focus synchronization.  We are about to update content offset whilst zooming
                    // to ensure that the viewport is focused on our desired content focus point.  Setting this
                    // to 'true' stops the automatic update of the content focus when content offset changes.
                    c.disableContentFocusSync = true;

                    // Whilst zooming in or out keep the content offset up-to-date so that the viewport is always
                    // focused on the content focus point (and also so that the content focus is locked to the 
                    // viewport focus point - this is how the google maps style zooming works).
                    double viewportOffsetX = c.ViewportZoomFocusX - (c.ViewportWidth / 2);
                    double viewportOffsetY = c.ViewportZoomFocusY - (c.ViewportHeight / 2);
                    double contentOffsetX = viewportOffsetX / c.ContentScale;
                    double contentOffsetY = viewportOffsetY / c.ContentScale;
                    c.ContentOffsetX = (c.ContentZoomFocusX - (c.ContentViewportWidth / 2)) - contentOffsetX;
                    c.ContentOffsetY = (c.ContentZoomFocusY - (c.ContentViewportHeight / 2)) - contentOffsetY;
                }
                finally
                {
                    c.disableContentFocusSync = false;
                }
            }

            if (c.ContentScaleChanged != null)
            {
                c.ContentScaleChanged(c, EventArgs.Empty);
            }

            if (c.ScrollOwner != null)
            {
                c.ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <summary>
        /// Event raised 'MinContentScale' or 'MaxContentScale' has changed.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void MinOrMaxContentScalePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanControl)o;
            c.ContentScale = Math.Min(Math.Max(c.ContentScale, c.MinContentScale), c.MaxContentScale);
        }

        /// <summary>
        /// Zoom to the specified scale and move the specified focus point to the center of the viewport.
        /// </summary>
        /// <param name="newContentScale">
        /// The new Content Scale.
        /// </param>
        /// <param name="contentZoomFocus">
        /// The content Zoom Focus.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        private void AnimatedZoomPointToViewportCenter(double newContentScale, Point contentZoomFocus, EventHandler callback)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, this.MinContentScale), this.MaxContentScale);

            AnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
            AnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

            this.ContentZoomFocusX = contentZoomFocus.X;
            this.ContentZoomFocusY = contentZoomFocus.Y;
            this.ViewportZoomFocusX = (this.ContentZoomFocusX - this.ContentOffsetX) * this.ContentScale;
            this.ViewportZoomFocusY = (this.ContentZoomFocusY - this.ContentOffsetY) * this.ContentScale;

            // When zooming about a point make updates to ContentScale also update content offset.
            this.enableContentOffsetUpdateFromScale = true;

            AnimationHelper.StartAnimation(
                this, 
                ContentScaleProperty, 
                newContentScale, 
                this.AnimationDuration, 
                delegate
                    {
                        this.enableContentOffsetUpdateFromScale = false;

                        if (callback != null)
                        {
                            callback(this, EventArgs.Empty);
                        }
                    });

            AnimationHelper.StartAnimation(this, ViewportZoomFocusXProperty, this.ViewportWidth / 2, this.AnimationDuration);
            AnimationHelper.StartAnimation(this, ViewportZoomFocusYProperty, this.ViewportHeight / 2, this.AnimationDuration);
        }

        /// <summary>
        /// Reset the viewport zoom focus to the center of the viewport.
        /// </summary>
        private void ResetViewportZoomFocus()
        {
            this.ViewportZoomFocusX = this.ViewportWidth / 2;
            this.ViewportZoomFocusY = this.ViewportHeight / 2;
        }

        /// <summary>
        /// Update the size of the viewport in content coordinates after the viewport size or 'ContentScale' has changed.
        /// </summary>
        private void UpdateContentViewportSize()
        {
            this.ContentViewportWidth = this.ViewportWidth / this.ContentScale;
            this.ContentViewportHeight = this.ViewportHeight / this.ContentScale;

            this.constrainedContentViewportWidth = Math.Min(this.ContentViewportWidth, this.unscaledExtent.Width);
            this.constrainedContentViewportHeight = Math.Min(this.ContentViewportHeight, this.unscaledExtent.Height);

            this.UpdateTranslationX();
            this.UpdateTranslationY();
        }

        /// <summary>
        /// Update the X coordinate of the zoom focus point in content coordinates.
        /// </summary>
        private void UpdateContentZoomFocusX()
        {
            this.ContentZoomFocusX = this.ContentOffsetX + (this.constrainedContentViewportWidth / 2);
        }

        /// <summary>
        /// Update the Y coordinate of the zoom focus point in content coordinates.
        /// </summary>
        private void UpdateContentZoomFocusY()
        {
            this.ContentZoomFocusY = this.ContentOffsetY + (this.constrainedContentViewportHeight / 2);
        }

        /// <summary>
        /// Update the X coordinate of the translation transformation.
        /// </summary>
        private void UpdateTranslationX()
        {
            if (this.contentOffsetTransform != null)
            {
                double scaledContentWidth = this.unscaledExtent.Width * this.ContentScale;
                if (scaledContentWidth < this.ViewportWidth)
                {
                    // When the content can fit entirely within the viewport, center it.
                    this.contentOffsetTransform.X = (this.ContentViewportWidth - this.unscaledExtent.Width) / 2;
                }
                else
                {
                    this.contentOffsetTransform.X = -this.ContentOffsetX;
                }
            }
        }

        /// <summary>
        /// Update the Y coordinate of the translation transformation.
        /// </summary>
        private void UpdateTranslationY()
        {
            if (this.contentOffsetTransform != null)
            {
                double scaledContentHeight = this.unscaledExtent.Height * this.ContentScale;
                if (scaledContentHeight < this.ViewportHeight)
                {
                    // When the content can fit entirely within the viewport, center it.
                    this.contentOffsetTransform.Y = (this.ContentViewportHeight - this.unscaledExtent.Height) / 2;
                }
                else
                {
                    this.contentOffsetTransform.Y = -this.ContentOffsetY;
                }
            }
        }

        /// <summary>
        /// Update the viewport size from the specified size.
        /// </summary>
        /// <param name="newSize">
        /// The new Size.
        /// </param>
        private void UpdateViewportSize(Size newSize)
        {
            if (this.viewport == newSize)
            {
                // The viewport is already the specified size.
                return;
            }

            this.viewport = newSize;

            // Update the viewport size in content coordiates.
            this.UpdateContentViewportSize();

            // Initialise the content zoom focus point.
            this.UpdateContentZoomFocusX();
            this.UpdateContentZoomFocusY();

            // Reset the viewport zoom focus to the center of the viewport.
            this.ResetViewportZoomFocus();

            // Update content offset from itself when the size of the viewport changes.
            // This ensures that the content offset remains properly clamped to its valid range.
            this.ContentOffsetX = this.ContentOffsetX;
            this.ContentOffsetY = this.ContentOffsetY;

            if (this.ScrollOwner != null)
            {
                // Tell that owning ScrollViewer that scrollbar data has changed.
                this.ScrollOwner.InvalidateScrollInfo();
            }
        }

        /// <summary>
        /// Zoom to the specified scale and move the specified focus point to the center of the viewport.
        /// </summary>
        /// <param name="newContentScale">
        /// The new Content Scale.
        /// </param>
        /// <param name="contentZoomFocus">
        /// The content Zoom Focus.
        /// </param>
        private void ZoomPointToViewportCenter(double newContentScale, Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, this.MinContentScale), this.MaxContentScale);

            AnimationHelper.CancelAnimation(this, ContentScaleProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            this.ContentScale = newContentScale;
            this.ContentOffsetX = contentZoomFocus.X - (this.ContentViewportWidth / 2);
            this.ContentOffsetY = contentZoomFocus.Y - (this.ContentViewportHeight / 2);
        }
    }
}