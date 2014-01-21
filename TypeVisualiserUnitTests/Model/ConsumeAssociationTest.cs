namespace TypeVisualiserUnitTests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Shapes;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;
    using TypeVisualiser.Model.Persistence;

    [TestClass]
    public class ConsumeAssociationTest
    {
        private IApplicationResources mockResources;
        private MockRepository mockery;

        [TestMethod]
        public void ConstructorShouldHandleUnresolvedGenericTypes()
        {
            ConsumeAssociation target = CreateTarget(typeof(Transmission<>).GetProperty("Engine").PropertyType, new[] { " hello " }, 1);

            target.AssociatedTo.AssemblyQualifiedName.Should().NotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void ConstructorShouldThrowWhenGivenNull()
        {
            CreateTarget(null, new List<string>(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowWhenGivenNullDescriptors()
        {
            CreateTarget(typeof(string), null, 0);
        }

        [TestMethod]
        public void StyleLineShouldSetThickness()
        {
            ConsumeAssociation target = CreateTarget(typeof(string), new List<string>(), 5);
            PrivateAccessor.SetField<Association>(target, "doNotUseResources", this.mockResources);
            var line = new ConnectionLine();
            target.StyleLine(line);
            Assert.IsTrue(line.Thickness > 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void StyleLineShouldThrowWhenGivenNull()
        {
            ConsumeAssociation target = CreateTarget(typeof(string), new List<string>(), 0);
            target.StyleLine(null);
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            this.mockery = new MockRepository();
            this.mockResources = null;
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.mockery = new MockRepository();
            this.mockResources = this.mockery.Stub<IApplicationResources>();
        }

        private ConsumeAssociation CreateTarget(Type type, IEnumerable<string> usageDescriptors, int usageCount)
        {
            return new ConsumeAssociation(type, usageCount, usageDescriptors.Select(x => new AssociationUsageDescriptor { Description = x, Kind = MemberKind.Method }), 0);
        }
    }
}