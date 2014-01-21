using System;
using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using StructureMap;
using TypeVisualiser;
using TypeVisualiser.Geometry;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;

namespace TypeVisualiserUnitTests.Model
{
    [TestClass]
    public class ParentAssociationTest
    {
        private IApplicationResources mockResources;
        private IVisualisableType mockType;

        [TestMethod]
        public void ConstructorShouldSetAssociatedTo()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            target.AssociatedTo.Should().NotBeNull();
        }

        [TestMethod]
        public void ConstructorShouldSetName()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            target.Name.Length.Should().BeGreaterThan(10);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullResourceException))]
        public void ConstructorShouldThrowWhenGivenNull1()
        {
            new ParentAssociation(MockRepository.GenerateStub<IModelBuilder>(), null, null);
        }

        [TestMethod]
        public void CreateLineHeadMustNotBeNull()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            ArrowHead result = target.CreateLineHead();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void ProposePositionForAssociateShouldProposeAPositionNotOverlappingWithCentre()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            var subjectArea = new Area(new Point(23, 45), new Point(67, 94));
            Func<Area, ProximityTestResult> overlapsWithOthers = area => new ProximityTestResult(Proximity.NotOverlapping);

            Area result = target.ProposePosition(123.44, 553.11, new Area(new Point(23, 45), new Point(67, 94)), overlapsWithOthers);

            Assert.IsTrue(result.OverlapsWith(subjectArea).Proximity == Proximity.NotOverlapping);
        }

        [TestMethod]
        public void ProposePositionForAssociateShouldThrowGivenNullArea()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            target.Invoking(x => x.ProposePosition(123.44, 553.11, null, null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void StyleLineShouldSetThickness()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            PrivateAccessor.SetField<Association>(target, "doNotUseResources", this.mockResources);
            var line = new ConnectionLine();
            target.StyleLine(line);
            line.Thickness.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void StyleLineShouldThrowWhenGivenNull()
        {
            ParentAssociation target = CreateTarget(this.mockType);
            target.Invoking(x => x.StyleLine(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            this.mockResources = null;
            this.mockType = null;
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.mockType = MockRepository.GenerateStub<IVisualisableType>();
            this.mockResources = MockRepository.GenerateStub<IApplicationResources>();
            this.mockType.Expect(m => m.Name).Return("1234567890");
            this.mockType.Expect(m => m.Modifiers).Return(new ModifiersData(typeof (string)));
        }

        private ParentAssociation CreateTarget(IVisualisableType type)
        {
            return PrivateAccessor.PrivateConstructor<ParentAssociation>(type);
        }
    }
}