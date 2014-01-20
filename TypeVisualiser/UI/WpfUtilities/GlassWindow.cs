namespace TypeVisualiser.UI.WpfUtilities
{
    using System.Windows;

    /// <summary>
    /// Something hjh
    /// </summary>
    public class GlassWindow : Window
    {
        /// <summary>
        /// Dependency porperty for <see cref="GlassThickness"/>
        /// </summary>
        public static readonly DependencyProperty GlassThicknessProperty = DependencyProperty.Register(
            "GlassThickness",
            typeof(Thickness),
            typeof(GlassWindow),
            new PropertyMetadata(GlassThicknessChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="GlassWindow"/> class.
        /// </summary>
        protected GlassWindow()
        {
        }

        /// <summary>         
        /// Gets or sets the Local property for Glass thickness.         
        /// </summary>         
        public Thickness GlassThickness
        {
            get
            {
                return (Thickness)GetValue(GlassThicknessProperty);
            }
            set
            {
                SetValue(GlassThicknessProperty, value);
            }
        }

        private static void GlassThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var windowBase = d as GlassWindow;
            if (windowBase == null)
            {
                return;
            }

            if (windowBase.IsLoaded)
            {
                DesktopWindowManagerApi.ExtendFrameIntoClientArea(windowBase, (Thickness)e.NewValue);
            } else
            {
                windowBase.Loaded += windowBase.GlassWindowLoaded;
            }
        }

        private void GlassWindowLoaded(object sender, RoutedEventArgs e)
        {
            DesktopWindowManagerApi.ExtendFrameIntoClientArea(this, GlassThickness);
        }
    }
}