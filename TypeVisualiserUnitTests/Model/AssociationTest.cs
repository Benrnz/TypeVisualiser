namespace TypeVisualiserUnitTests.Model
{
    using System;
    using System.Windows;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using TypeVisualiser;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;

    [TestClass]
    public class AssociationTest
    {
        private readonly MockRepository mockery = new MockRepository();
        private static VisualisableType stringData = new VisualisableType(typeof(string));

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAClass()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            filterMock.Expect(m => m.IsTrivialType("System.String")).Return(false);
            Association target = new TestAssociation(stringData);
            Assert.IsFalse(target.IsTrivialAssociation(filterMock));
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAClass2()
        {
            var filterMock = this.mockery.DynamicMock<ITrivialFilter>();
            using (this.mockery.Record())
            {
                filterMock.Expect(m => m.IsTrivialType("System.String")).IgnoreArguments().Return(true);
            }

            Association target = new TestAssociation(stringData);

            target.IsTrivialAssociation(filterMock).Should().BeTrue();
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAnEnum()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            Association target = new TestAssociation(new VisualisableType(typeof(Visibility)));
            Assert.IsTrue(target.IsTrivialAssociation(filterMock));
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAnInterface()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            Association target = new TestAssociation(new VisualisableType(typeof(IDiagramDimensions)));
            Assert.IsTrue(target.IsTrivialAssociation(filterMock));
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAnValueType()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            Association target = new TestAssociation(new VisualisableType(typeof(double)));
            Assert.IsTrue(target.IsTrivialAssociation(filterMock));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void IsTrivialAssociationShouldThrowIfGivenNull()
        {
            Association target = new TestAssociation(null);
            target.IsTrivialAssociation(null);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var target = new TestAssociation(stringData);
            Assert.IsFalse(string.IsNullOrWhiteSpace(target.ToString()));
        }
    }
}