using FluentAssertions;
using TypeVisualiser.Startup;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeSubjectDependencyTest
    {
        private static VisualisableTypeWithAssociations subject;
        private MockRepository mockery = new MockRepository();
        private ITrivialFilter mockFilter;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            IoC.MapHardcodedRegistrations();
            subject = new VisualisableTypeWithAssociations(typeof(Car));
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.mockFilter = this.mockery.DynamicMock<ITrivialFilter>();
            PrivateAccessor.SetStaticField(typeof(TrivialFilter), "current", this.mockFilter);
            using (this.mockery.Record())
            {
                this.mockFilter.Expect(m => m.HideTrivialTypes).Return(true).Repeat.Any();
                this.mockFilter.Expect(m => m.IsTrivialType(typeof(System.String).FullName)).Return(true).Repeat.Any();
                this.mockFilter.Expect(m => m.IsTrivialType(typeof(System.Delegate).FullName)).Return(true).Repeat.Any();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.mockery = new MockRepository();
        }

        [TestMethod]
        public void NonTrivialDependenciesCount()
        {
            using (this.mockery.Playback())
            {
                subject.NontrivialDependencies.Should().Be(11);
            }
        }

        [TestMethod]
        public void TrivialDependenciesCount()
        {
            using (this.mockery.Playback())
            {
                Assert.AreEqual(11, subject.TrivialDependencies);
            }
        }

        [TestMethod]
        public void DependenciesCountConsistencyWithAssoications()
        {
            using (this.mockery.Playback())
            {
                Assert.AreEqual(
                    subject.AllAssociations.Count() + 1 + subject.ThisTypeImplements.Count(),
                    subject.TrivialDependencies + subject.NontrivialDependencies);
            }
        }

        [TestMethod]
        public void ParentIsNullTest()
        {
            PrivateAccessor.SetProperty(subject, "Parent", null);
            using (this.mockery.Playback())
            {
                subject.TrivialDependencies.Should().Be(11);
                subject.NontrivialDependencies.Should().Be(10);
            }
        }
    }
}
