using TypeVisualiser.Startup;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeSubjectGenericsTest
    {
        private static VisualisableTypeWithAssociations target;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            IoC.MapHardcodedRegistrations();
            target = new VisualisableTypeWithAssociations(typeof(TiptronicTransmission<>));
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
