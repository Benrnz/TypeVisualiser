namespace TypeVisualiserUnitTests.Geometry
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Geometry;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CircleCalculatorTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void PointsOnACircleTestCase1()
        {
            var center = new Point(2472, 2472);
            var calc = new CircleCalculator(center, 384);
            int angleIncrement = 30;
            for (int angle = 90; angle <= 270; angle += angleIncrement)
            {
                Point point = calc.CalculatePointOnCircle(angle);
                string result = string.Format(CultureInfo.CurrentCulture, "Drawing at ({0}, {1}) Angle is {2}", point.X, point.Y, angle);
                Debug.WriteLine(result);
            }
        }
    }
}