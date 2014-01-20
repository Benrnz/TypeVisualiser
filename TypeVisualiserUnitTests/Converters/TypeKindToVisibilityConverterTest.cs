namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class TypeKindToVisibilityConverterTest
    {
        public TestContext TestContext { get; set; }

        private TypeKindToVisibilityConverter CreateTarget()
        {
            return new TypeKindToVisibilityConverter();
        }

        [TestMethod]
        public void ConvertBackShouldThrowNotSupported()
        {
            var target = CreateTarget();
            target.Invoking(x => x.ConvertBack(null, null, null, null)).ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ConvertShouldReturnVisibleGivenValueAndParamAreEqual()
        {
            var target = CreateTarget();
            var result = target.Convert(TypeKind.Enum, typeof(TypeKind), TypeKind.Enum, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void ConvertShouldReturnCollapsedGivenValueAndParamAreNotEqual()
        {
            var target = CreateTarget();
            var result = target.Convert(TypeKind.Enum, typeof(TypeKind), TypeKind.Class, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ConvertShouldReturnCollapsedGivenNull()
        {
            var target = CreateTarget();
            var result = target.Convert(null, null, null, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Collapsed);
        }

        [TestMethod]
        public void ConvertShouldReturnCollapsedGivenNullParam()
        {
            var target = CreateTarget();
            var result = target.Convert(TypeKind.Interface, typeof(TypeKind), null, CultureInfo.CurrentCulture);

            result.Should().Be(Visibility.Collapsed);
        }
    }
}