namespace TypeVisualiserUnitTests.Model
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using StructureMap;
    using TypeVisualiser.Model;

    [TestClass]
    public class ModelBuilderTest
    {
        private MockRepository mockery;
        private ModelBuilder target;
        private IContainer factory;
        private IVisualisableTypeWithAssociations mockSubject;

        [TestInitialize]
        public void TestInitialise()
        {
            this.mockery = new MockRepository();
            this.mockSubject = this.mockery.Stub<IVisualisableTypeWithAssociations>();
            this.factory = new Container(config => config.For<IVisualisableTypeWithAssociations>().Use(mockSubject));

            this.target = new ModelBuilder();
            PrivateAccessor.SetField(this.target, "doNotUseFactory", this.factory);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockery = null;
            this.mockSubject = null;
            this.factory.Dispose();
            this.factory = null;
        }

        [TestMethod]
        public void CanConstruct()
        {
        }

        [TestMethod]
        public void BuildSubjectShouldSucceedGivenStringType()
        {
            var result = this.target.BuildSubject(typeof(string), 0);
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildSubjectShouldThrowGivenNull()
        {
            this.target.Invoking(x => x.BuildSubject((Type)null, 0)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildTypeShouldReturnTypeGivenTypeNameAlreadyInAppDomain()
        {
            var result = this.target.BuildType(null, typeof(string).AssemblyQualifiedName);
            result.Name.Should().BeEquivalentTo("String");
        }
    }
}
