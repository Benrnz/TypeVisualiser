namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class TypeToBrushConverterTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackTest()
        {
            new TypeToBrushConverter().ConvertBack(null, null, null, null);
            Assert.Fail();
        }

        [TestMethod]
        public void GivenClass()
        {
            string actual = TypeToBrushConverter.ConvertToBrushName(new VisualisableType(typeof(Car)));
            Assert.AreEqual(TypeToBrushConverter.ClassBrushKey, actual);
        }

        [TestMethod]
        public void GivenEnum()
        {
            string actual = TypeToBrushConverter.ConvertToBrushName(new VisualisableType(typeof(Visibility)));
            Assert.AreEqual(TypeToBrushConverter.EnumBrushKey, actual);
        }

        [TestMethod]
        public void GivenInterface()
        {
            string actual = TypeToBrushConverter.ConvertToBrushName(new VisualisableType(typeof(IAsyncResult)));
            Assert.AreEqual(TypeToBrushConverter.InterfaceBrushKey, actual);
        }

        [TestMethod]
        public void GivenNullShouldBeNull()
        {
            string actual = TypeToBrushConverter.ConvertToBrushName(null);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GivenSystemClass()
        {
            string actual = TypeToBrushConverter.ConvertToBrushName(new VisualisableType(typeof(string)));
            Assert.AreEqual(TypeToBrushConverter.SystemClassBrushKey, actual);
        }
    }
}