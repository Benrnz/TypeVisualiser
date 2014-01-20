namespace TypeVisualiser.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    using TypeVisualiser.Model;
    using TypeVisualiser.UI.WpfUtilities;

    internal class ViewportControllerFilter
    {
        private readonly ITrivialFilter trivialFilter;

        private Dispatcher dispatcher;

        public ViewportControllerFilter(ITrivialFilter trivialFilter)
        {
            this.SecondaryAssociationElements = new Dictionary<string, DiagramElement>();
            this.trivialFilter = trivialFilter;
        }

        internal IEnumerable<DiagramElement> DiagramElements { get; set; }

        internal Dispatcher Dispatcher
        {
            get
            {
                return this.dispatcher ?? (this.dispatcher = Dispatcher.CurrentDispatcher);
            }

            set
            {
                this.dispatcher = value;
            }
        }

        internal IDictionary<string, DiagramElement> SecondaryAssociationElements { get; set; }

        public void Clear()
        {
            this.SecondaryAssociationElements.Clear();
            this.DiagramElements = null;
        }

        internal void ApplyTypeFilter(IVisualisableTypeWithAssociations currentSubject)
        {
            if (currentSubject == null)
            {
                return;
            }

            Task.Factory.StartNew(
                () =>
                    {
                        foreach (DiagramElement element in this.DiagramElements.ToList())
                        {
                            bool show = this.trivialFilter.IsVisible(element, !this.SecondaryAssociationElements.ContainsKey(element.DiagramContent.Id));
                            DiagramElement copyOfElement = element;
                            this.dispatcher.BeginInvoke(() => copyOfElement.Show = show, DispatcherPriority.Normal);
                        }

                        // This needs to happen because the filter only knows how to show/hide associations and secondary lines. The filter
                        // does not understand the dependency relationships between elements.
                        // The primary lines are shown/hidden based on events raised by the associations. Without doing the below refresh
                        // primary lines are not hidden sometimes.
                        foreach (DiagramElement element in this.DiagramElements.Where(e => e.DiagramContent is Association).ToList())
                        {
                            this.dispatcher.BeginInvoke(element.RefreshPosition, DispatcherPriority.Normal);
                        }
                    });
        }

        internal bool ShouldThisSecondaryElementBeVisible(DiagramElement element, bool showSuggestion)
        {
            if (!showSuggestion)
            {
                return false;
            }

            if (this.SecondaryAssociationElements.ContainsKey(element.DiagramContent.Id))
            {
                return !this.trivialFilter.HideSecondaryAssociations;
            }

            return true;
        }
    }
}