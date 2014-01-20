namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A stripped down version of a <see cref="VisualisableTypeWithAssociations"/> object for persistence.
    /// This class hasn't been renamed to VisualisableTypeWithAssociationsData for backwards compatibility reasons.
    /// </summary>
    [Serializable]
    public class VisualisableTypeSubjectData : VisualisableTypeData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Arrays work better for xml serialisation. This class is only used for serialisation.")]
        [XmlArrayItem(typeof(FieldAssociationData))]
        [XmlArrayItem(typeof(StaticAssociationData))]
        [XmlArrayItem(typeof(ConsumeAssociationData))]
        public FieldAssociationData[] Associations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Arrays work better for xml serialisation. This class is only used for serialisation.")]
        public ParentAssociationData[] Implements { get; set; }

        public ParentAssociationData Parent { get; set; }
    }
}