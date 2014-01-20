namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class NotBooleanToVisibilityConverterTest
    {
        [TestMethod]
        public void GivenNullShouldBeVisible()
        {
            object actual = new NotBooleanToVisibilityConverter().Convert(null, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Visible, actual);
        }

        [TestMethod]
        public void GivenTrueShouldBeCollapsed()
        {
            object actual = new NotBooleanToVisibilityConverter().Convert(true, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        [TestMethod]
        public void GivenFalseShouldBeVisible()
        {
            object actual = new NotBooleanToVisibilityConverter().Convert(false, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Visible, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackTest()
        {
            new NotBooleanToVisibilityConverter().ConvertBack(null, null, null, CultureInfo.CurrentCulture);
            Assert.Fail();
        }
    }
}