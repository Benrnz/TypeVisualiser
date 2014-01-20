namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TypeVisualiser.Properties;

    /// <summary>
    /// Creates a serializable object model of the given <see cref="VisualisableTypeWithAssociations"/>.
    /// Main use case is to invoke the static member <see cref="ExtractPersistentData"/> from a UI Controller. This will manage the clean up of caches releasing their memory.
    /// </summary>
    internal class VisualisableTypeWithAssociationsDataAdaptor
    {
        /// <summary>
        /// As <see cref="FieldAssociationData"/> objects are created from the <see cref="FieldAssociation"/> objects they are cached in this dictionary by their 
        /// <see cref="VisualisableType.Id"/>. This is to prevent the same type being converted multiple times and to prevent circular dependency loops 
        /// (where a string has an integer field and an integer has a string field).
        /// </summary>
        private static readonly Dictionary<string, VisualisableTypeDataContainer> FieldDataCache = new Dictionary<string, VisualisableTypeDataContainer>();

        private readonly IEnumerable<FieldAssociation> allAssociations;

        private readonly ParentAssociation parent;

        private readonly VisualisableTypeData persistentDataField;

        private readonly IEnumerable<ParentAssociation> thisTypeImplements;

        public VisualisableTypeWithAssociationsDataAdaptor(
            IEnumerable<FieldAssociation> allAssociations, IEnumerable<ParentAssociation> thisTypeImplements, ParentAssociation parent, VisualisableTypeData persistentDataField)
        {
            this.parent = parent;
            this.persistentDataField = persistentDataField;
            this.thisTypeImplements = thisTypeImplements;
            this.allAssociations = allAssociations;
        }

        public static VisualisableTypeSubjectData ExtractPersistentData(IVisualisableTypeWithAssociations subject)
        {
            var returnValue = (VisualisableTypeSubjectData)subject.ExtractPersistentData();
            CleanUp();
            return returnValue;
        }

        public VisualisableTypeSubjectData Adapt()
        {
            // pull data in from association objects.
            var subjectData = this.persistentDataField as VisualisableTypeSubjectData;
            if (subjectData == null)
            {
                throw new InvalidCastException(Resources.VisualisableTypeSubject_GetPersistentData_The_underlying_data_object_for_this_subject_is_not_of_Visualisable_Type_Subject_Data_class);
            }

            subjectData.Associations = this.allAssociations.Select(ConvertAndCache).ToArray();

            subjectData.Implements = this.thisTypeImplements.Select(ConvertAndCache).ToArray();

            if (this.parent != null)
            {
                ParentAssociationData parentAssociationData = ConvertAndCache(this.parent);
                subjectData.Parent = parentAssociationData;
            }

            return subjectData;
        }

        private static void CleanUp()
        {
            FieldDataCache.Clear();
        }

        /// <summary>
        /// Dehydrates a runtime object model <see cref="FieldAssociation"/> to a persistence object.
        /// </summary>
        /// <param name="association">
        /// The association.
        /// </param>
        /// <returns>
        /// The <see cref="FieldAssociationData"/>.
        /// </returns>
        private static FieldAssociationData ConvertAndCache(FieldAssociation association)
        {
            if (association == null)
            {
                throw new ArgumentNullResourceException("association", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            FieldAssociationData converted;
            if (association is StaticAssociation)
            {
                converted = new StaticAssociationData();
            }
            else if (association is ConsumeAssociation)
            {
                converted = new ConsumeAssociationData();
            }
            else
            {
                converted = new FieldAssociationData();
            }

            converted.Name = association.Name;
            VisualisableTypeDataContainer typeData;
            if (!FieldDataCache.TryGetValue(association.AssociatedTo.Id, out typeData))
            {
                typeData = new VisualisableTypeDataContainer();
                FieldDataCache.Add(association.AssociatedTo.Id, typeData); // Stack Overflow potential, be sure to cache converted types to ensure no circular references.
                typeData.Data = association.AssociatedTo.ExtractPersistentData();
            }

            if (typeData.Data == null)
            {
                // Should be free from memory leaks being a private class
                typeData.DataReady += (s, e) => converted.AssociatedTo = typeData.Data;
            }

            converted.AssociatedTo = typeData.Data;
            converted.UsageCount = association.UsageCount;
            return converted;
        }

        private static ParentAssociationData ConvertAndCache(ParentAssociation parentAssociation)
        {
            if (parentAssociation == null)
            {
                throw new ArgumentNullResourceException("parentAssociation", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            VisualisableTypeDataContainer typeData;
            if (!FieldDataCache.TryGetValue(parentAssociation.AssociatedTo.Id, out typeData))
            {
                typeData = new VisualisableTypeDataContainer();
                FieldDataCache.Add(parentAssociation.AssociatedTo.Id, typeData);
                typeData.Data = parentAssociation.AssociatedTo.ExtractPersistentData(); // Stack Overflow potential, be sure to cache converted types to ensure no circular references.
            }

            var parentData = new ParentAssociationData { Name = parentAssociation.Name, AssociatedTo = typeData.Data };

            if (typeData.Data == null)
            {
                // Should be free from memory leaks being a private class
                typeData.DataReady += (s, e) => parentData.AssociatedTo = typeData.Data;
            }

            // This must be stored in the cache before ExtractPersistentData is called, which is a recursive call and could cause infinite recursion.
            return parentData;
        }

        private class VisualisableTypeDataContainer
        {
            private VisualisableTypeData data;

            public event EventHandler DataReady;

            public VisualisableTypeData Data
            {
                get
                {
                    return this.data;
                }

                set
                {
                    this.data = value;
                    EventHandler handler = this.DataReady;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}