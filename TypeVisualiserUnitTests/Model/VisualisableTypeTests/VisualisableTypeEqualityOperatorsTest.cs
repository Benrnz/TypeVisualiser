namespace TypeVisualiserUnitTests.Model.VisualisableTypeTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;

    [TestClass]
    public class VisualisableTypeEqualityOperatorsTest
    {
        private static VisualisableType stringType1;
        private static VisualisableType stringType2;
        private static VisualisableType intType1;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
            stringType1 = new VisualisableType(typeof(string));
            stringType2 = new VisualisableType(typeof(string));
            intType1 = new VisualisableType(typeof(int));
        }

        [TestMethod]
        public void NullDoesNotEqualNullShouldBeFalse()
        {
            VisualisableType op1 = null;
            VisualisableType op2 = null;
            Assert.IsFalse(op1 != op2);
        }

        [TestMethod]
        public void NullEqualsNullShouldBeTrue()
        {
            VisualisableType op1 = null;
            VisualisableType op2 = null;
            Assert.IsTrue(op1 == op2);
        }

        [TestMethod]
        public void Op1DoesNotEqualOp2ShouldBeFalse()
        {
            Assert.IsFalse(stringType1 != stringType2);
        }

        [TestMethod]
        public void Op1DoesNotEqualOp2ShouldBeTrue()
        {
            Assert.IsTrue(stringType1 != intType1);
        }

        [TestMethod]
        public void Op1EqualsOp2ShouldBeFalse()
        {
            Assert.IsFalse(stringType1.Equals(intType1));
            Assert.IsFalse(stringType1== intType1);
        }

        [TestMethod]
        public void Op1EqualsOp2ShouldBeTrue()
        {
            Assert.IsTrue(stringType1.Equals(stringType2));
            Assert.IsTrue(stringType1 == stringType2);
        }

        [TestMethod]
        public void Op1NullDoesNotEqualOp2ShouldBeTrue()
        {
            VisualisableType op1 = null;
            Assert.IsTrue(op1 != stringType1);
        }

        [TestMethod]
        public void Op1NullEqualsOp2ShouldBeFalse()
        {
            VisualisableType op1 = null;
            Assert.IsFalse(stringType1.Equals(op1));
            Assert.IsFalse(op1 == stringType1);
        }

        [TestMethod]
        public void Op2NullDoesNotEqualsOp1ShouldBeTrue()
        {
            VisualisableType op2 = null;
            Assert.IsTrue(stringType1 != op2);
        }

        [TestMethod]
        public void Op2NullEqualsOp1ShouldBeFalse()
        {
            VisualisableType op2 = null;
            Assert.IsFalse(stringType1.Equals(op2));
            Assert.IsFalse(stringType1 == op2);
        }
    }
}