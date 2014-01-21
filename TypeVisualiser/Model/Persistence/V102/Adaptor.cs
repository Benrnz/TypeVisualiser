using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model.Persistence.V102
{
    /// <summary>
    /// Adapts v102 to v104 
    /// </summary>
    public static class Adaptor
    {
        public static V104.TypeVisualiserLayoutFile Adapt(V103.TypeVisualiserLayoutFile oldDiagram)
        {
            return AdaptDynamic(oldDiagram);
        }

        public static V104.TypeVisualiserLayoutFile Adapt(TypeVisualiserLayoutFile oldDiagram)
        {
            return AdaptDynamic(oldDiagram);
        }

        private static List<V104.FieldAssociationData> AdaptAssociations(dynamic oldSubject)
        {
            var associationList = new List<V104.FieldAssociationData>();
            foreach (dynamic association in oldSubject.Associations)
            {
                V104.FieldAssociationData association104 = CreateAssociation(association);

                if (association104 != null)
                {
                    association104.AssociatedTo = AdaptVisualisedType(association.AssociatedTo);
                    associationList.Add(association104);
                }
            }

            return associationList;
        }

        private static void AdaptCanvasLayoutTypes(dynamic oldDiagram, dynamic canvasLayout104)
        {
            foreach (dynamic type in oldDiagram.ViewportSaveData.CanvasLayout.Types)
            {
                var type104 = new V104.TypeLayoutData { FullName = type.FullName, Id = type.Id, TopLeft = type.TopLeft, Visible = type.Visible, };
                canvasLayout104.Types.Add(type104);
            }
        }

        private static V104.TypeVisualiserLayoutFile AdaptDynamic(dynamic oldDiagram)
        {
            // oldDiagram should be a TypeVisualiserLayoutFile either v101 or v102
            if (oldDiagram == null)
            {
                throw new ArgumentNullResourceException("oldDiagram", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            dynamic diagram104 = CreateDiagram(oldDiagram);

            dynamic canvasLayout104 = CreateViewportSaveDataAndCanvasLayoutData(oldDiagram, diagram104);
            AdaptCanvasLayoutTypes(oldDiagram, canvasLayout104);

            V104.VisualisableTypeSubjectData subject104 = AdaptVisualisedSubject(oldDiagram.ViewportSaveData.Subject);
            diagram104.ViewportSaveData.Subject = subject104;

            return diagram104;
        }

        private static List<V104.ParentAssociationData> AdaptImplementsAssociations(dynamic oldSubject)
        {
            var implementsList = new List<V104.ParentAssociationData>();
            foreach (dynamic implements in oldSubject.Implements)
            {
                var implements104 = new V104.ParentAssociationData { Name = implements.Name, Show = implements.Show };
                implements104.AssociatedTo = AdaptVisualisedType(implements.AssociatedTo);
                implementsList.Add(implements104);
            }
            return implementsList;
        }

        private static void AdaptModifiers(dynamic oldType, V104.VisualisableTypeData destination)
        {
            var modifiers104 = new V104.ModifiersData
                                   {
                                       Abstract = oldType.Modifiers.Abstract,
                                       Internal = oldType.Modifiers.Internal,
                                       Kind = oldType.Modifiers.Kind,
                                       Private = oldType.Modifiers.Private,
                                       Sealed = oldType.Modifiers.Sealed,
                                       ShowConstructors = oldType.Modifiers.ShowConstructors,
                                       Static = oldType.Modifiers.Static,
                                   };
            destination.Modifiers = modifiers104;
        }

        private static V104.VisualisableTypeSubjectData AdaptVisualisedSubject(dynamic oldSubject)
        {
            // subjust should be of type VisualisableTypeSubjectData (either v101 or v102)
            var subject104 = new V104.VisualisableTypeSubjectData();
            subject104.SubjectOrAssociate = SubjectOrAssociate.Subject;
            CopyBaseVisualisedType(oldSubject, subject104);

            // Associations
            dynamic associationList = AdaptAssociations(oldSubject);
            subject104.Associations = associationList.ToArray();

            // Implements
            dynamic implementsList = AdaptImplementsAssociations(oldSubject);
            subject104.Implements = implementsList.ToArray();

            // Parent
            if (oldSubject.Parent != null)
            {
                var parent104 = new V104.ParentAssociationData { Name = oldSubject.Parent.Name, Show = oldSubject.Parent.Show };
                parent104.AssociatedTo = AdaptVisualisedType(oldSubject.Parent.AssociatedTo);
                subject104.Parent = parent104;
            }

            return subject104;
        }

        private static V104.VisualisableTypeData AdaptVisualisedType(dynamic oldType)
        {
            // oldType should be VisualisableTypeData either v101 or v102
            var type104 = new V104.VisualisableTypeData();
            type104.SubjectOrAssociate = SubjectOrAssociate.Subject;
            CopyBaseVisualisedType(oldType, type104);
            return type104;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Use of dynamic is creating an artifically high rating - this method is simple enough")]
        private static void CopyBaseVisualisedType(dynamic oldType, V104.VisualisableTypeData destination)
        {
            // oldType should be of type VisualisableTypeData either v101 or v102
            destination.AssemblyFullName = oldType.AssemblyFullName;
            destination.AssemblyName = oldType.AssemblyName;
            destination.ConstructorCount = oldType.ConstructorCount;
            destination.EnumMemberCount = oldType.EnumMemberCount;
            destination.EventCount = oldType.EventCount;
            destination.FieldCount = oldType.FieldCount;
            destination.FullName = oldType.FullName;
            destination.Id = oldType.Id;
            destination.LinesOfCode = oldType.LinesOfCode;
            destination.LinesOfCodeToolTip = oldType.LinesOfCodeToolTip;
            destination.MethodCount = oldType.MethodCount;
            destination.Name = oldType.Name;
            destination.Namespace = oldType.Namespace;
            destination.PropertyCount = oldType.PropertyCount;
            destination.Show = true;
            destination.ToolTipName = oldType.ToolTipName;

            // MOdifiers
            AdaptModifiers(oldType, destination);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Use of dynamic is creating an artifically high rating - this method is simple enough")]
        private static V104.FieldAssociationData CreateAssociation(dynamic association)
        {
            V104.FieldAssociationData association104 = null;
            if (association.GetType().Name == typeof (FieldAssociationData).Name)
            {
                association104 = new V104.FieldAssociationData { Name = association.Name, Show = association.Show, UsageCount = association.UsageCount };
            } else if (association.GetType().Name == typeof (StaticAssociationData).Name)
            {
                association104 = new V104.StaticAssociationData { Name = association.Name, Show = association.Show, UsageCount = association.UsageCount };
            } else if (association.GetType().Name == typeof (ConsumeAssociationData).Name)
            {
                association104 = new V104.ConsumeAssociationData { Name = association.Name, Show = association.Show, UsageCount = association.UsageCount };
            }

            return association104;
        }

        private static V104.TypeVisualiserLayoutFile CreateDiagram(dynamic oldDiagram)
        {
            var diagram104 = new V104.TypeVisualiserLayoutFile();
            diagram104.AssemblyFileName = oldDiagram.AssemblyFileName;
            diagram104.AssemblyFullName = oldDiagram.AssemblyFullName;
            diagram104.FileVersion = oldDiagram.FileVersion;
            diagram104.HideParents = oldDiagram.HideParents;
            diagram104.HideTrivialTypes = oldDiagram.HideTrivialTypes;
            return diagram104;
        }

        private static V104.CanvasLayoutData CreateViewportSaveDataAndCanvasLayoutData(dynamic oldDiagram, dynamic diagram104)
        {
            var viewportSaveData104 = new V104.ViewportControllerSaveData();
            diagram104.ViewportSaveData = viewportSaveData104;
            var canvasLayout104 = new V104.CanvasLayoutData();
            diagram104.ViewportSaveData.CanvasLayout = canvasLayout104;
            canvasLayout104.Subject = new V104.TypeLayoutData();
            canvasLayout104.Subject.FullName = oldDiagram.ViewportSaveData.CanvasLayout.Subject.FullName;
            canvasLayout104.Subject.Id = oldDiagram.ViewportSaveData.CanvasLayout.Subject.Id;
            canvasLayout104.Subject.TopLeft = oldDiagram.ViewportSaveData.CanvasLayout.Subject.TopLeft;
            canvasLayout104.Subject.Visible = true;
            canvasLayout104.Types = new Collection<V104.TypeLayoutData>();
            return canvasLayout104;
        }
    }
}