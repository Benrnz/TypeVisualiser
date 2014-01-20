using TypeVisualiser.Startup;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;

    using StructureMap;

    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeSubjectConsumesTest
    {
        private const string DemoCombustionEngineName = "TypeVisualiser.DemoTypes.CombustionEngine";
        private const string DemoStaticTypeName = "TypeVisualiser.DemoTypes.StaticTestClass";

        private static IVisualisableTypeWithAssociations subjectCar;
        private static IVisualisableTypeWithAssociations subjectFleet;
        private static ITrivialFilter mockTrivialFilter;

        private static ITrivialFilter mockEmptyTrivialFilter;

        private static IContainer modelContainer;


        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            IoC.MapHardcodedRegistrations();

            mockTrivialFilter = MockRepository.GenerateMock<ITrivialFilter>();
            mockTrivialFilter.Expect(m => m.HideTrivialTypes).Return(true).Repeat.Any();
            mockTrivialFilter.Expect(m => m.IsTrivialType(typeof(Delegate).FullName)).Return(true).Repeat.Any();
            mockTrivialFilter.Expect(m => m.IsTrivialType(typeof(bool).FullName)).Return(true).Repeat.Any();
            mockTrivialFilter.Expect(m => m.IsTrivialType(typeof(Interlocked).FullName)).Return(true).Repeat.Any();

            mockEmptyTrivialFilter = MockRepository.GenerateStub<ITrivialFilter>();
            
            // By default filter is off
            modelContainer = new Container(c => c.For<ITrivialFilter>().Use(mockEmptyTrivialFilter));

            subjectCar = VisualisableTypeTestData.FullModel<Car>(modelContainer);
            subjectFleet = VisualisableTypeTestData.FullModel<Fleet>(modelContainer);
        }

        [TestMethod]
        public void ConsumeAssociationsConsistencyWithAssociationCollection()
        {
            SetupTrivialFilter(false);
            IEnumerable<ConsumeAssociation> result = subjectCar.AllAssociations.OfType<ConsumeAssociation>();
            subjectCar.Consumes.Count().Should().Be(result.Count());
        }

        [TestMethod]
        public void ConsumesListShouldNotContainSelfReferences()
        {
            subjectFleet.Consumes.Should().NotContain(x => x.AssociatedTo.Id == subjectFleet.Id);
        }

        [TestMethod]
        public void HasCombustionEngine()
        {
            subjectCar.Consumes.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith(DemoCombustionEngineName)).Should().BeTrue();
        }

        [TestMethod]
        public void HasCorrectCountOfConsumeAssociationsFilterOff()
        {
            /*
StaticAssociation: Colors
ConsumeAssociation: CombustionEngine
StaticAssociation: Delegate
StaticAssociation: Interlocked
ConsumeAssociation: NotImplementedException
ConsumeAssociation: PropertyChangedEventArgs
ConsumeAssociation: SqlConnection
StaticAssociation: StaticTestClass
ConsumeAssociation: String
            */
            // Not boolean becuase it gets promoted to FieldAssociation
            SetupTrivialFilter(false);
            List<ConsumeAssociation> result = subjectCar.Consumes.ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));
            result.Count().Should().Be(9);
        }

        [TestMethod]
        public void HasCorrectCountOfConsumeAssociationsFilterOn()
        {
            /*
StaticAssociation: Colors
ConsumeAssociation: CombustionEngine
ConsumeAssociation: NotImplementedException
ConsumeAssociation: PropertyChangedEventArgs
ConsumeAssociation: SqlConnection
StaticAssociation: StaticTestClass
ConsumeAssociation: String
             */
            SetupTrivialFilter(true);
            List<ConsumeAssociation> result = subjectCar.Consumes.ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));
            result.Count().Should().Be(7);
        }

        [TestMethod]
        public void HasCorrectNumberOfStaticConsumeAssociationsFilterOff()
        {
            SetupTrivialFilter(false);
            List<StaticAssociation> result = subjectCar.Consumes.OfType<StaticAssociation>().ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));
            result.Count().Should().Be(4);
        }

        [TestMethod]
        public void HasCorrectNumberOfStaticConsumeAssociationsFilterOn()
        {
            SetupTrivialFilter(true);
            List<StaticAssociation> result = subjectCar.Consumes.OfType<StaticAssociation>().ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));
            result.Count().Should().Be(2);
        }

        [TestMethod]
        public void HasSqlConnection()
        {
            subjectCar.Consumes.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Data.SqlClient.SqlConnection")).Should().BeTrue();
        }

        [TestMethod]
        public void HasStaticClass()
        {
            subjectCar.Consumes.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith(DemoStaticTypeName)).Should().BeTrue();
        }

        [TestMethod]
        public void ShouldDetectConsumptionFromOneLineStaticMethodReturningNewClass()
        {
            var target = subjectFleet;
            ConsumeAssociation result = target.Consumes.Single(x => x.AssociatedTo.AssemblyQualifiedName == typeof(DivideByZeroException).AssemblyQualifiedName);
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void ShouldNotHaveCommand()
        {
            Assert.IsFalse(subjectCar.Consumes.Any(x => x.AssociatedTo.AssemblyQualifiedName == "System.Data.SqlClient.SqlCommand"));
        }

        private void SetupTrivialFilter(bool filterActive)
        {
            if (filterActive)
            {
                modelContainer.Configure(c => c.For<ITrivialFilter>().Use(mockTrivialFilter));
            } else
            {
                modelContainer.Configure(c => c.For<ITrivialFilter>().Use(mockEmptyTrivialFilter));
            }
        }
    }
}