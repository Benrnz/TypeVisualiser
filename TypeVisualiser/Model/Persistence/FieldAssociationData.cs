namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Xml.Serialization;

    [XmlInclude(typeof(ConsumeAssociationData))]
    [XmlInclude(typeof(StaticAssociationData))]
    [Serializable]
    public class FieldAssociationData : AssociationData
    {
        [XmlAttribute]
        public int UsageCount { get; set; }
    }
}