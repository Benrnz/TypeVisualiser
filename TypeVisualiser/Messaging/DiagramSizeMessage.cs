namespace TypeVisualiser.Messaging
{
    using System;
    using Geometry;

    public abstract class DiagramSizeMessage : SingleDiagramOrientedMessage
    {
        protected DiagramSizeMessage(Guid diagramId) : base(diagramId) { }

        public Area DiagramSize { get; set; }
    }
}