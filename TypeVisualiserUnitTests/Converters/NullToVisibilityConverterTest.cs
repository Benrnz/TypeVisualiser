namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class NullToVisibilityConverterTest
    {
        public TestContext TestContext { get; set; }

        private NullToVisibilityConverter CreateTarget()
        {
            return new NullToVisibilityConverter();
        }

        [TestMethod]
        public void ConvertBackShouldThrowNotSupported()
        {
            var target = CreateTarget();
            target.Invoking(x => x.ConvertBack(null, null, null, null)).ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ConvertShouldReturnCollapsedGivenNull()
        {
            var target = CreateTarget();
            var result = target.Convert(null, null, null, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ConvertShouldReturnVisibleGivenSomeObject()
        {
            var target = CreateTarget();
            var result = target.Convert(new object(), typeof(object), null, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void ConvertShouldReturnVisibleGivenSomeObject2()
        {
            var target = CreateTarget();
            var result = target.Convert("foo", typeof(string), null, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Visible);
        }
    }
}