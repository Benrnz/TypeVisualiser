using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TypeVisualiser.Geometry;
using TypeVisualiser.Model;

namespace TypeVisualiser.UI
{
    /// <summary>
    /// A class responsible for drawing the uml diagram given the collection of <see cref="IVisualisableType"/>s.
    /// </summary>
    internal class ClassUmlDrawingEngine
    {
        private ICollection<DiagramElement> allElements;

        /// <summary>
        /// This holds a temporary list of diagram elements that have been moved to their final position on the diagram.
        /// Used by the <see cref="PositionAllOtherAssociations"/> method.
        /// </summary>
        private List<DiagramElement> positionedElements;

        private Func<DiagramElement, bool, bool> shouldSecondaryElementBeVisible;

        public ClassUmlDrawingEngine(Guid diagramId, IVisualisableTypeWithAssociations mainSubject)
        {
            DiagramId = diagramId;
            MainSubject = mainSubject;
            MainDrawingSubject = new DiagramElement(DiagramId, new SubjectAssociation(mainSubject));
        }

        public Guid DiagramId { get; private set; }

        public DiagramElement MainDrawingSubject { get; private set; }

        public IVisualisableTypeWithAssociations MainSubject { get; private set; }

        public IEnumerable<DiagramElement> DrawAllBoxes()
        {
            var addedElements = new List<DiagramElement> { MainDrawingSubject };

            AddDiagramElementForParentAssociation(MainSubject.Parent, addedElements);
            foreach (ParentAssociation @interface in MainSubject.ThisTypeImplements)
            {
                AddDiagramElementForParentAssociation(@interface, addedElements);
            }

            addedElements.AddRange(MainSubject.AllAssociations.Select(association => new DiagramElement(DiagramId, association)));

            return addedElements;
        }

        /// <summary>
        /// Draw the lines to connect all types in the diagram.
        /// </summary>
        /// <param name="allDiagramElements">All the diagram types already on diagram surface and arranged.</param>
        /// <param name="shouldSecondaryElementBeVisibleDelegate">A delegate that determines if secondary associations are configured to be visible.</param>
        /// <param name="secondaryElements">An out param that returns the secondary associations so they can be shown/hidden independently of the main elements.</param>
        /// <returns>A collection of added elements that wrapping the lines that have been created.</returns>
        public IEnumerable<DiagramElement> DrawConnectingLines(ICollection<DiagramElement> allDiagramElements,
                                                               Func<DiagramElement, bool, bool> shouldSecondaryElementBeVisibleDelegate,
                                                               out Dictionary<string, DiagramElement> secondaryElements)
        {
            if (allDiagramElements.Any(x => x.DiagramContent is ConnectionLine))
            {
                throw new InvalidOperationException("Code error: Draw Association Lines cannot be called twice.");
            }

            var addedElements = new List<DiagramElement>();
            secondaryElements = new Dictionary<string, DiagramElement>();
            this.allElements = allDiagramElements;
            this.shouldSecondaryElementBeVisible = shouldSecondaryElementBeVisibleDelegate;
            foreach (DiagramElement associateElement in allDiagramElements.ToList()) // Loop through a copy of the collection. The loop will add new content to it.
            {
                if (associateElement.DiagramContent is SubjectAssociation)
                {
                    continue;
                }

                DrawLine(addedElements, associateElement, secondaryElements);
            }

            this.allElements = null;
            this.shouldSecondaryElementBeVisible = null;
            return addedElements;
        }

        public void PositionAllOtherAssociations(ICollection<DiagramElement> allDiagramElements)
        {
            this.positionedElements = new List<DiagramElement>();
            Area subjectArea = MainDrawingSubject.Area;
            this.positionedElements.Add(MainDrawingSubject);

            foreach (DiagramElement diagramElement in allDiagramElements)
            {
                var calculatablePositionContent = diagramElement.DiagramContent as ICalculatedPositionDiagramContent;
                if (calculatablePositionContent != null)
                {
                    // Only certain diagram elements should have their position calculated. The rest follow suit with those that have had their position calculated.
                    Area area = calculatablePositionContent.ProposePosition(diagramElement.Width, diagramElement.Height, subjectArea, IsOverlappingWithOtherControls);
                    diagramElement.TopLeft = area.TopLeft;
                    this.positionedElements.Add(diagramElement); // Used for IsOverlappingWithOtherControls (no need to care about overlapping with elements not yet positioned.
                }
            }

            this.positionedElements.Clear();
        }

        public void PositionMainSubject(Diagram hostDiagram)
        {
            MainDrawingSubject.CenterOnPoint(hostDiagram.Centre);
        }

        private void AddDiagramElementForParentAssociation(ParentAssociation parent, IList<DiagramElement> addedElements)
        {
            if (parent == null)
            {
                return;
            }

            addedElements.Add(new DiagramElement(DiagramId, parent));

            var parentType = parent.AssociatedTo as IVisualisableTypeWithAssociations;
            if (parentType != null)
            {
                if (parentType.ThisTypeImplements != null)
                {
                    foreach (ParentAssociation grandParent in parentType.ThisTypeImplements)
                    {
                        AddDiagramElementForParentAssociation(grandParent, addedElements);
                    }
                }

                if (parentType.Parent != null)
                {
                    AddDiagramElementForParentAssociation(parentType.Parent, addedElements);
                }
            }
        }

        private void AppendToCollections(List<DiagramElement> addedElements, Dictionary<string, DiagramElement> secondaryElements, IEnumerable<DiagramElement> results)
        {
            foreach (DiagramElement element in results)
            {
                addedElements.Add(element);
                secondaryElements.Add(element.DiagramContent.Id, element);
                element.IsVisibleAdditionalLogic = this.shouldSecondaryElementBeVisible;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IDiagramContent", Justification = "Known context word")]
        private void DrawLine(List<DiagramElement> addedElements, DiagramElement associateElement, Dictionary<string, DiagramElement> secondaryElements)
        {
            var association = associateElement.DiagramContent as Association;
            if (association == null)
            {
                throw new InvalidCastException("Code Error: Incorrect IDiagramContent type used with type dependency diagram, it must be an Association derivative.");
            }

            // Draw the primary line from the diagram's subject to the association. This could also be a line from the subject's parent to a grandparent.
            DiagramElement fromElement = GetSourceOfLine(association); // Sometimes returns the subject or a parent connecting to a grandparent.
            addedElements.AddRange(DrawLine(fromElement, associateElement, association));

            // Draw any lines from this diagram element to its associations that are included on the diagram.
            IEnumerable<DiagramElement> results = DrawLineForSecondaryParent(associateElement, association);
            AppendToCollections(addedElements, secondaryElements, results);

            results = DrawLineForSecondaryImplements(associateElement, association);
            AppendToCollections(addedElements, secondaryElements, results);

            results = DrawLineForSecondaryAssociations(associateElement, association);
            AppendToCollections(addedElements, secondaryElements, results);
        }

        private IEnumerable<DiagramElement> DrawLine(DiagramElement fromElement, DiagramElement destinationElement, Association destinationAssociation)
        {
            Logger.Instance.WriteEntry("DrawLine for   {0}", destinationAssociation.AssociatedTo.Name);
            Logger.Instance.WriteEntry("    From Area: {0}", fromElement.Area);
            Logger.Instance.WriteEntry("    To Area  : {0}", destinationElement.Area);

            ConnectionLine route = ConnectionLine.FindBestConnectionRoute(fromElement.Area, destinationElement.Area, IsOverlappingWithOtherControls);
            destinationAssociation.StyleLine(route);
            var addedElements = new List<DiagramElement>();
            Logger.Instance.WriteEntry("    Route calculated from {0:F1}, {1:F1}  to {2:F1}, {3:F1}", route.From.X, route.From.Y, route.To.X, route.To.Y);
            Logger.Instance.WriteEntry("    From angle {0:F1} to angle {1:F1}", route.FromAngle, route.ToAngle);

            // Create the line diagram element and add to the diagram collection.
            // The line is linked to the arrow head position.
            var lineDiagramElement = new DiagramElement(DiagramId, route) { TopLeft = route.From };
            lineDiagramElement.RegisterPositionDependency(new[] { fromElement, destinationElement }, IsOverlappingWithOtherControls);
            addedElements.Add(lineDiagramElement);

            // Create an arrow head based on the best route and add to the diagram collection.
            // The arrow head is linked to the associate diagram element.
            ArrowHead arrowHead = destinationAssociation.CreateLineHead();
            var headDiagramElement = new DiagramElement(DiagramId, arrowHead) { TopLeft = route.To };
            headDiagramElement.RegisterPositionDependency(new[] { lineDiagramElement }, IsOverlappingWithOtherControls);
            addedElements.Add(headDiagramElement);

            return new[] { lineDiagramElement, headDiagramElement };
        }

        private IEnumerable<DiagramElement> DrawLineForSecondaryAssociations(DiagramElement fromDiagramElement, Association association)
        {
            var fullyExpandedModelType = association.AssociatedTo as IVisualisableTypeWithAssociations;
            if (fullyExpandedModelType == null || !fullyExpandedModelType.AllAssociations.Any())
            {
                return new List<DiagramElement>();
            }

            var addedElements = new List<DiagramElement>();
            foreach (FieldAssociation fieldAssociation in fullyExpandedModelType.AllAssociations)
            {
                DiagramElement destinationElement = FindDiagramElementFromContentId(fieldAssociation.Id);
                if (destinationElement == null)
                {
                    continue;
                }

                var destinationAssociation = destinationElement.DiagramContent as Association;
                addedElements.AddRange(DrawLine(fromDiagramElement, destinationElement, destinationAssociation));
            }

            return addedElements;
        }

        private IEnumerable<DiagramElement> DrawLineForSecondaryImplements(DiagramElement fromDiagramElement, Association association)
        {
            var fullyExpandedModelType = association.AssociatedTo as IVisualisableTypeWithAssociations;
            if (fullyExpandedModelType == null || !fullyExpandedModelType.ThisTypeImplements.Any())
            {
                return new List<DiagramElement>();
            }

            var addedElements = new List<DiagramElement>();
            foreach (ParentAssociation implement in fullyExpandedModelType.ThisTypeImplements)
            {
                DiagramElement destinationElement = FindDiagramElementFromContentId(implement.Id);
                if (destinationElement == null)
                {
                    continue;
                }

                var destinationAssociation = destinationElement.DiagramContent as Association;
                addedElements.AddRange(DrawLine(fromDiagramElement, destinationElement, destinationAssociation));
            }

            return addedElements;
        }

        private IEnumerable<DiagramElement> DrawLineForSecondaryParent(DiagramElement fromDiagramElement, Association parentAssociation)
        {
            var fullyExpandedModelType = parentAssociation.AssociatedTo as IVisualisableTypeWithAssociations;
            if (fullyExpandedModelType == null || fullyExpandedModelType.Parent == null)
            {
                return new List<DiagramElement>();
            }

            DiagramElement destinationElement = FindDiagramElementFromContentId(fullyExpandedModelType.Parent.Id);
            if (destinationElement == null)
            {
                return new List<DiagramElement>();
            }

            var destinationAssociation = destinationElement.DiagramContent as Association;
            return DrawLine(fromDiagramElement, destinationElement, destinationAssociation);
        }

        private DiagramElement FindDiagramElementFromContentId(string id)
        {
            return this.allElements.FirstOrDefault(e => e.DiagramContent.Id == id && e.DiagramContent is Association);
        }

        private DiagramElement GetSourceOfLine(Association association)
        {
            var parentAssociation = association as ParentAssociation;
            if (parentAssociation == null)
            {
                return MainDrawingSubject;
            }

            DiagramElement search = this.allElements.FirstOrDefault(e => e.DiagramContent.Id == parentAssociation.AssociatedFrom.Id);
            if (search == null)
            {
                return MainDrawingSubject;
            }

            return search;
        }

        /// <summary>
        /// Determines whether the given area is overlapping with other areas occupied by controls.
        /// </summary>
        /// <param name="proposedArea">The proposed area to compare with all others.</param>
        /// <returns>A result object indicating if an overlap exists or the closest object and distance to it.</returns>
        private ProximityTestResult IsOverlappingWithOtherControls(Area proposedArea)
        {
            // Only check for overlapps with elements that are already in their final position.
            List<ProximityTestResult> proximities = this.positionedElements.Select(diagramElement => diagramElement.Area.OverlapsWith(proposedArea)).ToList();
            bool overlapsWith = proximities.Any(result => result.Proximity == Proximity.Overlapping);
            if (overlapsWith)
            {
                return new ProximityTestResult(Proximity.Overlapping);
            }

            IOrderedEnumerable<ProximityTestResult> veryClose = proximities.Where(x => x.Proximity == Proximity.VeryClose).OrderBy(x => x.DistanceToClosestObject);

            if (veryClose.Any())
            {
                return veryClose.First();
            }

            return new ProximityTestResult(Proximity.NotOverlapping);
        }
    }
}