using System.Diagnostics;
using System.Linq;
using TypeVisualiser.Model;
using TypeVisualiser.Properties;

namespace TypeVisualiser.UI
{
    /// <summary>
    /// A tool class to provide searching and quering against a class diagram.
    /// </summary>
    public static class ClassDiagramSearchTool
    {
        /// <summary>
        /// Find the association a line or arrowhead being pointed at.
        /// </summary>
        /// <param name="lineOrArrowhead">A element containing a arrowhead or line</param>
        /// <returns>The <see cref="FieldAssociation"/> being pointed at by the line.</returns>
        public static FieldAssociation FindAssociationTarget(DiagramElement lineOrArrowhead)
        {
            if (lineOrArrowhead == null)
            {
                throw new ArgumentNullResourceException("lineOrArrowhead", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var line = lineOrArrowhead.DiagramContent as ConnectionLine;
            if (line != null)
            {
                return line.PointingAt as FieldAssociation;
            }

            line = lineOrArrowhead.RelatedDiagramElements.Single(x => x.DiagramContent is ConnectionLine).DiagramContent as ConnectionLine;
            Debug.Assert(line != null, "Code error: line == null");
            return line.PointingAt as FieldAssociation;
        }
    }
}