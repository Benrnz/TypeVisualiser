namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class BooleanToAbstractShadowConverterTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackTest()
        {
            new BooleanToAbstractShadowConverter().ConvertBack(null, null, null, null);
            Assert.Fail();
        }

        [TestMethod]
        public void GivenNullShouldBeNull()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(null, null, null, CultureInfo.CurrentCulture);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GivenEmptyArrayShouldBeNull()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { }, null, null, CultureInfo.CurrentCulture);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GivenIsAbstractAndIsStatic()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { true, true }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }

        [TestMethod]
        public void GivenNotIsAbstractAndIsStatic()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { false, true }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }

        [TestMethod]
        public void GivenNotIsAbstractAndNotIsStatic()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { false, false }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }

        [TestMethod]
        public void GivenIsAbstractAndNotIsStatic()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { true, false }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0D, actual);
        }

        [TestMethod]
        public void GivenIsAbstractOnly()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { true }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }

        [TestMethod]
        public void GivenNotIsAbstractOnly()
        {
            object actual = new BooleanToAbstractShadowConverter().Convert(new object[] { false }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0D, actual);
        }
    }
}