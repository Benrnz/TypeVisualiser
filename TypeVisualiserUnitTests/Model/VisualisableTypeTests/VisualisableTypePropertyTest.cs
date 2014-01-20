using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser.DemoTypes;
using TypeVisualiser.ILAnalyser;
using TypeVisualiser.Model;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeTests
{
    [TestClass]
    public class VisualisableTypePropertyTest
    {
        private static VisualisableType visualisableType;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            visualisableType = new VisualisableType(typeof (Car));
        }

        [TestMethod]
        public void AssemblyFullNameShouldBeGreaterThan20()
        {
            visualisableType.AssemblyFullName.Length.Should().BeGreaterThan(20);
        }

        [TestMethod]
        public void AssemblyNameShouldBeGreaterThan10()
        {
            visualisableType.AssemblyName.Length.Should().BeGreaterThan(10);
        }

        [TestMethod]
        public void ConstructorCountShouldBe1()
        {
            visualisableType.ConstructorCount.Should().Be(1);
        }

        [TestMethod]
        public void EnumMemberCountShouldBe0()
        {
            visualisableType.EnumMemberCount.Should().Be(0);
        }

        [TestMethod]
        public void EventCountShouldBe2()
        {
            visualisableType.EventCount.Should().Be(2);
        }

        [TestMethod]
        public void FieldCountShouldBe8()
        {
            visualisableType.FieldCount.Should().Be(8);
        }

        [TestMethod]
        public void FullNameShouldBeGreaterThan20()
        {
            visualisableType.AssemblyQualifiedName.Length.Should().BeGreaterThan(20);
        }

        [TestMethod]
        public void IdShouldNotBeEmpty()
        {
            visualisableType.Id.Should().NotBeEmpty();
        }

        [TestMethod]
        public void IsSubjectShouldNotBeFalse()
        {
            visualisableType.IsSubject.Should().BeFalse();
        }

        [TestMethod]
        public void LinesOfCodeShouldBeAsExpected()
        {
            var task = (Task) PrivateAccessor.GetProperty(visualisableType, "LinesOfCodeTask");
            task.Wait();
            visualisableType.LinesOfCode.Should().Be(255);
        }

        [TestMethod]
        public void LinesOfCodeToolTipShouldBe3()
        {
            var task = (Task)PrivateAccessor.GetProperty(visualisableType, "LinesOfCodeTask");
            task.Wait();
            visualisableType.LinesOfCodeToolTip.Length.Should().Be(3);
        }

        [TestMethod]
        public void MethodCountShouldBe4()
        {
            visualisableType.MethodCount.Should().Be(4);
        }

        [TestMethod]
        public void ModifiersShouldNotBeNull()
        {
            visualisableType.Modifiers.Should().NotBeNull();
        }

        [TestMethod]
        public void NameShouldBeOfLength3()
        {
            visualisableType.Name.Length.Should().Be(3);
        }

        [TestMethod]
        public void NamespaceShouldBeGreaterThan10()
        {
            visualisableType.ThisTypeNamespace.Length.Should().BeGreaterThan(10);
        }

        [TestMethod]
        public void PropertyShouldBe5()
        {
            visualisableType.PropertyCount.Should().Be(5);
        }

        [TestMethod]
        public void SubjectOrAssociateShouldBeAssociate()
        {
            visualisableType.SubjectOrAssociate.Should().Be(SubjectOrAssociate.Associate);
        }

        [TestInitialize]
        public void TestInitialise()
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
        }

        [TestMethod]
        public void TooltipShouldBeGreaterThan10()
        {
            visualisableType.TypeToolTip.Length.Should().BeGreaterThan(10);
        }
    }
}