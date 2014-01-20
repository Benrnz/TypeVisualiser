namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class CountToVisibilityConverterTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackTest()
        {
            new CountToVisibilityConverter().ConvertBack(null, null, null, CultureInfo.CurrentCulture);
            Assert.Fail();
        }

        [TestMethod]
        public void GivenNullShouldBeCollapsed()
        {
            var actual = new CountToVisibilityConverter().Convert(null, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        [TestMethod]
        public void Given0ShouldBeCollapsed()
        {
            var actual = new CountToVisibilityConverter().Convert(0, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        [TestMethod]
        public void Given1ShouldBeCollapsed()
        {
            var actual = new CountToVisibilityConverter().Convert(1, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Visible, actual);
        }
    }
}