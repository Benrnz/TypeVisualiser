namespace TypeVisualiser.Messaging
{
    using System;
    using Model;

    public class NavigateToDiagramAssociationMessage : SingleDiagramOrientedMessage
    {
        public NavigateToDiagramAssociationMessage(Guid diagramId, IVisualisableType type) : base(diagramId)
        {
            DiagramType = type;
        }

        public IVisualisableType DiagramType { get; private set; }
    }
}