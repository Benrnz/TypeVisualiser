namespace TypeVisualiser.Model.Persistence.V103
{
    using System;

    [Serializable]
    public class ViewportControllerSaveData
    {
        public CanvasLayoutData CanvasLayout { get; set; }

        public VisualisableTypeSubjectData Subject { get; set; }
    }
}
