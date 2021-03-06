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
    using TypeVisualiser.Startup;

    [TestClass]
    public class VisualisableTypeSubjectFieldsTest
    {
        private static IVisualisableTypeWithAssociations subjectCar;

        private static IVisualisableTypeWithAssociations subjectFleet;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            IoC.MapHardcodedRegistrations();
            var modelContainer = new Container();
            subjectCar = VisualisableTypeTestData.FullModel<Car>(modelContainer);
            subjectFleet = VisualisableTypeTestData.FullModel<Fleet>(modelContainer);
            GlobalIntermediateLanguageConstants.LoadOpCodes();
        }

        [TestMethod]
        public void HasBool()
        {
            foreach (FieldAssociation field in subjectCar.Fields)
            {
                Debug.WriteLine(field.AssociatedTo.AssemblyQualifiedName);
            }

            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Boolean")).Should().BeTrue();
        }

        [TestMethod]
        public void HasColor()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Windows.Media.Color")).Should().BeTrue();
        }

        [TestMethod]
        public void HasDouble()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Double")).Should().BeTrue();
        }

        [TestMethod]
        public void HasFieldCommand()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Data.SqlClient.SqlCommand")).Should().BeTrue();
        }

        [TestMethod]
        public void HasFieldEventHandler()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.EventHandler")).Should().BeTrue();
        }

        [TestMethod]
        public void HasIDbConnection()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Data.IDbConnection")).Should().BeTrue();
        }

        [TestMethod]
        public void HasInt()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Int32")).Should().BeTrue();
        }

        [TestMethod]
        public void ShouldHaveCorrectCount()
        {
            List<FieldAssociation> result = subjectCar.Fields.ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));

            result.Count().Should().Be(9);
        }

        [TestMethod]
        public void ShouldHaveCorrectCountForFilteredAssociationsFilterOff()
        {
            //FieldAssociation: Boolean
            //FieldAssociation: Color
            //StaticAssociation: Colors
            //ConsumeAssociation: CombustionEngine
            //StaticAssociation: Delegate
            //FieldAssociation: Double
            //FieldAssociation: EventHandler
            //FieldAssociation: IDbConnection
            //FieldAssociation: Int32
            //StaticAssociation: Interlocked
            //FieldAssociation: KeyValuePair<String,IWearAndTear>
            //ConsumeAssociation: NotImplementedException
            //ConsumeAssociation: PropertyChangedEventArgs
            //FieldAssociation: PropertyChangedEventHandler
            //FieldAssociation: SqlCommand
            //ConsumeAssociation: SqlConnection
            //StaticAssociation: StaticTestClass
            //ConsumeAssociation: String

            this.SetupTrivialFilter(false);
            List<FieldAssociation> result = subjectCar.FilteredAssociations.ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));

            result.Count().Should().Be(18);
        }

        [TestMethod]
        public void ShouldHaveCorrectCountForFilteredAssociationsFilterOn()
        {
            //FieldAssociation: Color
            //StaticAssociation: Colors
            //ConsumeAssociation: CombustionEngine
            //FieldAssociation: Double
            //FieldAssociation: EventHandler
            //FieldAssociation: IDbConnection
            //FieldAssociation: Int32
            //FieldAssociation: KeyValuePair<String,IWearAndTear>
            //ConsumeAssociation: NotImplementedException
            //ConsumeAssociation: PropertyChangedEventArgs
            //FieldAssociation: PropertyChangedEventHandler
            //FieldAssociation: SqlCommand
            //ConsumeAssociation: SqlConnection
            //StaticAssociation: StaticTestClass
            //ConsumeAssociation: String

            this.SetupTrivialFilter(true);
            List<FieldAssociation> result = subjectCar.FilteredAssociations.ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));

            result.Count().Should().Be(15);
        }

        [TestMethod]
        public void ShouldNotCountSelfFields()
        {
            this.SetupTrivialFilter(false);
            subjectFleet.Fields.Should().NotContain(x => x.AssociatedTo.Id == subjectFleet.Id);
        }

        [TestMethod]
        public void ShouldNotHaveSqlConnection()
        {
            subjectCar.Fields.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.Data.SqlClient.SqlConnection")).Should().BeFalse();
        }

        [TestInitialize]
        public void TestInitialise()
        {
        }

        private ITrivialFilter SetupTrivialFilter(bool filterActive)
        {
            var mockTrivialFilter2 = MockRepository.GenerateMock<ITrivialFilter>();
            mockTrivialFilter2.Expect(m => m.HideTrivialTypes).Return(filterActive).Repeat.Any();
            if (filterActive)
            {
                mockTrivialFilter2.Expect(m => m.IsTrivialType(typeof(Delegate).FullName)).Return(true).Repeat.Any();
                mockTrivialFilter2.Expect(m => m.IsTrivialType(typeof(bool).FullName)).Return(true).Repeat.Any();
                mockTrivialFilter2.Expect(m => m.IsTrivialType(typeof(Interlocked).FullName)).Return(true).Repeat.Any();
            }

            PrivateAccessor.SetField(subjectCar as VisualisableTypeWithAssociations, "getTrivialFilter", new Func<ITrivialFilter>(() => mockTrivialFilter2));
            return mockTrivialFilter2;
        }
    }
}