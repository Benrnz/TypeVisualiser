namespace TypeVisualiserUnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Model;

    [TestClass]
    public class DiagramDimensionsTest
    {
        [TestMethod]
        public void CalculateNextAvailableAngleShouldReturn12Given9ObjectsWithParentsShown()
        {
            var target = new DiagramDimensions(); 
            target.Initialise(9, true);
            target.CalculateNextAvailableAngle();
            var result = target.CalculateNextAvailableAngle();
            
            Assert.AreEqual(12.22, result, 0.005);
        }

        [TestMethod]
        public void CalculateNextAvailableAngleShouldReturn12Given9ObjectsWithParentsHidden()
        {
            var target = new DiagramDimensions();
            target.Initialise(9, false);
            target.CalculateNextAvailableAngle();
            var result = target.CalculateNextAvailableAngle();

            Assert.AreEqual(40, result, 0.005);
        }

        [TestMethod]
        public void DiagramDimensionsConstructorShouldNotThrow()
        {
            new DiagramDimensions();
        }

        [TestMethod]
        public void InitialiseWith15ShouldResetAngleList()
        {
            var target = new DiagramDimensions(); 
            target.Initialise(15, true);

            Assert.IsFalse(((List<double>)PrivateAccessor.GetField(target, "angles")).Any());
        }

        [TestMethod]
        public void InitialiseWith15ShouldSetTheProperties()
        {
            var target = new DiagramDimensions();
            target.Initialise(15, true);

            Assert.IsTrue(target.AngleIncrement > 0);
            Assert.IsTrue(target.FinishAngle > 0);
            Assert.IsTrue(target.StartAngle != 0.0);
            Assert.IsTrue(target.TotalAssociationsOnDiagram == 15);
        }
    }
}