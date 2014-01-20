namespace TypeVisualiserUnitTests.Model.VisualisableTypeTests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeGenericsTest
    {
        private static VisualisableType target1;
        private static VisualisableType target2;

        [ClassInitialize]
        public static void ClassIntialise(TestContext context)
        {
            target1 = new VisualisableType(typeof(TiptronicTransmission<>));
            target2 = new VisualisableType(typeof(Transmission<>));
        }

        [TestMethod]
        public void LoadingUnresolvedGenericTypeWithGenericParentShouldSetFullName()
        {
            target1.AssemblyQualifiedName.Should().NotBeNull();
        }

        [TestMethod]
        public void LoadingUnresolvedGenericTypeShouldSetFullName()
        {
            target2.AssemblyQualifiedName.Should().NotBeNull();
        }
    }
}
