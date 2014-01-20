namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Windows;
    using System.Xml.Serialization;

    [Serializable]
    public class TypeLayoutData
    {
        [XmlAttribute]
        public string Id { get; set; }

        public Point TopLeft { get; set; }

        [XmlAttribute]
        public bool Visible { get; set; }

        [XmlAttribute]
        public string ContentType { get; set; }

        [XmlText]
        public string Data { get; set; }
    }
}