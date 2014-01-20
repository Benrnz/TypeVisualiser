namespace TypeVisualiserUnitTests.Converters
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.UI.Converters;

    [TestClass]
    public class MemberKindToImageConverterTest
    {
        public TestContext TestContext { get; set; }

        private MemberKindToImageConverter CreateTarget()
        {
            return new MemberKindToImageConverter();
        }

        [TestMethod]
        public void ConvertBackShouldThrowNotSupported()
        {
            var target = CreateTarget();
            target.Invoking(x => x.ConvertBack(null, null, null, null)).ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ConvertShouldReturnNullGivenNull()
        {
            var target = CreateTarget();
            var result = target.Convert(null, null, null, CultureInfo.CurrentCulture);

            result.Should().BeNull();
        }

        [TestMethod]
        public void ConvertShouldReturnPathToPropertyImageGivenPropertyMemberKind()
        {
            var target = CreateTarget();
            var result = (string)target.Convert(MemberKind.Property, typeof(MemberKind), null, CultureInfo.CurrentCulture);

            result.Should().Contain("Assets/Property.png");
        }

        [TestMethod]
        public void ConvertShouldReturnPathToFieldImageGivenPropertyMemberKind()
        {
            var target = CreateTarget();
            var result = (string)target.Convert(MemberKind.Field, typeof(MemberKind), null, CultureInfo.CurrentCulture);

            result.Should().Contain("Assets/Field.png");
        }

        [TestMethod]
        public void ConvertShouldReturnPathToMethodImageGivenPropertyMemberKind()
        {
            var target = CreateTarget();
            var result = (string)target.Convert(MemberKind.Method, typeof(MemberKind), null, CultureInfo.CurrentCulture);

            result.Should().Contain("Assets/Method.png");
        }
    }
}