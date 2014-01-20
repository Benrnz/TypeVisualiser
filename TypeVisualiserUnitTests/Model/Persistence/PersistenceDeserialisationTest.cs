using System;

namespace TypeVisualiserUnitTests.Model.Persistence
{
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Model.Persistence;

    [TestClass]
    public class PersistenceDeserialisationTest
    {
        [TestMethod]
        public void LoadDemoType1_5()
        {
            var result = DeserialiseXml(DemoTypeDiagram1_5.XmlText);
            result.Should().NotBeNull();
            result.CanvasLayout.Should().NotBeNull();
            result.CanvasLayout.Types.Should().NotBeNull();
            result.Subject.Should().NotBeNull();
            result.Subject.FullName.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void LoadDemoType1_5_BadData()
        {
            DeserialiseXml(DemoTypeDiagram1_5.BadXmlText);
            Assert.Fail();
        }

        [TestMethod]
        public void LoadDemoType1_3()
        {
            var result = DeserialiseXml(DemoTypeDiagram1_3.XmlText);
            result.Should().NotBeNull();
            result.CanvasLayout.Should().NotBeNull();
            result.CanvasLayout.Types.Should().NotBeNull();
            result.Subject.Should().NotBeNull();
            result.Subject.FullName.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void LoadDemoType1_3_BadData()
        {
            DeserialiseXml(DemoTypeDiagram1_3.BadXmlText);
        }

        [TestMethod]
        public void LoadDemoType1_2()
        {
            var result = DeserialiseXml(DemoTypeDiagram1_2.XmlText);
            result.Should().NotBeNull();
            result.CanvasLayout.Should().NotBeNull();
            result.CanvasLayout.Types.Should().NotBeNull();
            result.Subject.Should().NotBeNull();
            result.Subject.FullName.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void LoadDemoType1_2_BadData()
        {
            DeserialiseXml(DemoTypeDiagram1_2.BadXmlText);
        }

        [TestMethod]
        public void LoadDemoType1_2_bug1()
        {
            var result = DeserialiseXml(DemoTypeDiagram1_2.XmlText2);
            result.Should().NotBeNull();
            result.CanvasLayout.Should().NotBeNull();
            result.CanvasLayout.Types.Should().NotBeNull();
            result.Subject.Should().NotBeNull();
            result.Subject.FullName.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void LoadDemoType1_1()
        {
            var result = DeserialiseXml(DemoTypeDiagram1_1.XmlText);
            result.Should().NotBeNull();
        }

        private static TypeVisualiserLayoutFile DeserialiseXml(string xml)
        {
            var serialiser = new ModelSerialiser_Accessor();
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(xml)))
            {
                return serialiser.Deserialise(stream);
            }
        }
    }
}