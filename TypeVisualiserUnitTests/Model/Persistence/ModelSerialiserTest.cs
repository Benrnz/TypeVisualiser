using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser.DemoTypes;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;

namespace TypeVisualiserUnitTests.Model.Persistence
{
    [TestClass]
    public class ModelSerialiserTest
    {
        private static VisualisableTypeWithAssociations carData = new VisualisableTypeWithAssociations(typeof(Car), 0);

        [TestMethod]
        public void CanSerialise()
        {
            var dataToSerialise = new TypeVisualiserLayoutFile();
            var dataContents = (VisualisableTypeSubjectData)carData.ExtractPersistentData();
            dataToSerialise.Subject = dataContents;
            var subject = new ModelSerialiser();
            subject.Serialise("TypeVisualiserTest.tvd", dataToSerialise);
        }

        [TestMethod]
        public void SanityCheck4()
        {
            var dataToSerialise = new TypeVisualiserLayoutFile();
            var dataContents = new VisualisableTypeSubjectData
            {
                Associations = new[] { new FieldAssociationData(), new StaticAssociationData(), new ConsumeAssociationData() }
            };
            dataToSerialise.Subject = dataContents;

            var serialiser = new XmlSerializer(typeof(TypeVisualiserLayoutFile));
            using (var writer = new XmlTextWriter("SanityTest.xml", Encoding.UTF8))
            {
                serialiser.Serialize(writer, dataToSerialise);
            }
        }

        [TestMethod]
        public void SanityCheck3()
        {
            var dataContents = (VisualisableTypeSubjectData)carData.ExtractPersistentData();
            var serialiser = new XmlSerializer(typeof(VisualisableTypeSubjectData));
            using (var writer = new XmlTextWriter("SanityTest.xml", Encoding.UTF8))
            {
                serialiser.Serialize(writer, dataContents);
            }
        }

        [TestMethod]
        public void SanityCheck2()
        {
            var diagramData = new List<FieldAssociationData> { new StaticAssociationData { Name = "Test", Show = true, UsageCount = 2 } };
            var serialiser = new XmlSerializer(typeof(List<FieldAssociationData>));
            using (var writer = new XmlTextWriter("SanityTest.xml", Encoding.UTF8))
            {
                serialiser.Serialize(writer, diagramData);
            }
        }

        [TestMethod]
        public void SanityCheck1()
        {
            var diagramData = new List<FieldAssociationData> { new ConsumeAssociationData { Name = "Test", Show = true, UsageCount = 2 } };
            var serialiser = new XmlSerializer(typeof(List<FieldAssociationData>));
            using (var writer = new XmlTextWriter("SanityTest.xml", Encoding.UTF8))
            {
                serialiser.Serialize(writer, diagramData);
            }
        }
    }
}
