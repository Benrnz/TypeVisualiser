namespace TypeVisualiser.Model.Persistence.V104
{
    using System;

    [Serializable]
    public class ViewportControllerSaveData
    {
        public CanvasLayoutData CanvasLayout { get; set; }

        public VisualisableTypeSubjectData Subject { get; set; }
    }
}
