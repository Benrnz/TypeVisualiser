using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TypeVisualiser.Model;
using TypeVisualiser.UI.WpfUtilities;

namespace TypeVisualiser.UI
{
    internal class ViewportControllerFilter
    {
        private readonly Func<ITrivialFilter> getTrivialFilter = () => TrivialFilter.Current;

        private Dispatcher dispatcher;

        public ViewportControllerFilter()
        {
            SecondaryAssociationElements = new Dictionary<string, DiagramElement>(); 
        }

        internal Dispatcher Dispatcher
        {
            get { return dispatcher ?? (this.dispatcher = Dispatcher.CurrentDispatcher); }
            set { this.dispatcher = value; }
        }

        internal IDictionary<string, DiagramElement> SecondaryAssociationElements { get; set; }

        internal IEnumerable<DiagramElement> DiagramElements { get; set; }

        internal bool ShouldThisSecondaryElementBeVisible(DiagramElement element, bool showSuggestion)
        {
            if (!showSuggestion)
            {
                return false;
            }

            if (SecondaryAssociationElements.ContainsKey(element.DiagramContent.Id))
            {
                return !this.getTrivialFilter().HideSecondaryAssociations;
            }

            return true;
        }

        internal void ApplyTypeFilter(IVisualisableTypeWithAssociations currentSubject)
        {
            if (currentSubject == null)
            {
                return;
            }

            Task.Factory.StartNew(() =>
                {
                    var filter = this.getTrivialFilter();
                    foreach (DiagramElement element in DiagramElements.ToList())
                    {
                        bool show = filter.IsVisible(element, !SecondaryAssociationElements.ContainsKey(element.DiagramContent.Id));
                        DiagramElement copyOfElement = element;
                        dispatcher.BeginInvoke(() => copyOfElement.Show = show, DispatcherPriority.Normal);
                    }

                    // This needs to happen because the filter only knows how to show/hide associations and secondary lines. The filter
                    // does not understand the dependency relationships between elements.
                    // The primary lines are shown/hidden based on events raised by the associations. Without doing the below refresh
                    // primary lines are not hidden sometimes.
                    foreach (DiagramElement element in DiagramElements.Where(e => e.DiagramContent is Association).ToList())
                    {
                        dispatcher.BeginInvoke(element.RefreshPosition, DispatcherPriority.Normal);
                    }
                });
        }

        public void Clear()
        {
            SecondaryAssociationElements.Clear();
            DiagramElements = null;
        }
    }
}
