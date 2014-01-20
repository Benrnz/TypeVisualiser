namespace TypeVisualiser
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(ExactMatch))]
    [XmlInclude(typeof(RegexMatch))]
    public abstract class TrivialType
    {
        public abstract bool IsMatch(string fullTypeName);
    }

    [Serializable]
    public class ExactMatch : TrivialType
    {
        [XmlAttribute]
        public string FullTypeName { get; set; }

        public override bool IsMatch(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
            {
                return false;
            }

            return 0 == string.Compare(fullTypeName, this.FullTypeName, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return this.FullTypeName;
        }
    }

    [Serializable]
    public class RegexMatch : TrivialType
    {
        private Regex regex;
        private string regexString;

        [XmlAttribute]
        public string RegexString
        {
            get
            {
                return this.regexString;
            }

            set
            {
                if (this.regexString != value)
                {
                    this.regexString = value;
                    this.regex = new Regex(value);
                }
            }
        }

        [XmlIgnore]
        private Regex Regex
        {
            get
            {
                return this.regex ?? (this.regex = new Regex(this.RegexString));
            }
        }

        public override bool IsMatch(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
            {
                return false;
            }

            return this.Regex.IsMatch(fullTypeName);
        }

        public override string ToString()
        {
            return this.RegexString;
        }
    }
}