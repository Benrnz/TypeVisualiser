using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser.DemoTypes;
using TypeVisualiser.ILAnalyser;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.Startup;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    [TestClass]
    public class VisualisableTypeSubjectPersistenceTest
    {
        private static VisualisableTypeSubjectData subject;
        private static VisualisableTypeWithAssociations visualisableType;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            IoC.MapHardcodedRegistrations();
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            visualisableType = new VisualisableTypeWithAssociations(typeof (Car));
            subject = visualisableType.ExtractPersistentData() as VisualisableTypeSubjectData;
        }

        [TestMethod]
        public void ExtractPersistentDataShouldNotCreateParentIfNoParentExists()
        {
            var target = new VisualisableTypeWithAssociations(typeof (string), 0);
            var initTask = PrivateAccessor.GetProperty(target, "LinesOfCodeTask") as Task;
            initTask.Wait();
            // todo fix and support circular field references. this test causes stack overflow currently. 
            var result = (VisualisableTypeSubjectData) target.ExtractPersistentData();

            result.Parent.Should().BeNull();
        }

        [TestMethod]
        public void PersistenceDataForAssociationsCheck()
        {
            subject.Associations.Should().NotBeNull();
            subject.Associations.Count().Should().Be(visualisableType.AllAssociations.Count());

            for (int i = 0; i < visualisableType.AllAssociations.Count(); i++)
            {
                FieldAssociation sourceElement = visualisableType.AllAssociations.ElementAt(i);
                FieldAssociationData destinationElement = subject.Associations.ElementAt(i);

                destinationElement.AssociatedTo.Id.Should().Be(sourceElement.AssociatedTo.Id);
                destinationElement.Name.Should().Be(sourceElement.Name);
                destinationElement.UsageCount.Should().Be(sourceElement.UsageCount);
                destinationElement.GetType().Should().Be(sourceElement.PersistenceType);
            }
        }

        [TestMethod]
        public void PersistenceDataForImplementsCheck()
        {
            subject.Implements.Should().NotBeNull();
            subject.Implements.Count().Should().Be(visualisableType.ThisTypeImplements.Count());

            ParentAssociationData[] query = visualisableType.ThisTypeImplements.Join(subject.Implements, outer => outer.AssociatedTo.Id, inner => inner.AssociatedTo.Id, (real, data) => data).ToArray();

            query.Count().Should().Be(visualisableType.ThisTypeImplements.Count());
            query.All(x => x != null).Should().BeTrue();
        }

        [TestMethod]
        public void PersistenceDataIsNotNull()
        {
            subject.Should().NotBeNull();
        }

        [TestMethod]
        public void PersistenceDataModifiersConsistencyCheck()
        {
            subject.Modifiers.Abstract.Should().Be(visualisableType.Modifiers.Abstract);
            subject.Modifiers.Kind.Should().Be(visualisableType.Modifiers.Kind);
            subject.Modifiers.Internal.Should().Be(visualisableType.Modifiers.Internal);
            subject.Modifiers.Private.Should().Be(visualisableType.Modifiers.Private);
            subject.Modifiers.Sealed.Should().Be(visualisableType.Modifiers.Sealed);
            subject.Modifiers.Static.Should().Be(visualisableType.Modifiers.Static);
        }

        [TestMethod]
        public void PersistenceDataPropertiesConsistencyCheck()
        {
            visualisableType.AssemblyFullName.Should().Be(subject.AssemblyFullName);
            visualisableType.AssemblyName.Should().Be(subject.AssemblyName);
            visualisableType.ConstructorCount.Should().Be(subject.ConstructorCount);
            visualisableType.EnumMemberCount.Should().Be(subject.EnumMemberCount);
            visualisableType.EventCount.Should().Be(subject.EventCount);
            visualisableType.FieldCount.Should().Be(subject.FieldCount);
            visualisableType.AssemblyQualifiedName.Should().Be(subject.FullName);
            visualisableType.Id.Should().Be(subject.Id);
            visualisableType.LinesOfCode.Should().Be(subject.LinesOfCode);
            visualisableType.MethodCount.Should().Be(subject.MethodCount);
            visualisableType.Name.Should().Be(subject.Name);
            visualisableType.ThisTypeNamespace.Should().Be(subject.Namespace);
            visualisableType.PropertyCount.Should().Be(subject.PropertyCount);
            visualisableType.SubjectOrAssociate.Should().Be(subject.SubjectOrAssociate);
            visualisableType.TypeToolTip.Should().Be(subject.ToolTipName);
        }

        [TestMethod]
        public void PresistenceDataForParentCheck()
        {
            subject.Parent.AssociatedTo.Id.Should().Be(visualisableType.Parent.AssociatedTo.Id);
            subject.Parent.Name.Should().Be(visualisableType.Parent.Name);
        }

        [TestInitialize]
        public void TestInitialise()
        {
        }
    }
}