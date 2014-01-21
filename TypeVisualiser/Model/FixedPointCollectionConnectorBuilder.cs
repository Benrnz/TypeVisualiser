using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TypeVisualiser.Geometry;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// A connector builder implementation that snaps connection lines to one of 8 fixed connection points. The closest connection point to the intersection point is chosen.
    /// </summary>
    internal class FixedPointCollectionConnectorBuilder : IConnectorBuilder
    {
        public ConnectionLine CalculateBestConnection(Area fromArea, Area destinationArea, Func<Area, ProximityTestResult> isOverlappingWithOtherControls)
        {
            var fromConnectors = GetAllConnectorPoints(fromArea, false);
            var toLineEndConnectors = GetAllConnectorPoints(destinationArea, true);
            var toArrowTipConnectors = GetAllConnectorPoints(destinationArea, false);
            // Logger.Instance.WriteEntry("Finding Best Connection Line from {0} to {1}", (fromArea.Tag ?? fromArea.TopLeft.ToString()), (destinationArea.Tag ?? destinationArea.TopLeft.ToString()));
            // Logger.Instance.WriteEntry("    From: {0}  To: {1}", fromArea, destinationArea);

            double angle = fromArea.Centre.AngleToPointInDegrees(destinationArea.Centre);
            var idealPoint = fromArea.CalculateCircumferenceIntersectionPoint(angle);
            var chosenfromConnector = fromConnectors.Select(fromConnector => new Tuple<KeyValuePair<int, Point>, double>(fromConnector, fromConnector.Value.DistanceTo(idealPoint)))
                .OrderBy(x => x.Item2)
                .First();
            var toConnectorsInOrderOfPreference = toLineEndConnectors.Select(toConnector => new Tuple<KeyValuePair<int, Point>, double>(toConnector, toConnector.Value.DistanceTo(idealPoint)))
                .OrderBy(x => x.Item2)
                .ToList();

            // This code checks to see if the arrow head is overlapping with an existing diagram element. (not really needed)
            //int chosenToIndex = 0;
            //for (int toConnectorIndex = 0; toConnectorIndex < toConnectors.Length; toConnectorIndex++)
            //{
            //    var toConnector = toConnectorsInOrderOfPreference[toConnectorIndex];
            //    Area arrowheadArea = ArrowHead.GetArea(CalculateExitAngle(toConnectorIndex), toConnector.Item1);
            //    ProximityTestResult proximityTestResult = isOverlappingWithOtherControls(arrowheadArea);
            //    if (proximityTestResult.Proximity == Proximity.NotOverlapping || proximityTestResult.Proximity == Proximity.VeryClose)
            //    {
            //        chosenToIndex = toConnectorIndex;
            //        break;
            //    }
            //} 

            return new ConnectionLine
                {
                    Distance = chosenfromConnector.Item1.Value.DistanceTo(toConnectorsInOrderOfPreference[0].Item1.Value),
                    From = chosenfromConnector.Item1.Value,
                    To = toArrowTipConnectors[toConnectorsInOrderOfPreference[0].Item1.Key], // toConnectorsInOrderOfPreference[0].Item1.Value, // Incorrect 
                    ToLineEnd = toConnectorsInOrderOfPreference[0].Item1.Value,
                    ExitAngle = CalculateExitAngle(toConnectorsInOrderOfPreference[0].Item1.Key)
                }; 
        }

        private static int CalculateExitAngle(int toSide)
        {
            int x = (toSide / 2) * 90;
            return x;
        }

        private static Dictionary<int, Point> GetAllConnectorPoints(Area rectangle, bool isHead)
        {
            // All arrow heads are 41 units long.
            int leaveSpaceForArrowHead = isHead ? ArrowHead.ArrowWidth : 0;
            int index = 0;
            var connectors = new Dictionary<int, Point>(8);
            Point tempPoint;
            double oneThirdOfHeight = rectangle.Height / 3;
            double oneThirdOfWidth = rectangle.Width / 3;

            // Lower Left
            tempPoint = rectangle.TopLeft.Clone();
            tempPoint.Offset(-leaveSpaceForArrowHead, oneThirdOfHeight * 2);
            connectors[index++] = tempPoint;

            // Upper Left
            tempPoint = rectangle.TopLeft.Clone();
            tempPoint.Offset(-leaveSpaceForArrowHead, oneThirdOfHeight);
            connectors[index++] = tempPoint;

            // Left Top
            tempPoint = rectangle.TopLeft.Clone();
            tempPoint.Offset(oneThirdOfWidth, -leaveSpaceForArrowHead);
            connectors[index++] = tempPoint;

            // Right Top
            tempPoint = rectangle.TopLeft.Clone();
            tempPoint.Offset(oneThirdOfWidth * 2, -leaveSpaceForArrowHead);
            connectors[index++] = tempPoint;

            // Upper Right
            tempPoint = rectangle.BottomRight.Clone();
            tempPoint.Offset(leaveSpaceForArrowHead, -oneThirdOfHeight * 2);
            connectors[index++] = tempPoint;

            // Lower Right
            tempPoint = rectangle.BottomRight.Clone();
            tempPoint.Offset(leaveSpaceForArrowHead, -oneThirdOfHeight);
            connectors[index++] = tempPoint;

            // Right Bottom
            tempPoint = rectangle.BottomRight.Clone();
            tempPoint.Offset(-oneThirdOfWidth * 2, leaveSpaceForArrowHead);
            connectors[index++] = tempPoint;

            // Left Bottom
            tempPoint = rectangle.BottomRight.Clone();
            tempPoint.Offset(-oneThirdOfWidth, leaveSpaceForArrowHead);
            connectors[index] = tempPoint;

            return connectors;
        }
    }
}