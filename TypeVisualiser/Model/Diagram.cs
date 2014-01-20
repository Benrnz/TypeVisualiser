namespace TypeVisualiser.Model
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    using TypeVisualiser.Properties;

    /// <summary>
    /// A class representing any diagram instance. Not just a type dependency diagram.
    /// </summary>
    public class Diagram : INotifyPropertyChanged
    {
        private const double MaximumScaleValue = 1.5;

        private const double MinimumDiagramDimensionX = 1000;

        private const double MinimumDiagramDimensionY = 600;

        private const double MinimumDimensionChange = 0.001;

        private const double MinimumScaleChange = 0.001;

        private const double MinimumScaleValue = 0.1;

        private double contentHeight;

        private double contentOffsetX;

        private double contentOffsetY;

        private double contentScale;

        private double contentViewportHeight;

        private double contentViewportWidth;

        private double contentWidth;

        private IDiagramController controller;

        public Diagram(IDiagramController content)
        {
            if (content == null)
            {
                throw new ArgumentNullResourceException("content", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            this.Id = Guid.NewGuid();
            this.Controller = content;
            this.Controller.SetHostDiagram(this);
            content.DiagramLoaded += this.OnDiagramLoaded;

            this.ContentHeight = MinimumDiagramDimensionY;
            this.ContentWidth = MinimumDiagramDimensionX;
            this.ContentScale = 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Caption
        {
            get
            {
                return this.controller.DiagramCaption;
            }
        }

        public Point Centre
        {
            get
            {
                return new Point(this.ContentWidth / 2, this.ContentHeight / 2);
            }
        }

        /// <summary>
        /// Gets or sets the height of the content. This is the diagram paper space.
        /// </summary>
        /// <value>
        /// The height of the content.
        /// </value>
        public double ContentHeight
        {
            get
            {
                return this.contentHeight;
            }

            set
            {
                if (value < MinimumDiagramDimensionY)
                {
                    value = MinimumDiagramDimensionY;
                }

                if (Math.Abs(this.contentHeight - value) > MinimumDimensionChange)
                {
                    this.contentHeight = value;
                    this.RaisePropertyChanged("ContentHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the content offset X. Viewport focal point.
        /// </summary>
        /// <value>
        /// The content offset X.
        /// </value>
        public double ContentOffsetX
        {
            get
            {
                return this.contentOffsetX;
            }

            set
            {
                this.contentOffsetX = value;
                this.RaisePropertyChanged("ContentOffsetX");
            }
        }

        /// <summary>
        /// Gets or sets the content offset Y. Viewport focal point.
        /// </summary>
        /// <value>
        /// The content offset Y.
        /// </value>
        public double ContentOffsetY
        {
            get
            {
                return this.contentOffsetY;
            }

            set
            {
                this.contentOffsetY = value;
                this.RaisePropertyChanged("ContentOffsetY");
            }
        }

        /// <summary>
        /// Gets or sets the content scale. The zoom scale as a fraction 0.1 to 1.5
        /// </summary>
        /// <value>
        /// The content scale.
        /// </value>
        public double ContentScale
        {
            get
            {
                return this.contentScale;
            }

            set
            {
                if (value > MaximumScaleValue)
                {
                    value = MaximumScaleValue;
                }
                else if (value < MinimumScaleValue)
                {
                    value = MinimumScaleValue;
                }

                if (Math.Abs(this.contentScale - value) > MinimumScaleChange)
                {
                    this.contentScale = value;
                    this.RaisePropertyChanged("ContentScale");
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the content viewport. This is the size of the window through which
        /// the diagram paper space is displayed. 
        /// </summary>
        /// <value>
        /// The height of the content viewport.
        /// </value>
        public double ContentViewportHeight
        {
            get
            {
                return this.contentViewportHeight;
            }

            set
            {
                this.contentViewportHeight = value;
                this.RaisePropertyChanged("ContentViewportHeight");
            }
        }

        /// <summary>
        /// Gets or sets the width of the content viewport. This is the size of the window through which
        /// the diagram paper space is displayed. 
        /// </summary>
        /// <value>
        /// The width of the content viewport.
        /// </value>
        public double ContentViewportWidth
        {
            get
            {
                return this.contentViewportWidth;
            }

            set
            {
                this.contentViewportWidth = value;
                this.RaisePropertyChanged("ContentViewportWidth");
            }
        }

        /// <summary>
        /// Gets or sets the width of the content. This is the diagram paper space.
        /// </summary>
        /// <value>
        /// The width of the content.
        /// </value>
        public double ContentWidth
        {
            get
            {
                return this.contentWidth;
            }

            set
            {
                if (value < MinimumDiagramDimensionX)
                {
                    value = MinimumDiagramDimensionX;
                }

                if (Math.Abs(this.contentWidth - value) > MinimumDimensionChange)
                {
                    this.contentWidth = value;
                    this.RaisePropertyChanged("ContentWidth");
                }
            }
        }

        public IDiagramController Controller
        {
            get
            {
                return this.controller;
            }

            private set
            {
                if (value != null)
                {
                    if (this.controller != null)
                    {
                        this.controller.PropertyChanged -= this.OnViewControllerChanged;
                    }

                    this.controller = value;
                    this.controller.DiagramId = this.Id;
                    this.controller.PropertyChanged += this.OnViewControllerChanged;
                    this.RaisePropertyChanged("Caption");
                    this.RaisePropertyChanged("Controller");
                    this.RaisePropertyChanged("FullName");
                }
            }
        }

        public string FullName
        {
            get
            {
                return this.controller.DiagramFullName;
            }
        }

        public Guid Id { get; private set; }

        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets the maximum scale. The maximum zoom scale allowed.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Used by data binding.")]
        public double MaximumScale
        {
            get
            {
                return MaximumScaleValue;
            }
        }

        /// <summary>
        /// Gets the minimum scale. The minimum zoom scale allowed.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Used by data binding.")]
        public double MinimumScale
        {
            get
            {
                return MinimumScaleValue;
            }
        }

        /// <summary>
        /// Centers the view port into the center of the diagram.
        /// </summary>
        public void CentreDiagram()
        {
            this.ContentOffsetX = this.Centre.X;
            this.contentOffsetY = this.Centre.Y;
        }

        public void ZoomToFit()
        {
            double unzoomedViewportHeight = this.ContentScale * this.ContentViewportHeight;
            double unzoomedViewportWidth = this.ContentScale * this.ContentViewportWidth;
            if (this.ContentHeight > unzoomedViewportHeight || this.ContentWidth > unzoomedViewportWidth)
            {
                // Paper is too big for viewport.
                double candidate1 = unzoomedViewportHeight / this.ContentHeight;
                double candidate2 = unzoomedViewportWidth / this.ContentWidth;
                const double Margin = 0.01;
                this.ContentScale = candidate1 < candidate2 ? candidate1 - Margin : candidate2 - Margin;
                return;
            }

            this.ContentScale = 1;
        }

        private void OnDiagramLoaded(object sender, EventArgs e)
        {
            this.controller.DiagramLoaded -= this.OnDiagramLoaded;
            this.IsLoaded = true;
        }

        private void OnViewControllerChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Subject")
            {
                this.RaisePropertyChanged("Caption");
                this.RaisePropertyChanged("FullName");
            }
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}