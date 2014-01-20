namespace TypeVisualiser.Messaging
{
    using System;
    using GalaSoft.MvvmLight.Messaging;

    public abstract class SingleDiagramOrientedMessage : MessageBase
    {
        protected SingleDiagramOrientedMessage(Guid diagramId)
        {
            DiagramId = diagramId;
        }

        public Guid DiagramId { get; private set; }
    }
}