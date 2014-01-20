namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using StructureMap;

    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.Properties;

    public class VisualisableTypeWithAssociations : VisualisableType, IVisualisableTypeWithAssociations
    {
        private const int DepthLimit = 2;

        /// <summary>
        /// A temporary cache for storing unrefined data during consumption discovery.
        /// Tuple.Item1 = used by method name, Item2 = the type associated to
        /// </summary>
        private readonly List<Tuple<string, Type>> allConsumptionUsage = new List<Tuple<string, Type>>();

        /// <summary>
        /// A temporary cache for storing unrefined data during consumption discovery.
        /// Tuple.Item1 = used by method name, Item2 = the type associated to
        /// </summary>
        private readonly List<Tuple<string, Type>> allStaticUsage = new List<Tuple<string, Type>>();

        private readonly object associationsSyncRoot = new object();

        private List<FieldAssociation> associations = new List<FieldAssociation>();

        /// <summary>
        /// Depth of the diagram, the number of hops to the main subject of the diagram. Used to prevent walking the entire dot net framework.
        /// </summary>
        private int depth;

        private Func<ITrivialFilter> getTrivialFilter;

        private AssociationLoadStatus loadStatus = AssociationLoadStatus.ConstructedOnly;

        public VisualisableTypeWithAssociations(Type type)
            : this(type, 0)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Reviewed, ok here")]
        public VisualisableTypeWithAssociations(Type type, int depth)
            : base(type, new VisualisableTypeSubjectData(), depth == 0 ? SubjectOrAssociate.Subject : SubjectOrAssociate.Associate)
        {
            this.ConstructorSharedLogic(type, depth);
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Reviewed, ok here")]
        public VisualisableTypeWithAssociations(Type type, int depth, IContainer factory)
            : base(factory, type, new VisualisableTypeSubjectData(), depth == 0 ? SubjectOrAssociate.Subject : SubjectOrAssociate.Associate)
        {
            this.ConstructorSharedLogic(type, depth);
        }

        /// <summary>
        /// Gets all associations. This is an unfiltered list of all associations. It does not take into account the 
        /// <see cref="TrivialFilter"/>. IT ALSO DOES NOT INCLUDE THE SUPERCLASS OR INTERFACES.
        /// </summary>
        /// <value>All associations.</value>
        public IEnumerable<FieldAssociation> AllAssociations
        {
            get
            {
                return this.associations;
            }
        }

        /// <summary>
        /// Gets the consumes associations. This is a subset of <see cref="FilteredAssociations"/>. 
        /// This collection takes into account the <see cref="TrivialFilter"/>.
        /// This collection includes the sub-classes of <see cref="ConsumeAssociation"/>, such as
        /// <see cref="StaticAssociation"/>.
        /// </summary>
        /// <value>The consume associations.</value>
        public IEnumerable<ConsumeAssociation> Consumes
        {
            get
            {
                ITrivialFilter trivialFilter = this.getTrivialFilter();
                if (trivialFilter.HideTrivialTypes)
                {
                    return this.associations.OfType<ConsumeAssociation>().Where(x => !trivialFilter.IsTrivialType(x.AssociatedTo.NamespaceQualifiedName));
                }

                return this.associations.OfType<ConsumeAssociation>();
            }
        }

        /// <summary>
        /// Gets just the field associations. This is a subset of <see cref="FilteredAssociations"/>. 
        /// This collection takes into account the <see cref="TrivialFilter"/>.
        /// This collection does NOT include the sub-classes of <see cref="FieldAssociation"/>.
        /// </summary>
        /// <value>The field associations.</value>
        public IEnumerable<FieldAssociation> Fields
        {
            get
            {
                ITrivialFilter trivialFilter = this.getTrivialFilter();
                if (trivialFilter.HideTrivialTypes)
                {
                    return this.associations.Where(x => x.GetType() == typeof(FieldAssociation) && !trivialFilter.IsTrivialType(x.AssociatedTo.NamespaceQualifiedName));
                }

                return this.associations.Where(x => x.GetType() == typeof(FieldAssociation));
            }
        }

        /// <summary>
        /// Gets all associations while taking into account the <see cref="TrivialFilter"/>.
        /// This collection includes the sub-classes of <see cref="FieldAssociation"/>, such as
        /// <see cref="ConsumeAssociation"/> and others.
        /// </summary>
        /// <value>All associations.</value>
        public IEnumerable<FieldAssociation> FilteredAssociations
        {
            get
            {
                ITrivialFilter trivialFilter = this.getTrivialFilter();
                if (trivialFilter.HideTrivialTypes)
                {
                    return this.associations.Where(x => !trivialFilter.IsTrivialType(x.AssociatedTo.NamespaceQualifiedName));
                }

                return this.associations;
            }
        }

        /// <summary>
        /// Gets the number of nontrivial dependencies. This is the number of associations that are
        /// classified as non-trivial by the <see cref="TrivialFilter"/> and associates that are interfaces
        /// enumerations or value types.
        /// </summary>
        /// <value>The nontrivial dependencies.</value>
        public int NontrivialDependencies
        {
            get
            {
                IEnumerable<FieldAssociation> nontrivialDependencies = this.associations.Where(x => !x.IsTrivialAssociation());
                return nontrivialDependencies.Count() + (this.Parent == null ? 0 : this.Parent.IsTrivialAssociation() ? 0 : 1);
            }
        }

        /// <summary>
        /// Gets the parent of this subject.
        /// </summary>
        /// <value>The parent.</value>
        public ParentAssociation Parent { get; private set; }

        /// <summary>
        /// Gets all the interfaces this subject implements. This list is not affected by
        /// the <see cref="TrivialFilter"/>.
        /// </summary>
        /// <value>The interfaces this subject implements.</value>
        public IEnumerable<ParentAssociation> ThisTypeImplements { get; private set; }

        /// <summary>
        /// Gets the number of nontrivial dependencies. This is the number of associations that are
        /// classified as trivial by the <see cref="TrivialFilter"/> and associates that are interfaces
        /// enumerations or value types.
        /// </summary>
        /// <value>The nontrivial dependencies.</value>
        public int TrivialDependencies
        {
            get
            {
                IEnumerable<FieldAssociation> result = this.associations.Where(x => x.IsTrivialAssociation());
                return result.Count() + this.ThisTypeImplements.Count() + (this.Parent == null ? 0 : this.Parent.IsTrivialAssociation() ? 1 : 0);
            }
        }

        /// <summary>
        /// Discovers the relationships between associations. This is an interim step to find only associations that are not already modeled.
        /// This should only be called on the main subject of the diagram. Call this for multiple types will result in unnecessary duplicated processing.
        /// For example: Subject will be excluded and its associations also.
        /// </summary>
        public void DiscoverSecondaryAssociationsInModel()
        {
            // Now examine all assoications and see if any of them relate to each other. (Up to this point only relationships back to this subject have been created).
            List<IVisualisableType> allAssociations =
                this.associations.Cast<Association>().Union(this.ThisTypeImplements).Union(new[] { this.Parent }).Where(x => x != null).Select(x => x.AssociatedTo).ToList();

            foreach (IVisualisableType visualisableType in allAssociations.ToList())
            {
                var fullyExpandedTypeModel = visualisableType as VisualisableTypeWithAssociations;
                if (fullyExpandedTypeModel == null)
                {
                    throw new InvalidOperationException("Code Error: Must be of type Visualisable Type Subject");
                }

                // Discover relationships from current loop variable to others fields
                IEnumerable<FieldInfo> fields = this.GetAllFieldsInThisType(fullyExpandedTypeModel.NetType);

                // Resulting list should exclude the main subject, its associations have already been drawn.
                IVisualisableType copyOfVisualisableType = visualisableType;

                // Not equal to current loop variable
                IEnumerable<FieldInfo> includeTheseFields = fields.Where(f => new TypeDescriptorHelper(f.FieldType).GenerateId() != copyOfVisualisableType.Id).Join(
                    allAssociations, fieldInfo => new TypeDescriptorHelper(fieldInfo.FieldType).GenerateId(), association => association.Id, (fieldInfo, association) => fieldInfo);

                // Do I have the associated field on my diagram already? If so include otherwise omit.
                fullyExpandedTypeModel.AddToAssociationsCollection(this.InitialiseFieldAssociations(includeTheseFields));
            }
        }

        public override VisualisableTypeData ExtractPersistentData()
        {
            return new VisualisableTypeWithAssociationsDataAdaptor(this.AllAssociations, this.ThisTypeImplements, this.Parent, this.PersistentDataField).Adapt();
        }

        /// <summary>
        /// This method is called when an instance is reused from the cache. 
        /// </summary>
        /// <param name="newDepth">
        /// The new Depth.
        /// </param>
        public void InitialiseForReuseFromCache(int newDepth)
        {
            if (newDepth > 0)
            {
                return;
            }

            this.depth = 0;
            if (this.loadStatus == AssociationLoadStatus.FullyLoaded)
            {
                return;
            }

            this.Initialise(this.NetType);
        }

        /// <summary>
        /// This virtual is to allow isolation of the parent relationship.
        /// </summary>
        /// <param name="mainSubjectType">
        /// The main Subject Type.
        /// </param>
        protected virtual void InitialiseParentTypeRelationship(Type mainSubjectType)
        {
            if (mainSubjectType == null)
            {
                throw new ArgumentNullResourceException(Resources.General_Given_Parameter_Cannot_Be_Null, "mainSubjectType");
            }

            if (mainSubjectType.BaseType != null && mainSubjectType.BaseType != typeof(object))
            {
                this.Parent = this.Factory.GetInstance<ParentAssociation>().Initialise(mainSubjectType.BaseType, this);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated in base")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated in base")]
        protected override void SetConsumes(MethodBase method, IMethodBodyReader reader)
        {
            base.SetConsumes(method, reader);
            foreach (ILInstruction instruction in reader.Instructions.Where(i => i.Operand != null))
            {
                var externalMethodCall = instruction.Operand as MethodBase;
                if (externalMethodCall != null)
                {
                    Type declaringType = externalMethodCall.DeclaringType;
                    if ((externalMethodCall.Attributes & MethodAttributes.Static) == MethodAttributes.Static)
                    {
                        // Statics discovered elsewhere
                        continue;
                    }

                    if (method.DeclaringType != null)
                    {
                        if (method.IsConstructor && (declaringType == method.DeclaringType || declaringType == method.DeclaringType.BaseType))
                        {
                            // Calling another constructor in same class.
                            continue;
                        }
                    }

                    if (declaringType == null || TypeDescriptorHelper.AreEqual(declaringType, this.Id))
                    {
                        // Don't bother counting "self-consumption" calls.
                        continue;
                    }

                    this.allConsumptionUsage.Add(new Tuple<string, Type>(method.Name, declaringType));
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated in base")]
        protected override void SetStaticAssociations(MethodBase consumedStatics, IMethodBodyReader reader)
        {
            base.SetStaticAssociations(consumedStatics, reader);
            IQueryable<Tuple<string, Type>> statics = from staticMethod in reader.Instructions.Select(instruction => instruction.Operand).OfType<MethodInfo>()
                                                      where (staticMethod.Attributes & MethodAttributes.Static) == MethodAttributes.Static
                                                      select new Tuple<string, Type>(consumedStatics.Name, staticMethod.DeclaringType);
            this.allStaticUsage.AddRange(statics);
        }

        private static void MergeConsumeCollectionIntoAssociations(List<FieldAssociation> existingAssociations, IEnumerable<FieldAssociation> consumeCollection)
        {
            List<FieldAssociation> consumeList = consumeCollection.ToList();

            // Find intersection between consumes collection and associations. What do we already have in the main association collection.
            FieldAssociation[] overlapCollection = existingAssociations.Join(consumeList, a => a.AssociatedTo.AssemblyQualifiedName, c => c.AssociatedTo.AssemblyQualifiedName, (a, c) => c).ToArray();

            IEnumerable<string> duplicates = existingAssociations.GroupBy(i => i.AssociatedTo.AssemblyQualifiedName).Where(g => g.Count() > 1).Select(g => g.Key);
            if (duplicates.Any())
            {
                throw new DuplicateNameException("Code bug: Association list contains duplicates.");
            }

            // Remove consumes relationship if also is a strong association already.
            foreach (FieldAssociation associationAndConsumerOf in overlapCollection)
            {
                consumeList.Remove(associationAndConsumerOf);
                FieldAssociation trumpAssociation = existingAssociations.Single(x => x.AssociatedTo.AssemblyQualifiedName == associationAndConsumerOf.AssociatedTo.AssemblyQualifiedName);
                if (trumpAssociation != null)
                {
                    trumpAssociation.Merge(associationAndConsumerOf);
                }
            }

            List<FieldAssociation> exceptList = consumeList.Except(overlapCollection).ToList();
            existingAssociations.AddRange(exceptList);
        }

        /// <summary>
        /// Adding to the associations collection must be done using this method. There are two sources to add to the associations, <see cref="Initialise"/>
        /// and <see cref="DiscoverSecondaryAssociationsInModel"/>.
        /// </summary>
        /// <param name="fields">
        /// The fields to be added to the <see cref="associations"/> collection.
        /// </param>
        private void AddToAssociationsCollection(IEnumerable<FieldAssociation> fields)
        {
            lock (this.associationsSyncRoot)
            {
                IEnumerable<FieldAssociation> duplicateFiltered = this.DuplicateCheck(fields.ToList());
                this.associations.AddRange(duplicateFiltered);
            }
        }

        private void AggregateRawConsumeCollection(
            IList<ConsumeAssociation> rawConsumeList, Func<Type, int, IEnumerable<AssociationUsageDescriptor>, ConsumeAssociation> ctor, IList<Tuple<string, Type>> allUsageList)
        {
            var usageGroup = new Tuple<string, Type>(null, null);
            var callerMethodList = new List<AssociationUsageDescriptor>();
            int count = 0;
            if (allUsageList.Any(x => x.Item2 == null))
            {
                // todo sometimes getting null ref here on x.Item2 == null
                // I suspect these might be win32 C++ types that do not have a correspnding type. These need to be specially dealt with.
                // Debugger.Break();
                allUsageList = allUsageList.Where(x => x.Item2 != null).ToList();
            }

            foreach (var usage in allUsageList.OrderBy(x => x.Item2.FullName))
            {
                if (new TypeDescriptorHelper(usage.Item2).GenerateId() == this.Id)
                {
                    // filter out associations to itself.
                    continue;
                }

                if (usageGroup.Item2 != usage.Item2)
                {
                    if (usageGroup.Item2 != null)
                    {
                        rawConsumeList.Add(ctor(usageGroup.Item2, count, callerMethodList));
                    }

                    usageGroup = usage;
                    count = 0;
                    callerMethodList = new List<AssociationUsageDescriptor>();
                }

                count++;
                callerMethodList.Add(AssociationUsageDescriptor.CreateMethodUsage(usage.Item1));
            }

            if (callerMethodList.Any())
            {
                rawConsumeList.Add(ctor(usageGroup.Item2, count, callerMethodList));
            }
        }

        private void ConstructorSharedLogic(Type type, int depthFromSubject)
        {
            this.getTrivialFilter = this.GetTrivialFilterImplementation;
            this.depth = depthFromSubject;
            this.ThisTypeImplements = new List<ParentAssociation>();
            if (this.LinesOfCodeTask != null)
            {
                if (this.LinesOfCodeTask.IsCompleted)
                {
                    this.Initialise(type);
                }
                else
                {
                    this.LinesOfCodeTask.ContinueWith(t => this.Initialise(type));
                }
            }
            else
            {
                this.Initialise(type);
            }
        }

        private IEnumerable<FieldAssociation> DuplicateCheck(IList<FieldAssociation> addRange)
        {
            if (!addRange.Any() || !this.associations.Any())
            {
                return addRange;
            }

            IEnumerable<FieldAssociation> d = addRange.Except(this.associations.Intersect(addRange));
            return d;
        }

        private IEnumerable<FieldInfo> GetAllFieldsInThisType(Type type)
        {
            if (type == null)
            {
                throw new InvalidOperationException("Code Error: type should never be null here.");
            }

            IOrderedEnumerable<FieldInfo> allFields = from field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                                      where !TypeDescriptorHelper.AreEqual(field.FieldType, this.Id)
                                                      // Do not want to include "this" type in list
                                                      orderby field.FieldType.FullName
                                                      select field;
            return allFields;
        }

        private ITrivialFilter GetTrivialFilterImplementation()
        {
            return this.Factory.TryGetInstance<ITrivialFilter>();
        }

        private void Initialise(Type type)
        {
            if (this.loadStatus == AssociationLoadStatus.FullyLoaded)
            {
                return;
            }

            if (this.depth >= DepthLimit)
            {
                this.loadStatus = AssociationLoadStatus.NotLoaded;
                return;
            }

            this.loadStatus = AssociationLoadStatus.FullyLoaded;

            this.InitialiseParentTypeRelationship(type);

            IEnumerable<FieldAssociation> fields = this.InitialiseFieldAssociations(this.GetAllFieldsInThisType(type));
            this.AddToAssociationsCollection(fields);

            // Consumes
            this.InitialiseConsumesCollection();

            // Implements
            List<Type> allInterfaces = type.GetInterfaces().ToList();
            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                allInterfaces.Add(type.BaseType);
            }

            IEnumerable<Type> interfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).Where(t => t.IsInterface);
            this.ThisTypeImplements = interfaces.Select(i => this.Factory.GetInstance<ParentAssociation>().Initialise(i, this));

            // Put all associations excluding parents into a list.
            this.associations = this.associations.OrderBy(x => x.AssociatedTo.Name).ToList();
        }

        private void InitialiseConsumesCollection()
        {
            // Field Consumes
            var rawConsumeList = new List<ConsumeAssociation>();
            var ctor = new Func<Type, int, IEnumerable<AssociationUsageDescriptor>, ConsumeAssociation>((t, i, l) => this.Factory.GetInstance<ConsumeAssociation>().Initialise(t, i, l, this.depth + 1));
            this.AggregateRawConsumeCollection(rawConsumeList, ctor, this.allConsumptionUsage);
            MergeConsumeCollectionIntoAssociations(this.associations, rawConsumeList);

            // Static Consumes
            rawConsumeList.Clear();
            ctor = (t, i, l) => this.Factory.GetInstance<StaticAssociation>().Initialise(t, i, l, this.depth + 1);
            this.AggregateRawConsumeCollection(rawConsumeList, ctor, this.allStaticUsage);
            MergeConsumeCollectionIntoAssociations(this.associations, rawConsumeList);
        }

        private IEnumerable<FieldAssociation> InitialiseFieldAssociations(IEnumerable<FieldInfo> fields)
        {
            int fieldUsageCount = 0;
            Type groupType = null;
            var fieldNameList = new List<AssociationUsageDescriptor>();
            var fieldAssociations = new List<FieldAssociation>();
            foreach (FieldInfo field in fields)
            {
                if (groupType == field.FieldType)
                {
                    fieldUsageCount++;
                    fieldNameList.Add(AssociationUsageDescriptor.CreateFieldUsage(field.Name));
                }
                else
                {
                    if (fieldUsageCount > 0)
                    {
                        fieldAssociations.Add(this.Factory.GetInstance<FieldAssociation>().Initialise(groupType, fieldUsageCount, fieldNameList, this.depth + 1));
                    }

                    fieldUsageCount = 1;
                    fieldNameList = new List<AssociationUsageDescriptor> { AssociationUsageDescriptor.CreateFieldUsage(field.Name) };
                    groupType = field.FieldType;
                }
            }

            if (fieldUsageCount > 0)
            {
                fieldAssociations.Add(this.Factory.GetInstance<FieldAssociation>().Initialise(groupType, fieldUsageCount, fieldNameList, this.depth + 1));
            }

            return fieldAssociations;
        }
    }
}