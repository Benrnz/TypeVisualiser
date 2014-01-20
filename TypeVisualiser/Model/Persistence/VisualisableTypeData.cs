namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class VisualisableTypeData
    {
        public VisualisableTypeData()
        {
            LinesOfCode = -1;
        }

        [XmlAttribute]
        public string AssemblyFileName { get; set; }

        [XmlAttribute]
        public string AssemblyFullName { get; set; }

        [XmlAttribute]
        public string AssemblyName { get; set; }

        [XmlAttribute]
        public int ConstructorCount { get; set; }

        [XmlAttribute]
        public int EnumMemberCount { get; set; }

        [XmlAttribute]
        public int EventCount { get; set; }

        [XmlAttribute]
        public int FieldCount { get; set; }

        [XmlAttribute]
        public string FullName { get; set; }

        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public int LinesOfCode { get; set; }

        [XmlAttribute]
        public string LinesOfCodeToolTip { get; set; }

        [XmlAttribute]
        public int MethodCount { get; set; }

        public ModifiersData Modifiers { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Namespace { get; set; }

        [XmlAttribute]
        public int PropertyCount { get; set; }

        [XmlAttribute]
        public bool Show { get; set; }

        [XmlAttribute]
        public SubjectOrAssociate SubjectOrAssociate { get; set; }

        [XmlAttribute]
        public string ToolTipName { get; set; }
    }
}