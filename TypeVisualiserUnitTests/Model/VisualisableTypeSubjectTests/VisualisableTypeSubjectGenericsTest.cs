using TypeVisualiser.Startup;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StructureMap;

    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeSubjectGenericsTest
    {
        private static IVisualisableTypeWithAssociations target;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            IoC.MapHardcodedRegistrations();
            target = VisualisableTypeTestData.FullModel(typeof(TiptronicTransmission<>), new Container());
        }

        [TestInitialize]
        public void TestInitialise()
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
        }

        [TestMethod]
        public void LoadingUnresolvedGenericTypeShouldSetFullName()
        {
            target.AssemblyQualifiedName.Should().NotBeNull();
        }

        [TestMethod]
        public void LoadingUnresolvedGenericTypeWithParentGenericShouldSetFullName()
        {
            target.Parent.AssociatedTo.AssemblyQualifiedName.Should().NotBeNull();
        }
    }
}
