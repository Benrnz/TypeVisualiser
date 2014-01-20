namespace TypeVisualiserUnitTests.ILAnalyser
{
    using System;
    using System.Reflection.Emit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.ILAnalyser;
    using FluentAssertions;

    [TestClass]
    public class GlobalsTest
    {
        [TestMethod]
        public void LoadOpCodesShouldPopulateSingleByteOpCodes()
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            GlobalIntermediateLanguageConstants.SingleByteOpCodes.Count.Should().Be(199);
        }

        [TestMethod]
        public void LoadOpCodesShouldPopulateMultiByteOpCodes()
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            GlobalIntermediateLanguageConstants.MultiByteOpCodes.Count.Should().Be(27);
        }

        [TestMethod]
        public void MultiByteOpCodesShouldBeReadonly()
        {
            Action act = () => GlobalIntermediateLanguageConstants.MultiByteOpCodes.Add(12, new OpCode());
            act.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void SingleByteOpCodesShouldBeReadonly()
        {
            Action act = () => GlobalIntermediateLanguageConstants.SingleByteOpCodes.Add(12, new OpCode());
            act.ShouldThrow<NotSupportedException>();
        }
    }
}
