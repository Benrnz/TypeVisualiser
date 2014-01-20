namespace TypeVisualiser.Model.Persistence.V103
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(StaticAssociationData))]
    public class ConsumeAssociationData : FieldAssociationData
    {
    }
}