using System;
using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser;
using TypeVisualiser.Geometry;

namespace TypeVisualiserUnitTests.Geometry
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AreaTest
    {
        [TestMethod]
        public void CanConstructUsing2Points()
        {
            double topX = 12.43, topY = 56.89;
            double bottomX = 45.89, bottomY = 45.87;
            var topPoint = new Point(topX, topY);
            var bottomPoint = new Point(bottomX, bottomY);
            var subject = new Area(topPoint, bottomPoint);

            // Ensure it doesnt change after creation, ie cloned.
            topPoint.Offset(1, 1);
            bottomPoint.Offset(2, 2);

            Assert.AreEqual(topX, subject.TopLeft.X);
            Assert.AreEqual(topY, subject.TopLeft.Y);
            Assert.AreEqual(bottomX, subject.BottomRight.X);
            Assert.AreEqual(bottomY, subject.BottomRight.Y);
        }

        [TestMethod]
        public void CentreShouldBeAsExpected()
        {
            var subject = new Area(new Point(1, 1), new Point(3, 3));
            Point result = subject.Centre;
            Assert.AreEqual(2, result.X);
            Assert.AreEqual(2, result.Y);
        }

        [TestMethod]
        public void DistanceToPointShouldBeAsExpected()
        {
            var subject = new Area(new Point(1, 1), new Point(3, 3));
            double result = subject.DistanceToPoint(new Point(10, 10));
            Assert.AreEqual(12.72792, result, 0.00001);
        }

        [TestMethod]
        public void HeightShouldBeAsExpected()
        {
            var subject = new Area(new Point(1, 1), new Point(3, 3));
            double result = subject.Height;
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void IfThisIsNaNShouldNotThrowOverlap()
        {
            var area1 = new Area(new Point(10, 10), new Point(20, 20));
            var area2 = new Area(new Point(double.NaN, 20), new Point(25, 25));
            ProximityTestResult result = area2.OverlapsWith(area1);
            Assert.AreEqual(Proximity.NotOverlapping, result.Proximity);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void NaNShouldThrowOverlap()
        {
            var area1 = new Area(new Point(double.NaN, 10), new Point(20, 20));
            var area2 = new Area(new Point(11, 20), new Point(25, 25));
            area2.OverlapsWith(area1);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullResourceException))]
        public void NullShouldThrowOverlap()
        {
            Area area1 = null;
            var area2 = new Area(new Point(11, 20), new Point(25, 25));
            area2.OverlapsWith(area1);
            Assert.Fail();
        }

        [TestMethod]
        public void OffSetShouldMoveAsExpected()
        {
            var subject = new Area(new Point(12.34, 45.56), new Point(67.67, 89.90));
            Area result = subject.Offset(1, 1);

            Assert.AreEqual(13.34, result.TopLeft.X);
            Assert.AreEqual(46.56, result.TopLeft.Y);
            Assert.AreEqual(68.67, result.BottomRight.X);
            Assert.AreEqual(90.90, result.BottomRight.Y);
        }

        [TestMethod]
        public void OffSetShouldNotReassignReference()
        {
            var subject = new Area(new Point(12.34, 45.56), new Point(67.67, 89.90));
            Area result = subject.Offset(1, 1);

            // Ensure it doesnt change after creation, ie cloned.
            subject.Offset(1, 1);

            Assert.AreNotEqual(result.TopLeft, subject.TopLeft);
            Assert.AreNotEqual(result.BottomRight, subject.BottomRight);
        }

        [TestMethod]
        public void OffsetToCentreMustMoveToCentre()
        {
            var subject = new Area(new Point(12.34, 45.56), new Point(67.67, 89.90));
            Area result = subject.OffsetToMakeTopLeftCentre();

            Assert.AreEqual(result.Centre, subject.TopLeft);
        }

        [TestMethod]
        public void OffsetToCentreMustNotReassignReference()
        {
            var subject = new Area(new Point(12.34, 45.56), new Point(67.67, 89.90));
            Area result = subject.OffsetToMakeTopLeftCentre();

            Assert.AreNotEqual(subject.TopLeft, result.TopLeft);
        }

        [TestMethod]
        public void ShouldBeVeryCloseAndBeDown()
        {
            var area1 = new Area(new Point(20, 0), new Point(25, 5));
            var area2 = new Area(new Point(20, 20), new Point(25, 25));
            ProximityTestResult result = area1.OverlapsWith(area2);
            Assert.AreEqual(Direction.Down, result.DirectionToOtherObject);
            Assert.AreEqual(Proximity.VeryClose, result.Proximity);
        }

        [TestMethod]
        public void ShouldBeVeryCloseAndBeToTheLeft()
        {
            var area1 = new Area(new Point(10, 20), new Point(15, 25));
            var area2 = new Area(new Point(20, 20), new Point(25, 25));
            ProximityTestResult result = area2.OverlapsWith(area1);
            Assert.AreEqual(Direction.Left, result.DirectionToOtherObject);
            Assert.AreEqual(Proximity.VeryClose, result.Proximity);
        }

        [TestMethod]
        public void ShouldBeVeryCloseAndBeToTheRight()
        {
            var area1 = new Area(new Point(10, 20), new Point(15, 25));
            var area2 = new Area(new Point(20, 20), new Point(25, 25));
            ProximityTestResult result = area1.OverlapsWith(area2);
            Assert.AreEqual(Direction.Right, result.DirectionToOtherObject);
            Assert.AreEqual(Proximity.VeryClose, result.Proximity);
        }

        [TestMethod]
        public void ShouldBeVeryCloseAndBeUp()
        {
            var area1 = new Area(new Point(0, 0), new Point(5, 5));
            var area2 = new Area(new Point(0, 10), new Point(5, 15));
            ProximityTestResult result = area2.OverlapsWith(area1);
            Assert.AreEqual(Direction.Up, result.DirectionToOtherObject);
            Assert.AreEqual(Proximity.VeryClose, result.Proximity);
        }

        [TestMethod]
        public void ShouldBeVeryCloseNotTouching()
        {
            var area1 = new Area(new Point(10, 10), new Point(20, 20));
            var area2 = new Area(new Point(11, 21), new Point(25, 25));
            Assert.AreEqual(Proximity.VeryClose, area2.OverlapsWith(area1).Proximity);
        }

        [TestMethod]
        public void ShouldBeVeryCloseTouching()
        {
            var area1 = new Area(new Point(10, 10), new Point(20, 20));
            var area2 = new Area(new Point(10, 20), new Point(25, 25));
            Assert.AreEqual(Proximity.VeryClose, area2.OverlapsWith(area1).Proximity);
        }

        [TestMethod]
        public void ShouldNotOverlapAndBeDown()
        {
            var area1 = new Area(new Point(20, 0), new Point(25, 5));
            var area2 = new Area(new Point(20, 45.1), new Point(25, 65.1));
            ProximityTestResult result = area1.OverlapsWith(area2);
            result.Proximity.Should().Be(Proximity.VeryClose);
        }

        [TestMethod]
        public void ShouldNotOverlapAndBeToTheLeft()
        {
            var area1 = new Area(new Point(10, 20), new Point(15, 25));
            var area2 = new Area(new Point(55, 20), new Point(60, 25));
            ProximityTestResult result = area2.OverlapsWith(area1);
            result.Proximity.Should().Be(Proximity.VeryClose);
        }

        [TestMethod]
        public void ShouldNotOverlapAndBeToTheRight()
        {
            var area1 = new Area(new Point(10, 20), new Point(15, 25));
            var area2 = new Area(new Point(55, 20), new Point(60, 25));
            ProximityTestResult result = area1.OverlapsWith(area2);
            result.Proximity.Should().Be(Proximity.VeryClose);
        }

        [TestMethod]
        public void ShouldNotOverlapAndBeUp()
        {
            var area1 = new Area(new Point(20, 0), new Point(25, 5));
            var area2 = new Area(new Point(20, 45.1), new Point(25, 65.1));
            ProximityTestResult result = area2.OverlapsWith(area1);
            result.Proximity.Should().Be(Proximity.VeryClose);
        }

        [TestMethod]
        public void ShouldOverlap()
        {
            var area1 = new Area(new Point(10, 10), new Point(20, 20));
            var area2 = new Area(new Point(15, 15), new Point(25, 25));
            Assert.AreEqual(Proximity.Overlapping, area2.OverlapsWith(area1).Proximity);
        }

        [TestMethod]
        public void WidthShouldBeAsExpected()
        {
            var subject = new Area(new Point(1, 1), new Point(3, 3));
            double result = subject.Height;
            Assert.AreEqual(2, result);
        }
    }
}