using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser.Geometry;

namespace TypeVisualiserUnitTests.Geometry
{
    [TestClass]
    public class PointExtensionTest
    {
        private Point source = new Point(205, 205);

        [TestMethod]
        public void ShouldBe0Degrees()
        {
            var result = PointExtension.AngleToPointInDegrees(new Point(500, 300), new Point(500, 57.2));
            result.Should().BeInRange(0, 0.001);
        }

        [TestMethod]
        public void ShouldBe157Degrees()
        {
            var result = PointExtension.AngleToPointInDegrees(new Point(354, 520), new Point(467, 776));
            result.Should().BeInRange(156, 157);
        }

        [TestMethod]
        public void ShouldBe238Degrees()
        {
            var result = PointExtension.AngleToPointInDegrees(new Point(354, 520), new Point(102, 681));
            result.Should().BeInRange(237, 238);
        }

        [TestMethod]
        public void ShouldBe327Degrees()
        {
            var result = PointExtension.AngleToPointInDegrees(new Point(645.1, 520.6), new Point(556.7, 386.2));
            result.Should().BeInRange(326, 327);
        }

        [TestMethod]
        public void ShouldBe33Degrees()
        {
            var result = PointExtension.AngleToPointInDegrees(new Point(354.9, 520.6), new Point(443.3, 386.2));
            result.Should().BeInRange(33.3, 33.4);
        }

        [TestMethod]
        public void ShouldBeToTheLeft()
        {
            var destination = new Point(5, 205);
            var result = this.source.AngleToPointInDegrees(destination);
            System.Diagnostics.Debug.WriteLine("Angle = " + result);
            result.Should().BeInRange(270.0, 270.1);
        }

        [TestMethod]
        public void ShouldBeDiagonalUpAndLeft()
        {
            var destination = new Point(55, 55);
            var result = this.source.AngleToPointInDegrees(destination);
            System.Diagnostics.Debug.WriteLine("Angle = " + result);
            result.Should().BeInRange(314.9, 315.1);
        }

        [TestMethod]
        public void ShouldBelUp()
        {
            var destination = new Point(205, 5);
            var result = this.source.AngleToPointInDegrees(destination);
            System.Diagnostics.Debug.WriteLine("Angle = " + result);
            result.Should().BeInRange(0, 0.1);
        }

        [TestMethod]
        public void ShouldBeDiagonalUpAndRight()
        {
            var destination = new Point(255, 55);
            var result = this.source.AngleToPointInDegrees(destination);
            System.Diagnostics.Debug.WriteLine("Angle = " + result);
            result.Should().BeInRange(18.43, 18.439);
        }

        [TestMethod]
        public void ShouldBeRight()
        {
            var destination = new Point(405, 205);
            var result = this.source.AngleToPointInDegrees(destination);
            System.Diagnostics.Debug.WriteLine("Angle = " + result);
            result.Should().BeInRange(90, 90.1);
        }

        [TestMethod]
        public void ShouldBeDown()
        {
            var destination = new Point(205, 405);
            var result = this.source.AngleToPointInDegrees(destination);
            System.Diagnostics.Debug.WriteLine("Angle = " + result);
            result.Should().BeInRange(180, 180.1);
        }
    }
}
