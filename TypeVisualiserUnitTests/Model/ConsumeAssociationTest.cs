namespace TypeVisualiserUnitTests.Model
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StructureMap;

    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;

    [TestClass]
    public class ConsumeAssociationTest
    {
        private IContainer factory;

        [TestMethod]
        public void ConstructorShouldHandleUnresolvedGenericTypes()
        {
            ConsumeAssociation target = AssociationTestData.ConsumeAssociationFullModel(this.factory, typeof(Transmission<>).GetProperty("Engine").PropertyType, new[] { " hello " }, 1);

            target.AssociatedTo.AssemblyQualifiedName.Should().NotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void ConstructorShouldThrowWhenGivenNull()
        {
            AssociationTestData.ConsumeAssociationFullModel(this.factory, null, new List<string>(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowWhenGivenNullDescriptors()
        {
            AssociationTestData.ConsumeAssociationFullModel(this.factory, typeof(string), null, 0);
        }

        [TestMethod]
        public void StyleLineShouldSetThickness()
        {
            ConsumeAssociation target = AssociationTestData.ConsumeAssociationFullModel(this.factory, typeof(string), new List<string>(), 5);
            var line = new ConnectionLine();
            target.StyleLine(line);
            Assert.IsTrue(line.Thickness > 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void StyleLineShouldThrowWhenGivenNull()
        {
            ConsumeAssociation target = AssociationTestData.ConsumeAssociationFullModel(this.factory, typeof(string), new List<string>(), 0);
            target.StyleLine(null);
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.factory = new Container();
        }

    }
}