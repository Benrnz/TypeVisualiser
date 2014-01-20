namespace TypeVisualiser.Model.Persistence.V104
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    [Serializable]
    public class VisualisableTypeSubjectData : VisualisableTypeData
    {
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Arrays work better for xml serialisation. This class is only used for serialisation.")]
        [XmlArrayItem(typeof(FieldAssociationData))]
        [XmlArrayItem(typeof(StaticAssociationData))]
        [XmlArrayItem(typeof(ConsumeAssociationData))]
        public FieldAssociationData[] Associations { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Arrays work better for xml serialisation. This class is only used for serialisation.")]
        public ParentAssociationData[] Implements { get; set; }

        public ParentAssociationData Parent { get; set; }
    }
}