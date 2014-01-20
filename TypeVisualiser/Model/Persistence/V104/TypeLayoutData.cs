namespace TypeVisualiser.Model.Persistence.V104
{
    using System;
    using System.Windows;
    using System.Xml.Serialization;

    [Serializable]
    public class TypeLayoutData
    {
        [XmlAttribute]
        public Guid Id { get; set; }

        public Point TopLeft { get; set; }

        [XmlAttribute]
        public bool Visible { get; set; }

        [XmlAttribute]
        public string FullName { get; set; }
    }
}