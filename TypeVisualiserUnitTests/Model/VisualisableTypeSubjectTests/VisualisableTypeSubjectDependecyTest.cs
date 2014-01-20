namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using StructureMap;

    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;
    using TypeVisualiser.Startup;

    [TestClass]
    public class VisualisableTypeSubjectDependencyTest
    {
        private static IVisualisableTypeWithAssociations subject;

        private static ITrivialFilter mockFilter;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            IoC.MapHardcodedRegistrations();

            mockFilter = MockRepository.GenerateMock<ITrivialFilter>();
            var modelContainer = new Container();

            mockFilter.Expect(m => m.HideTrivialTypes).Return(true).Repeat.Any();
            mockFilter.Expect(m => m.IsTrivialType(typeof(String).FullName)).Return(true).Repeat.Any();
            mockFilter.Expect(m => m.IsTrivialType(typeof(Delegate).FullName)).Return(true).Repeat.Any();

            subject = VisualisableTypeTestData.FullModel<Car>(modelContainer, mockFilter);
        }

        [TestMethod]
        public void DependenciesCountConsistencyWithAssoications()
        {
            // +1 for the parent type
            Assert.AreEqual(subject.AllAssociations.Count() + 1 + subject.ThisTypeImplements.Count(), subject.TrivialDependencies + subject.NontrivialDependencies);
        }

        [TestMethod]
        public void NonTrivialDependenciesCount()
        {
            /*
            {StaticAssociation: Colors}	
            {ConsumeAssociation: CombustionEngine}
            {FieldAssociation: EventHandler}
            {StaticAssociation: Interlocked}
            {ConsumeAssociation: NotImplementedException}	
            {ConsumeAssociation: PropertyChangedEventArgs}	
            {FieldAssociation: PropertyChangedEventHandler}	
            {FieldAssociation: SqlCommand}	
            {ConsumeAssociation: SqlConnection}	
            {StaticAssociation: StaticTestClass}	
            {Vehicle} - PARENT
             */
            subject.NontrivialDependencies.Should().Be(11);
        }

        [TestMethod]
        public void ParentIsNullTest()
        {
            PrivateAccessor.SetProperty(subject, "Parent", null);
            subject.TrivialDependencies.Should().Be(11);
            subject.NontrivialDependencies.Should().Be(10);
        }

        [TestMethod]
        public void TrivialDependenciesCount()
        {
            Assert.AreEqual(11, subject.TrivialDependencies);
        }
    }
}