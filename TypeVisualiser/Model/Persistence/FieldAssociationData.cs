using TypeVisualiser.Properties;

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

        public static FieldAssociationData Convert(FieldAssociation association)
        {
            if (association == null)
            {
                throw new ArgumentNullResourceException("association", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            FieldAssociationData converted;
            if (association is StaticAssociation)
            {
                converted = new StaticAssociationData();
            } else if (association is ConsumeAssociation)
            {
                converted = new ConsumeAssociationData();
            } else
            {
                converted = new FieldAssociationData();
            }

            converted.Name = association.Name;
            converted.AssociatedTo = association.AssociatedTo.ExtractPersistentData();
            converted.UsageCount = association.UsageCount;
            return converted;
        }
    }
}