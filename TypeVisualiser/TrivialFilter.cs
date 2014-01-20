using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TypeVisualiser.Model;
using TypeVisualiser.Properties;

namespace TypeVisualiser
{
    public class TrivialFilter : ITrivialFilter
    {
        public const string TrivialListXmlFileName = "TrivialExcludeList.xml";
        private readonly Func<List<TrivialType>> deserialiseXmlIntoCollection;
        private readonly Action<string> editTrivialList;
        private string applicationFolder;
        private bool hasLoadedTypes;
        private bool hideTrivialTypes;
        private List<TrivialType> trivialTypes = new List<TrivialType>();

        public TrivialFilter()
        {
            this.deserialiseXmlIntoCollection = LoadTrivialExcludeXml;
            this.editTrivialList = filename => Process.Start("notepad.exe", filename);
            HideSecondaryAssociations = true;
        }

        public bool HideSecondaryAssociations { get; set; }
        public bool HideSystemTypes { get; set; }

        private List<TrivialType> TrivialTypes
        {
            get
            {
                if (!this.hasLoadedTypes)
                {
                    this.trivialTypes = this.deserialiseXmlIntoCollection();                    
                }

                return this.trivialTypes;
            }
        } 

        public bool HideTrivialTypes
        {
            get { return this.hideTrivialTypes; }
            set
            {
                this.hideTrivialTypes = value;
                if (!this.hasLoadedTypes)
                {
                    this.trivialTypes = this.deserialiseXmlIntoCollection();
                }
            }
        }

        private string ApplicationFolder
        {
            get { return this.applicationFolder ?? (this.applicationFolder = Path.GetDirectoryName(GetType().Assembly.Location)); }
        }

        private string FullTrivialListXmlFileName
        {
            get { return Path.Combine(ApplicationFolder, TrivialListXmlFileName); }
        }

        public void AddToTrivialTypeList(IVisualisableType type)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            TrivialTypes.Add(new ExactMatch { FullTypeName = type.NamespaceQualifiedName });
            SaveTrivialExcludeXml();
        }

        public void EditTrivialList()
        {
            this.editTrivialList(FullTrivialListXmlFileName);
        }

        /// <summary>
        /// Determines if the given type is deemed a trivial type.
        /// </summary>
        /// <param name="namespaceQualifiedName">The full type name.</param>
        /// <returns>
        /// Returns <c>true</c> if given type is a trivial type, otherwise <c>false</c>
        /// </returns>
        public bool IsTrivialType(string namespaceQualifiedName)
        {
            if (string.IsNullOrEmpty(namespaceQualifiedName))
            {
                return false;
            }

            bool retval = TrivialTypes.Any(x => x.IsMatch(namespaceQualifiedName));
            return retval;
        }

        /// <summary>
        /// Determines whether the specified element is visible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="primaryDiagramElement">if set to <c>true</c> [primary diagram element].</param>
        /// <returns>
        ///   <c>true</c> if the specified element is visible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsVisible(DiagramElement element, bool primaryDiagramElement)
        {
            if (element == null || element.DiagramContent == null)
            {
                throw new ArgumentNullResourceException("element", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var association = element.DiagramContent as Association;
            bool shouldShow = true;
            if (association != null)
            {
                if (association is SubjectAssociation)
                {
                    // Subject should always be shown.
                    return true;
                }

                if (HideSystemTypes)
                {
                    shouldShow = !association.AssociatedTo.AssemblyQualifiedName.StartsWith("System.", StringComparison.Ordinal);
                }

                if (shouldShow && HideTrivialTypes)
                {
                    shouldShow = !IsTrivialType(association.AssociatedTo.NamespaceQualifiedName);
                }
            }

            if (shouldShow)
            {
                if (!primaryDiagramElement)
                {
                    if ((element.DiagramContent is ConnectionLine || element.DiagramContent is ArrowHead))
                    {
                        shouldShow = !HideSecondaryAssociations;
                    }
                }
            }

            return shouldShow;
        }

        private List<TrivialType> LoadTrivialExcludeXml()
        {
            this.hasLoadedTypes = true;
            if (!File.Exists(FullTrivialListXmlFileName))
            {
                return new List<TrivialType>();
            }

            var xmlSerialiser = new XmlSerializer(typeof(List<TrivialType>));
            List<TrivialType> output;
            using (var stream = new FileStream(FullTrivialListXmlFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                output = xmlSerialiser.Deserialize(stream) as List<TrivialType>;
            }

            return output;
        }

        private void SaveTrivialExcludeXml()
        {
            this.trivialTypes = TrivialTypes.OrderBy(x => x.ToString()).Select(x => x).ToList();
            var xmlSerialiser = new XmlSerializer(typeof(List<TrivialType>));
            using (var stream = new FileStream(FullTrivialListXmlFileName, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                xmlSerialiser.Serialize(stream, this.trivialTypes);
            }
        }
    }
}