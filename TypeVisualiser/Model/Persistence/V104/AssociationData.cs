namespace TypeVisualiser.Model.Persistence.V104
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class AssociationData
    {
        public VisualisableTypeData AssociatedTo { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool Show { get; set; }
    }
}