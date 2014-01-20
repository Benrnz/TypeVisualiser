namespace TypeVisualiser.Model.Persistence.V103
{
    using System;
    using System.Xml.Serialization;
    using Properties;

    public class ModifiersData
    {
        public ModifiersData()
        {
        }

        public ModifiersData(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            bool isClass = false;
            bool isEnum = type.IsEnum;
            bool isInterface = type.IsInterface;
            bool isValueType = type.IsValueType;
            bool isAbstract = type.IsAbstract;

            if (type.IsClass)
            {
                ShowConstructors = true;
                isClass = true;
            }

            Static = isAbstract & type.IsSealed & isClass;

            if (isClass && !Static)
            {
                Abstract = isAbstract; // Only set Abstract to true if it is also not static (all static classes are considered abstract by the compiler but not in an OO sense).
                Sealed = type.IsSealed;
            }

            if (!type.IsPublic)
            {
                if (type.IsNestedPrivate)
                {
                    Private = true;
                } else
                {
                    Internal = true;
                }
            }

            if (isClass)
            {
                Kind = TypeKind.Class;
            } else if (isInterface)
            {
                Kind = TypeKind.Interface;
            } else if (isEnum)
            {
                Kind = TypeKind.Enum;
            } else if (isValueType)
            {
                Kind = TypeKind.ValueType;
            } else
            {
                Kind = TypeKind.Unknown;
            }
        }

        [XmlAttribute]
        public bool Abstract { get; set; }

        [XmlAttribute]
        public bool Internal { get; set; }

        [XmlAttribute]
        public TypeKind Kind { get; set; }

        [XmlAttribute]
        public bool Private { get; set; }

        [XmlAttribute]
        public bool Sealed { get; set; }

        [XmlAttribute]
        public bool ShowConstructors { get; set; }

        [XmlAttribute]
        public bool Static { get; set; }

        [XmlIgnore]
        public string TypeTextName
        {
            get
            {
                switch (Kind)
                {
                    case TypeKind.Class:
                        return "class";
                    case TypeKind.Interface:
                        return "interface";
                    case TypeKind.Enum:
                        return "enumeration";
                    case TypeKind.ValueType:
                        return "structure";
                    default:
                        return "unknown-type";
                }
            }
        }
    }
}