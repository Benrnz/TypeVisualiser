using System;
using TypeVisualiser.Model;

namespace TypeVisualiser.Messaging
{
    public class DiagramElementChangedMessage : SingleDiagramOrientedMessage
    {
        public DiagramElementChangedMessage(Guid diagramId) : base(diagramId)
        {
        }

        public DiagramElement ChangedElement { get; set; }
    }
}