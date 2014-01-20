namespace TypeVisualiser.Model.Persistence.V104
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "VisualisableTypeDiagramFile", Namespace = "http://typevisualiser.rees.biz/v1_4")]
    public class TypeVisualiserLayoutFile
    {
        [XmlAttribute]
        public string AssemblyFileName { get; set; }

        [XmlAttribute]
        public string AssemblyFullName { get; set; }

        [XmlAttribute]
        public string FileVersion { get; set; }

        [XmlAttribute]
        public bool HideParents { get; set; }

        [XmlAttribute]
        public bool HideTrivialTypes { get; set; }

        public ViewportControllerSaveData ViewportSaveData { get; set; }
    }
}