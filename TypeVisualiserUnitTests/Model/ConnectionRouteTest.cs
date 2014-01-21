using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeVisualiser;
using TypeVisualiser.Geometry;
using TypeVisualiser.Model;

namespace TypeVisualiserUnitTests.Model
{
    [TestClass]
    public class ConnectionRouteTest
    {
        private Area[] allAreas;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullResourceException))]
        public void CompareToShouldThrowWhenGivenNull()
        {
            var target = new ConnectionLine();
            var result = target.CompareTo(null);
        }

        [TestMethod]
        public void CompareToShouldReturnPositive()
        {
            var target = new ConnectionLine { Distance = 20.1 };
            var compareTo = new ConnectionLine { Distance = 10.1 };
            var result = target.CompareTo(compareTo);
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void CompareToShouldReturnNegative()
        {
            var target = new ConnectionLine { Distance = 3.4 };
            var compareTo = new ConnectionLine { Distance = 10.1 };
            var result = target.CompareTo(compareTo);
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void CompareToShouldReturnZero()
        {
            var target = new ConnectionLine { Distance = 3.4 };
            var compareTo = new ConnectionLine { Distance = 3.4 };
            var result = target.CompareTo(compareTo);
            Assert.IsTrue(result == 0);
        }

        [TestMethod]
        public void CompareToObjectShouldReturnNegative()
        {
            var target = new ConnectionLine { Distance = 3.4 };
            object compareTo = new ConnectionLine { Distance = 10.1 };
            var result = target.CompareTo(compareTo);
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void FindBestConnectionRouteShouldPointUpWhenDirectlyAbove()
        {
            var area1 = new Area(new Point(200, 200), 10, 10); // Subject
            var area2 = new Area(new Point(200, 0), 10, 10);
            this.allAreas = new[] { area1, area2 };
            var route = ConnectionLine.FindBestConnectionRoute(area1, area2, IsOverlapping);

            route.ExitAngle.Should().BeInRange(269.9999, 270.0001); // Pointing Up
            route.Distance.Should().BeInRange(154.08, 154.089);
        }

        [TestMethod]
        public void FindBestConnectionRouteShouldPointUpWhenSlightlyToLeft()
        {
            var area1 = new Area(new Point(200, 200), 30, 30); // Subject
            var area2 = new Area(new Point(149, 0), 30, 30);
            this.allAreas = new[] { area1, area2 };
            var route = ConnectionLine.FindBestConnectionRoute(area1, area2, IsOverlapping);

            route.ExitAngle.Should().BeInRange(269.9999, 270.0001); // Pointing Up
            route.Distance.Should().BeInRange(148.43, 148.9);
        }

        [TestMethod]
        public void FindBestConnectionRouteShouldPointUpWhenSlightlyToLeftAndClose()
        {
            var area1 = new Area(new Point(667.377, 232.822), new Point(818.377, 429.153)) { Tag = "Subject" }; // Subject
            var area2 = new Area(new Point(667.377, 0), new Point(826.937, 130.322)) { Tag = "Area2" }; // Directly above subject (667.37715347279,0) (826.93715347279,130.322792358398)
            var area3 = new Area(new Point(476.377, 0), new Point(627.377, 130.322)) { Tag = "Area3" }; // To the left and above subject
            this.allAreas = new[] { area1, area2, area3 };

            var route = ConnectionLine.FindBestConnectionRoute(area1, area3, IsOverlapping);

            route.ExitAngle.Should().BeInRange(269.9999, 270.0001); // Pointing Up
            route.Distance.Should().BeInRange(197.06, 197.069);
        }

        private ProximityTestResult IsOverlapping(Area proposedArea)
        {
            List<ProximityTestResult> proximities = this.allAreas.Select(x => x.OverlapsWith(proposedArea)).ToList();
            bool overlapsWith = proximities.Any(result => result.Proximity == Proximity.Overlapping);
            if (overlapsWith)
            {
                return new ProximityTestResult(Proximity.Overlapping);
            }

            var veryClose = proximities.Where(x => x.Proximity == Proximity.VeryClose).OrderBy(x => x.DistanceToClosestObject);

            if (veryClose.Any())
            {
                return veryClose.First();
            }

            return new ProximityTestResult(Proximity.NotOverlapping);
        }
    }
}