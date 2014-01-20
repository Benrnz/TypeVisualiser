namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    [Serializable]
    public class AssociationData
    {
        public VisualisableTypeData AssociatedTo { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool Show { get; set; }
    
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}: {1} {2}", this.GetType().Name, this.Name, this.AssociatedTo.Name);
        }
    }
}