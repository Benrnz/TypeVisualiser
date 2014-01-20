namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class BooleanToAbstractConverterTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackTest()
        {
            var target = new BooleanToAbstractConverter();
            target.ConvertBack(null, null, null, null);
            Assert.Fail();
        }

        [TestMethod]
        public void GivenNullShouldBeNull()
        {
            var target = new BooleanToAbstractConverter();
            CultureInfo culture = CultureInfo.CurrentCulture;
            object actual = target.Convert(null, null, null, culture);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void MustHaveAtLeast2Arguments()
        {
            var target = new BooleanToAbstractConverter();
            CultureInfo culture = CultureInfo.CurrentCulture;
            var arguments = new[] { "1" };
            target.Convert(arguments, null, null, culture);
            Assert.Fail();
        }

        [TestMethod]
        public void GivenIsAbstractAndIsStatic()
        {
            var target = new BooleanToAbstractConverter();
            var actual = target.Convert(new object[] { true, true }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }

        [TestMethod]
        public void GivenIsAbstractAndNotIsStatic()
        {
            var target = new BooleanToAbstractConverter();
            var actual = target.Convert(new object[] { true, false }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(0.7, actual);
        }

        [TestMethod]
        public void GivenNotIsAbstractAndNotIsStatic()
        {
            var target = new BooleanToAbstractConverter();
            var actual = target.Convert(new object[] { false, false }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }

        [TestMethod]
        public void GivenNotIsAbstractAndIsStatic()
        {
            var target = new BooleanToAbstractConverter();
            var actual = target.Convert(new object[] { false, true }, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(1.0, actual);
        }
    }
}