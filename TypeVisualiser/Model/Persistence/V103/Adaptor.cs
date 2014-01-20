using System.Collections.ObjectModel;
using System.Diagnostics;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model.Persistence.V103
{
    public static class Adaptor
    {
        public static V104.TypeVisualiserLayoutFile Adapt(TypeVisualiserLayoutFile oldDiagram)
        {
            if (oldDiagram == null)
            {
                throw new ArgumentNullResourceException("oldDiagram", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            // Call down into the v102 adaptor to convert 102 or 103 to 104.
            var latestDiagramVersion = V102.Adaptor.Adapt(oldDiagram);
            
            // Convert annotations (new to 104)
            latestDiagramVersion.ViewportSaveData.CanvasLayout.Annotations = new Collection<V104.AnnotationData>();
            foreach (var oldAnnotationData in oldDiagram.ViewportSaveData.CanvasLayout.Annotations)
            {
                var newAnnotationData = new V104.AnnotationData { Text = oldAnnotationData.Text, TopLeft = oldAnnotationData.TopLeft };
                latestDiagramVersion.ViewportSaveData.CanvasLayout.Annotations.Add(newAnnotationData);
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(latestDiagramVersion.AssemblyFileName));

            return latestDiagramVersion;
        }
    }
}