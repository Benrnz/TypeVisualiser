using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeVisualiser.Properties;

namespace TypeVisualiser.Model.Persistence.V104
{
    /// <summary>
    /// A collection of tools to assist in cloning and copying objects and their properties.
    /// </summary>
    internal static class CloningTools
    {
        /// <summary>
        /// Copys an object by Reflection. Makes the following assumptions: 
        /// Assumes that properties from the source exist on the destination.
        /// Assumes that properties of the same name are of the same type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void ReflectionCopy(object source, object destination)
        {
            var nullArguments = new object[] { };
            Type destinationType = destination.GetType();
            Type sourceType = source.GetType();
            var properties = GetAllPropertiesFromHierarchy(sourceType);
            foreach (PropertyInfo property in properties)
            {
                object sourcePropertyValue = property.GetValue(source, nullArguments);
                PropertyInfo destinationProperty = destinationType.GetProperty(property.Name);
                if (sourcePropertyValue == null)
                {
                    destinationProperty.SetValue(destination, null, nullArguments);
                    continue;
                }

                if (property.PropertyType == typeof(Guid))
                {
                    AssignGuidValue(destination, sourcePropertyValue, destinationProperty);
                    continue;
                }

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    destinationProperty.SetValue(destination, sourcePropertyValue, nullArguments);
                    continue;
                }

                object newDestination;
                if (destinationProperty.PropertyType.IsArray)
                {
                    newDestination = ReflectionCopyArray(destinationProperty.PropertyType, sourcePropertyValue);
                } else
                {
                    newDestination = Activator.CreateInstance(destinationProperty.PropertyType);
                    ReflectionCopy(sourcePropertyValue, newDestination);
                }

                destinationProperty.SetValue(destination, newDestination, nullArguments);
            }
        }

        private static IEnumerable<PropertyInfo> GetAllPropertiesFromHierarchy(Type sourceType)
        {
            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            var parentType = sourceType.BaseType;
            while (parentType != null)
            {
                if (parentType == typeof(object))
                {
                    break;
                }

                properties.AddRange(parentType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                parentType = parentType.BaseType;
            }

            return properties.Distinct(new PropertyInfoComparer()).Where(x => x.CanWrite);
        }

        private static void AssignGuidValue(object destination, object sourcePropertyValue, PropertyInfo destinationProperty)
        {
            try
            {
                destinationProperty.SetValue(destination, sourcePropertyValue, new object[] { });
            } catch (ArgumentException)
            {
                destinationProperty.SetValue(destination, sourcePropertyValue.ToString(), new object[] { });
            }
        }

        private static object ReflectionCopyArray(Type arrayType, object sourcePropertyValue)
        {
            var sourceArray = sourcePropertyValue as Array;
            if (sourceArray == null)
            {
                return null;
            }

            Type elementType = arrayType.GetElementType();
            Array newDestinationArray = Array.CreateInstance(elementType, sourceArray.Length);
            for (int index = 0; index < sourceArray.Length; index++)
            {
                object newDestination = Activator.CreateInstance(elementType);
                ReflectionCopy(sourceArray.GetValue(index), newDestination);
                newDestinationArray.SetValue(newDestination, index);
            }

            return newDestinationArray;
        }
    }

    /// <summary>
    /// Converts v104 to v105
    /// </summary>
    public static class Adaptor
    {
        public static Persistence.TypeVisualiserLayoutFile Adapt(TypeVisualiserLayoutFile oldDiagram)
        {
            if (oldDiagram == null)
            {
                throw new ArgumentNullResourceException("oldDiagram", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var diagramv105 = new Persistence.TypeVisualiserLayoutFile
                                  {
                                      AssemblyFileName = oldDiagram.AssemblyFileName,
                                      AssemblyFullName = oldDiagram.AssemblyFullName,
                                      FileVersion = oldDiagram.FileVersion,
                                      HideParents = oldDiagram.HideParents,
                                      HideTrivialTypes = oldDiagram.HideTrivialTypes,
                                  };

            var subjectv105 = new Persistence.VisualisableTypeSubjectData();
            CloningTools.ReflectionCopy(oldDiagram.ViewportSaveData.Subject, subjectv105);
            diagramv105.Subject = subjectv105;

            var canvasLayoutv105 = new Persistence.CanvasLayoutData();
            canvasLayoutv105.Types.Add(new Persistence.TypeLayoutData
                                           {
                                               ContentType = typeof(SubjectAssociation).FullName,
                                               Id = oldDiagram.ViewportSaveData.Subject.Id.ToString(),
                                               TopLeft = oldDiagram.ViewportSaveData.CanvasLayout.Subject.TopLeft,
                                               Visible = oldDiagram.ViewportSaveData.CanvasLayout.Subject.Visible,
                                           });
            ConvertLayoutTypes(oldDiagram, canvasLayoutv105);
            ConvertLayoutAnnotations(oldDiagram, canvasLayoutv105);
            diagramv105.CanvasLayout = canvasLayoutv105;

            return diagramv105;
        }

        private static void ConvertLayoutAnnotations(TypeVisualiserLayoutFile oldDiagram, Persistence.CanvasLayoutData canvasLayoutv105)
        {
            foreach (AnnotationData oldAnnotation in oldDiagram.ViewportSaveData.CanvasLayout.Annotations)
            {
                canvasLayoutv105.Types.Add(new Persistence.TypeLayoutData
                                               {
                                                   ContentType = typeof(Persistence.AnnotationData).FullName,
                                                   Id = oldAnnotation.Id.ToString(),
                                                   TopLeft = oldAnnotation.TopLeft,
                                                   Visible = oldAnnotation.Show,
                                                   Data = oldAnnotation.Text,
                                               });
            }
        }

        private static void ConvertLayoutTypes(TypeVisualiserLayoutFile oldDiagram, Persistence.CanvasLayoutData canvasLayoutv105)
        {
            foreach (TypeLayoutData oldType in oldDiagram.ViewportSaveData.CanvasLayout.Types)
            {
                TypeLayoutData copyOfOldType = oldType;
                AssociationData association = oldDiagram.ViewportSaveData.Subject.Associations.FirstOrDefault(x => x.AssociatedTo.Id == copyOfOldType.Id);
                if (association == null)
                {
                    if (oldDiagram.ViewportSaveData.Subject.Implements != null)
                    {
                        association = oldDiagram.ViewportSaveData.Subject.Implements.FirstOrDefault(x => x.AssociatedTo.Id == oldType.Id);
                    }

                    if (association == null && oldDiagram.ViewportSaveData.Subject.Parent != null)
                    {
                        association = oldDiagram.ViewportSaveData.Subject.Parent.AssociatedTo.Id == oldType.Id ? oldDiagram.ViewportSaveData.Subject.Parent : null;
                    }
                }

                if (association == null)
                {
                    throw new NotSupportedException("Association Id " + oldType.Id + " not found in the Subject's data. Cannot deserialise from this data.");
                }

                var newType = new Persistence.TypeLayoutData { Id = oldType.Id.ToString(), TopLeft = oldType.TopLeft, Visible = oldType.Visible, ContentType = association.GetType().FullName, };
                canvasLayoutv105.Types.Add(newType);
            }
        }
    }
}