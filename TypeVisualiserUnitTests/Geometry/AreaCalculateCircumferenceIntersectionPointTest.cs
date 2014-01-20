using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser.Geometry;

namespace TypeVisualiserUnitTests.Geometry
{
    /// <summary>
    /// Used http://www.carbidedepot.com/formulas-trigright.asp to visual triangles and double check calculations
    /// </summary>
    [TestClass]
    public class AreaCalculateCircumferenceIntersectionPointTest
    {
        private Area testArea1 = new Area(new Point(0, 0), 100, 100);
        private Area testArea2 = new Area(new Point(424.5, 213.81), new Point(575.5, 386.19));

        [TestMethod]
        public void BugWhereExitsOnHorizontalQuadrant1WasInsideRectangleNotOnCircumference()
        {
            var result = testArea2.CalculateCircumferenceIntersectionPoint(146.6666666666);
            result.Y.Should().BeInRange(386, 387);
        }

        [TestMethod]
        public void BugWhereExitsOnHorizontalQuadrant0WasInsideRectangleNotOnCircumference()
        {
            var result = testArea2.CalculateCircumferenceIntersectionPoint(0.0);
            result.Y.Should().BeInRange(213, 214);
        }

        [TestMethod]
        public void Given0DegreeAngleShouldReturnMiddleTop()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(0);
            result.X.Should().BeInRange(49.999999, 50.000001);
            result.Y.Should().BeInRange(-0.00001, 0.000001);
        }

        [TestMethod]
        public void Given90DegreeAngleShouldReturnMiddleRight()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(90);
            result.X.Should().BeInRange(99.999999, 100.000001);
            result.Y.Should().BeInRange(49.999999,  50.000001);
        }

        [TestMethod]
        public void Given180DegreeAngleShouldReturnMiddleBottom()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(180);
            result.X.Should().BeInRange(49.999999, 50.000001);
            result.Y.Should().BeInRange(99.999999, 100.000001);
        }

        [TestMethod]
        public void Given270DegreeAngleShouldReturnMiddleBottom()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(270);
            result.X.Should().BeInRange(-0.0000001, 0.00000001);
            result.Y.Should().BeInRange(49.999999, 50.000001);
        }

        [TestMethod]
        public void Given480DegreeAngleShouldReturnMiddleBottom()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(480);
            result.X.Should().BeInRange(99.999999, 100.0000001);
            result.Y.Should().BeInRange(78.8675, 78.8676);
        }

        [TestMethod]
        public void GivenAngleInQuadrant0ExitsOnHorizontal()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(40);
            result.X.Should().BeInRange(91.95, 92);
            result.Y.Should().BeInRange(-0.00001, 0.000001);
        }

        [TestMethod]
        public void GivenAngleInQuadrant0ExitsOnVertical()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(80);
            result.X.Should().BeInRange(99.999999, 100.0000001);
            result.Y.Should().BeInRange(41.183, 41.1837);
        }

        [TestMethod]
        public void GivenAngleInQuadrant1ExitsOnHorizontal()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(157);
            result.X.Should().BeInRange(71.223, 71.224);
            result.Y.Should().BeInRange(99.999999, 100.000001);
        }

        [TestMethod]
        public void GivenAngleInQuadrant1ExitsOnVertical()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(120);
            result.X.Should().BeInRange(99.999999, 100.0000001);
            result.Y.Should().BeInRange(78.8675, 78.8676);
        }

        [TestMethod]
        public void GivenAngleInQuadrant2ExitsOnHorizontal()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(205);
            result.X.Should().BeInRange(26.6846, 26.6847);
            result.Y.Should().BeInRange(99.999999, 100.000001);
        }

        [TestMethod]
        public void GivenAngleInQuadrant2ExitsOnVertical()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(251);
            result.X.Should().BeInRange(-0.00001, 0.0000001);
            result.Y.Should().BeInRange(67.2163, 67.2164);
        }

        [TestMethod]
        public void GivenAngleInQuadrant3ExitsOnHorizontal()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(359);
            result.X.Should().BeInRange(49.1272, 49.1273);
            result.Y.Should().BeInRange(-0.000001, 0.000001);
        }

        [TestMethod]
        public void GivenAngleInQuadrant3ExitsOnVertical()
        {
            var result = testArea1.CalculateCircumferenceIntersectionPoint(270.01);
            result.X.Should().BeInRange(-0.00001, 0.0000001);
            result.Y.Should().BeInRange(49.9912, 49.9913);
        }
    }
}
