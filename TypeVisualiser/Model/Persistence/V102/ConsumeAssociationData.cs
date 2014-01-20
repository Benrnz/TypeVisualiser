namespace TypeVisualiser.Model.Persistence.V102
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(StaticAssociationData))]
    public class ConsumeAssociationData : FieldAssociationData
    {
    }
}