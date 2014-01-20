namespace TypeVisualiserUnitTests.Model.VisualisableTypeTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;
    using TypeVisualiser.Model.Persistence;

    [TestClass]
    public class VisualisableTypePersistenceTest
    {
        private static VisualisableType visualisableType;
        private static VisualisableTypeData subject;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            visualisableType = new VisualisableType(typeof(Car));
            var accessor = VisualisableType_Accessor.AttachShadow(visualisableType);
            subject = accessor.ExtractPersistentData();
        }

        [TestInitialize]
        public void TestInitialise()
        {
        }

        [TestMethod]
        public void PersistenceDataIsNotNull()
        {
            Assert.IsNotNull(subject);
        }

        [TestMethod]
        public void PersistenceDataPropertiesConsistencyCheck()
        {
            Assert.AreEqual(visualisableType.AssemblyFullName, subject.AssemblyFullName);
            Assert.AreEqual(visualisableType.AssemblyName, subject.AssemblyName);
            Assert.AreEqual(visualisableType.ConstructorCount, subject.ConstructorCount);
            Assert.AreEqual(visualisableType.EnumMemberCount, subject.EnumMemberCount);
            Assert.AreEqual(visualisableType.EventCount, subject.EventCount);
            Assert.AreEqual(visualisableType.FieldCount, subject.FieldCount);
            Assert.AreEqual(visualisableType.AssemblyQualifiedName, subject.FullName);
            Assert.AreEqual(visualisableType.Id, subject.Id);
            Assert.AreEqual(visualisableType.LinesOfCode, subject.LinesOfCode);
            Assert.AreEqual(visualisableType.MethodCount, subject.MethodCount);
            Assert.AreEqual(visualisableType.Name, subject.Name);
            Assert.AreEqual(visualisableType.ThisTypeNamespace, subject.Namespace);
            Assert.AreEqual(visualisableType.PropertyCount, subject.PropertyCount);
            Assert.AreEqual(visualisableType.SubjectOrAssociate, subject.SubjectOrAssociate);
            Assert.AreEqual(visualisableType.TypeToolTip, subject.ToolTipName);
        }

        [TestMethod]
        public void PersistenceDataModifiersConsistencyCheck()
        {
            Assert.AreEqual(visualisableType.Modifiers.Abstract, subject.Modifiers.Abstract);
            Assert.AreEqual(visualisableType.Modifiers.Kind, subject.Modifiers.Kind);
            Assert.AreEqual(visualisableType.Modifiers.Internal, subject.Modifiers.Internal);
            Assert.AreEqual(visualisableType.Modifiers.Private, subject.Modifiers.Private);
            Assert.AreEqual(visualisableType.Modifiers.Sealed, subject.Modifiers.Sealed);
            Assert.AreEqual(visualisableType.Modifiers.Static, subject.Modifiers.Static);
        }

    }
}
