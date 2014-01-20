namespace TypeVisualiser.Model.Persistence.V104
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(StaticAssociationData))]
    public class ConsumeAssociationData : FieldAssociationData
    {
    }
}