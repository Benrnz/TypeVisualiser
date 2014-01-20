namespace TypeVisualiserUnitTests.ILAnalyser
{
    using System;
    using System.Linq;
    using System.Reflection.Emit;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.ILAnalyser;

    [TestClass]
    public class MethodBodyReaderTest
    {
        private MethodBodyReader target;

        [TestInitialize]
        public void TestInitialise()
        {
            target = new MethodBodyReader();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            target = null;
        }

        [TestMethod]
        public void ReadShouldThrowIfGlobalsNotInitialised()
        {
            PrivateAccessor.SetStaticField(typeof(GlobalIntermediateLanguageConstants), "singleByteOpCodes", new ReadOnlyDictionary<int, OpCode>());
            PrivateAccessor.SetStaticField(typeof(GlobalIntermediateLanguageConstants), "isInitialised", false);
            Action act = () => target.Read(typeof(Car).GetMethod("Move"));
            act.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ReadShouldCreateCorrectNumberOfInstructions()
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            target.Read(typeof(Car).GetMethod("Move"));
            target.Instructions.Count().Should().Be(16);
        }
    }
}
