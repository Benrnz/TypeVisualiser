using System;
using System.Collections.Concurrent;
using TypeVisualiser.Startup;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeSubjectImplementsTest
    {
        private static VisualisableTypeWithAssociations subject;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            var cache = PrivateAccessor.GetStaticField(typeof(ModelBuilder), "TypeCache") as ConcurrentDictionary<Type, IVisualisableType>;
            cache.Clear();
            IoC.MapHardcodedRegistrations();
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            subject = new VisualisableTypeWithAssociations(typeof(Car));
        }

        [TestMethod]
        public void HasICloneable()
        {
            subject.ThisTypeImplements.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("System.ICloneable")).Should().BeTrue();
        }

        [TestMethod]
        public void HasITransportProvider()
        {
            subject.ThisTypeImplements.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("TypeVisualiser.DemoTypes.ITransportProvider")).Should().BeTrue();
        }

        [TestMethod]
        public void HasIWearAndTear()
        {
            subject.ThisTypeImplements.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("TypeVisualiser.DemoTypes.IWearAndTear")).Should().BeTrue();
        }

        [TestMethod]
        public void ShouldHaveCorrectCount()
        {
            List<ParentAssociation> result = subject.ThisTypeImplements.ToList();
            result.ForEach(x => Debug.WriteLine(x.ToString()));
            result.Count().Should().Be(3);
        }

        [TestMethod]
        public void ShouldNotHaveVehicle()
        {
            subject.ThisTypeImplements.Any(x => x.AssociatedTo.AssemblyQualifiedName.StartsWith("TypeVisualiser.DemoTypes.Vehicle")).Should().BeFalse();
        }

        [TestInitialize]
        public void TestInitialise()
        {
        }
    }
}