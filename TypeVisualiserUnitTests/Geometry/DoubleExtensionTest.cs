namespace TypeVisualiserUnitTests.Geometry
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Geometry;

    [TestClass]
    public class DoubleExtensionTest
    {
        [TestMethod]
        public void ShouldBeBetweenTest()
        {
            Assert.IsTrue(40.11.IsBetween(30, 41));
        }

        [TestMethod]
        public void ShouldNotBeBetweenTest()
        {
            Assert.IsFalse(40.11.IsBetween(41, 42));
        }
    }
}