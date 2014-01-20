namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Model;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class SubjectOnlyVisibilityConverterTest
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertBackTest()
        {
            new SubjectOnlyVisibilityConverter().ConvertBack(null, null, null, null);
            Assert.Fail();
        }

        [TestMethod]
        public void GivenNullShouldBeCollapsed()
        {
            var actual = new SubjectOnlyVisibilityConverter().Convert(null, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        [TestMethod]
        public void GivenSubjectShouldBeVisible()
        {
            var actual = new SubjectOnlyVisibilityConverter().Convert(SubjectOrAssociate.Subject, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Visible, actual);
        }

        [TestMethod]
        public void GivenAssociateShouldBeCollapsed()
        {
            var actual = new SubjectOnlyVisibilityConverter().Convert(SubjectOrAssociate.Associate, null, null, CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Collapsed, actual);
        }
    }
}