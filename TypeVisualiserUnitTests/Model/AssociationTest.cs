namespace TypeVisualiserUnitTests.Model
{
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
            Association target = AssociationTestData.AssociationIsolated(filterMock);
            Assert.IsFalse(target.IsTrivialAssociation());
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAClass2()
        {
            var filterMock = this.mockery.DynamicMock<ITrivialFilter>();
            using (this.mockery.Record())
            {
                filterMock.Expect(m => m.IsTrivialType("System.String")).IgnoreArguments().Return(true);
            }

            Association target = AssociationTestData.AssociationIsolated(filterMock);

            target.IsTrivialAssociation().Should().BeTrue();
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAnEnum()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            Association target = AssociationTestData.AssociationIsolated(typeof(Visibility), filterMock);
            Assert.IsTrue(target.IsTrivialAssociation());
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAnInterface()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            Association target = AssociationTestData.AssociationIsolated(typeof(IDiagramDimensions), filterMock);
            Assert.IsTrue(target.IsTrivialAssociation());
        }

        [TestMethod]
        public void IsTrivialAssociationShouldReturnTrueGivenAnValueType()
        {
            var filterMock = this.mockery.Stub<ITrivialFilter>();
            Association target = AssociationTestData.AssociationIsolated(typeof(double), filterMock);
            Assert.IsTrue(target.IsTrivialAssociation());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void IsTrivialAssociationShouldThrowIfGivenNull()
        {
            Association target = AssociationTestData.AssociationIsolated();
            target.IsTrivialAssociation();
        }

        [TestMethod]
        public void ToStringTest()
        {
            TestAssociation target = AssociationTestData.AssociationIsolated();
            Assert.IsFalse(string.IsNullOrWhiteSpace(target.ToString()));
        }
    }
}