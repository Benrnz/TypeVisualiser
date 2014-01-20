namespace TypeVisualiserUnitTests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;

    using StructureMap;

    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Geometry;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;
    using TypeVisualiser.Model.Persistence;

    [TestClass]
    public class FieldAssociationTest
    {
        private IContainer factory;
        private IDiagramDimensions mockDimensions;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
        }

        [TestMethod]
        public void ConstructorShouldHandleUnresolvedGenericTypes()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(Transmission<>).GetProperty("Engine").PropertyType, new[] { " hello " }, 1);

            target.AssociatedTo.AssemblyQualifiedName.Should().NotBeNull();
        }

        [TestMethod]
        public void ConstructorShouldSetAssociatedTo()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 0);
            Assert.IsNotNull(target.AssociatedTo);
        }

        [TestMethod]
        public void ConstructorShouldSetName()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            Assert.IsTrue(target.Name.Length > "name".Length);
        }

        [TestMethod]
        public void ConstructorShouldSetNumberOfUsageCount()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            Assert.AreEqual(3, target.UsageCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowWhenGivenNull1()
        {
            AssociationTestData.FieldAssociationFullModel(this.factory, null, null, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowWhenGivenNull2()
        {
            AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), null, 0);
        }

        [TestMethod]
        public void CreateLineHeadMustNotBeNull()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            var result = target.CreateLineHead(); 
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FieldAssociationShouldBeEqual()
        {
            FieldAssociation target1 = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            FieldAssociation target2 = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "Moose" }, 7);
            Assert.IsTrue(target1.Equals(target2));
        }

        [TestMethod]
        public void FieldAssociationShouldNotBeEqualToNull()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            Assert.IsFalse(target.Equals(null));
        }

        [TestMethod]
        public void PersistenceTypeShouldNotBeNull()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            Type result = target.PersistenceType;
            Assert.IsNotNull(target.PersistenceType);
            Assert.IsTrue(typeof(FieldAssociationData).IsAssignableFrom(result));
        }

        [TestMethod]
        public void ProposePositionForAssociateShouldProposeAPositionNotOverlappingWithCentre()
        {
            this.mockDimensions.Expect(m => m.CalculateNextAvailableAngle()).Return(33.33);

            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3, this.mockDimensions);
            var subjectArea = new Area(new Point(23, 45), new Point(67, 94));
            Func<Area, ProximityTestResult> overlapsWithOthers = area => new ProximityTestResult(Proximity.NotOverlapping);


            Area result = target.ProposePosition(
                123.44,
                553.11,
                new Area(new Point(23, 45), new Point(67, 94)),
                overlapsWithOthers);

            Assert.IsTrue(result.OverlapsWith(subjectArea).Proximity == Proximity.NotOverlapping);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void ProposePositionForAssociateShouldThrowGivenNullArea()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new[] { "name" }, 3);
            target.ProposePosition(
                123.44,
                553.11,
                null,
                null);
        }

        [TestMethod]
        public void StyleLineShouldSetThickness()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new List<string> { "one", "two" }, 5);
            var line = new ConnectionLine();
            target.StyleLine(line);

            Assert.IsTrue(line.Thickness > 1);
        }

        [TestMethod]
        public void StyleLineShouldSetThicknessTo16WhenUsageIsHigh()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new List<string> { "one", "two" }, 20);
            var line = new ConnectionLine();
            target.StyleLine(line);

            Assert.AreEqual(16, line.Thickness);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void StyleLineShouldThrowWhenGivenNull()
        {
            FieldAssociation target = AssociationTestData.FieldAssociationFullModel(this.factory, typeof(string), new List<string>(), 1);
            target.StyleLine(null);
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.mockDimensions = MockRepository.GenerateMock<IDiagramDimensions>();
            this.factory = new Container();
        }
    }
}