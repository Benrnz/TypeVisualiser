using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TypeVisualiser.Model.Persistence.V102;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model.Persistence
{
    public class ModelSerialiser
    {
        private bool errorsOccurred;

        /// <summary>
        /// Deserialises the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The deserialised diagram data.</returns>
        /// <exception cref="IOException">Will be thrown when file access problems occur.</exception>
        /// <exception cref="XmlException">Will be thrown if there is any problem serialising the data.</exception>
        public virtual TypeVisualiserLayoutFile Deserialise(string fileName)
        {
            this.errorsOccurred = false;
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                return Deserialise(stream);
            }
        }

        /// <summary>
        /// Serialises the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="layoutData">The diagram data.</param>
        /// <exception cref="IOException">Will be thrown when file access problems occur.</exception>
        /// <exception cref="XmlException">Will be thrown if there is any problem serialising the data.</exception>
        public virtual void Serialise(string fileName, IPersistentFileData layoutData)
        {
            if (layoutData == null)
            {
                throw new ArgumentNullResourceException("layoutData", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var serialiser = new XmlSerializer(layoutData.GetType());
            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                serialiser.Serialize(writer, layoutData);
            }
        }

        private XmlSerializer CreateSerialiser<T>()
        {
            var serialiser = new XmlSerializer(typeof (T));
            serialiser.UnknownNode += SerialiserOnUnknownNode;
            serialiser.UnreferencedObject += SerialiserOnUnreferencedObject;
            return serialiser;
        }

        private static TypeVisualiserLayoutFile Deserialise101()
        {
            throw new NotSupportedException();
        }

        private TypeVisualiserLayoutFile Deserialise102(Stream xmlSource)
        {
            xmlSource.Position = 0;
            XmlSerializer serialiser = CreateSerialiser<V102.TypeVisualiserLayoutFile>();
            var oldDiagramType = serialiser.Deserialize(xmlSource) as V102.TypeVisualiserLayoutFile;
            if (this.errorsOccurred)
            {
                throw new NotSupportedException("The TVD file provided is an invalid and unsupported format. Unable to load this diagram (v1.02).");
            }

            V104.TypeVisualiserLayoutFile diagramv104 = Adaptor.Adapt(oldDiagramType);
            return V104.Adaptor.Adapt(diagramv104);
        }

        private TypeVisualiserLayoutFile Deserialise103(Stream xmlSource)
        {
            xmlSource.Position = 0;
            XmlSerializer serialiser = CreateSerialiser<V103.TypeVisualiserLayoutFile>();
            var oldDiagramType = serialiser.Deserialize(xmlSource) as V103.TypeVisualiserLayoutFile;
            if (this.errorsOccurred)
            {
                throw new NotSupportedException("The TVD file provided is an invalid and unsupported format. Unable to load this diagram (v1.03).");
            }

            V104.TypeVisualiserLayoutFile diagramv104 = V103.Adaptor.Adapt(oldDiagramType);
            return V104.Adaptor.Adapt(diagramv104);
        }

        private TypeVisualiserLayoutFile Deserialise104(Stream xmlSource)
        {
            xmlSource.Position = 0;
            XmlSerializer serialiser = CreateSerialiser<V104.TypeVisualiserLayoutFile>();
            var oldDiagramType = serialiser.Deserialize(xmlSource) as V104.TypeVisualiserLayoutFile;
            if (this.errorsOccurred)
            {
                throw new NotSupportedException("The TVD file provided is an invalid and unsupported format. Unable to load this diagram (v1.04).");
            }

            return V104.Adaptor.Adapt(oldDiagramType);
        }

        private TypeVisualiserLayoutFile Deserialise105(Stream xmlSource)
        {
            var serialiser = CreateSerialiser<TypeVisualiserLayoutFile>();
            var result = serialiser.Deserialize(xmlSource) as TypeVisualiserLayoutFile;
            if (this.errorsOccurred)
            {
                throw new NotSupportedException("The TVD file provided is an invalid and unsupported format. Unable to load this diagram (v1.05).");
            }

            return result;
        }

        private TypeVisualiserLayoutFile DeserialiseBasedOnVersion(Stream xmlSource)
        {
            xmlSource.Position = 0;
            XDocument document = XDocument.Load(xmlSource);
            if (document.Root == null)
            {
                throw new NotSupportedException("The TVD file provided is an invalid and unsupported format. Unable to load this diagram.");
            }

            XNamespace diagramVersion = document.Root.GetDefaultNamespace();
            switch (diagramVersion.NamespaceName)
            {
                case "http://typevisualiser.rees.biz/v1_1":
                    return Deserialise101();
                case "http://typevisualiser.rees.biz/v1_2":
                    return Deserialise102(xmlSource);
                case "http://typevisualiser.rees.biz/v1_3":
                    return Deserialise103(xmlSource);
                case "http://typevisualiser.rees.biz/v1_4":
                    return Deserialise104(xmlSource);
            }

            throw new NotSupportedException("The TVD diagram file you are attempting to open is too old and no longer supported.");
        }

        private void SerialiserOnUnknownNode(object sender, XmlNodeEventArgs e)
        {
            this.errorsOccurred = true;
            Logger.Instance.WriteEntry("ModelDeserialiser: An error occured attempting to deserialise a TVD Xml document:");
            Logger.Instance.WriteEntry("    Line Number: " + e.LineNumber + " Position: " + e.LinePosition);
            Logger.Instance.WriteEntry("    Object being deserialised: " + e.ObjectBeingDeserialized);
        }

        private void SerialiserOnUnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            this.errorsOccurred = true;
            Logger.Instance.WriteEntry("ModelDeserialiser: An error occured attempting to deserialise a TVD Xml document:");
            Logger.Instance.WriteEntry("    Unreferenced Object: " + e.UnreferencedObject);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Prefer instance method, works better for unit testing")]
        private TypeVisualiserLayoutFile Deserialise(Stream xmlSource)
        {
            try
            {
                // Assume latest version of diagram file.
                return Deserialise105(xmlSource);
            } catch (InvalidOperationException ex)
            {
                // Cannot deserialise this tvd file. Most likely old version.
                Logger.Instance.WriteEntry("Error occured in ModelSerialiser.Deserialise, trying previous known diagram version deserialisers...");
                Logger.Instance.WriteEntry(ex);
                return DeserialiseBasedOnVersion(xmlSource);
            }
        }
    }
}