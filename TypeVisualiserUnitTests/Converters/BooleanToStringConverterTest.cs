namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class BooleanToStringConverterTest
    {
        [TestMethod]
        public void ConvertBackNull()
        {
            var actual = new BooleanToStringConverter().ConvertBack(null, null, null, CultureInfo.CurrentCulture);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void ConvertBackTrue()
        {
            var actual = new BooleanToStringConverter().ConvertBack("true", null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertBackRubbish()
        {
            new BooleanToStringConverter().ConvertBack("kljbgjklab", null, null, CultureInfo.CurrentCulture);
            Assert.Fail();
        }

        [TestMethod]
        public void ConvertNull()
        {
            var actual = new BooleanToStringConverter().Convert(null, null, null, CultureInfo.CurrentCulture);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void ConvertTrue()
        {
            var actual = new BooleanToStringConverter().Convert(true, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual("True", actual);
        }
    }
}