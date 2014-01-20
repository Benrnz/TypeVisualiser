namespace TypeVisualiser.Messaging
{
    using System;

    public class GetDiagramSizeMessage : DiagramSizeMessage
    {
        public GetDiagramSizeMessage(Guid diagramId) : base(diagramId) { }
    }
}