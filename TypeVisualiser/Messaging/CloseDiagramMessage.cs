namespace TypeVisualiser.Messaging
{
    using System;

    public class CloseDiagramMessage : SingleDiagramOrientedMessage
    {
        public CloseDiagramMessage(Guid diagramId) : base(diagramId) { }
    }
}