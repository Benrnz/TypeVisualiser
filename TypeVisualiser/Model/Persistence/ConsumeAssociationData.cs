namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(StaticAssociationData))]
    public class ConsumeAssociationData : FieldAssociationData
    {
    }
}