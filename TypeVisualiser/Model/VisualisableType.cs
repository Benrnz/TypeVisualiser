namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model.Persistence;
    using TypeVisualiser.Properties;
    using TypeVisualiser.Startup;

    using IContainer = StructureMap.IContainer;

    [DebuggerDisplay("VisualisableType {AssemblyQualifiedName}")]
    public class VisualisableType : INotifyPropertyChanged, IVisualisableType
    {
        private static readonly TaskScheduler BackgroundLimitedScheduler = new LimitedConcurrencyLevelTaskScheduler(3);

        // private static readonly TaskScheduler BackgroundLimitedScheduler = null;
        private IContainer doNotUseFactory;

        public VisualisableType(Type type)
            : this(type, new VisualisableTypeData(), SubjectOrAssociate.Associate)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Reviewed here with no consequences")]
        protected VisualisableType(IContainer factory, Type type, VisualisableTypeData data, SubjectOrAssociate subjectOrAssociate)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            this.doNotUseFactory = factory;
            this.NetType = type;
            this.PersistentDataField = data;
            this.SubjectOrAssociate = subjectOrAssociate;
            this.PersistentDataField.Modifiers = new ModifiersData(type);
            this.AssemblyName = type.Assembly.GetName().Name;
            this.AssemblyFullName = type.Assembly.FullName;
            this.AssemblyFileName = type.Assembly.Location;
            var genericNameHelper = new TypeDescriptorHelper(type);
            this.Id = genericNameHelper.GenerateId(); // It is not reliable to use type.GUID different generic parameters do not yeild different guids.
            this.Name = genericNameHelper.IsGeneric ? genericNameHelper.GenerateName() : type.Name;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName ?? string.Format(CultureInfo.InvariantCulture, "{0}, {1}", type.Name, this.AssemblyFullName);
            this.ThisTypeNamespace = type.Namespace;
            if (type.IsEnum)
            {
                this.EnumMemberCount = Enum.GetNames(type).Length;
            }

            this.SetToolTip(type);
            this.SetAssociations(type);
            this.SetLinesOfCodeAndStaticAssociations(type);
        }

        protected VisualisableType(Type type, VisualisableTypeData data, SubjectOrAssociate subjectOrAssociate)
            : this(IoC.Default, type, data, subjectOrAssociate)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AssemblyFileName
        {
            get
            {
                return this.PersistentDataField.AssemblyFileName;
            }

            private set
            {
                this.PersistentDataField.AssemblyFileName = value;
            }
        }

        public string AssemblyFullName
        {
            get
            {
                return this.PersistentDataField.AssemblyFullName;
            }

            private set
            {
                this.PersistentDataField.AssemblyFullName = value;
            }
        }

        public string AssemblyName
        {
            get
            {
                return this.PersistentDataField.AssemblyName;
            }

            private set
            {
                this.PersistentDataField.AssemblyName = value;
            }
        }

        public string AssemblyQualifiedName
        {
            get
            {
                return this.PersistentDataField.FullName;
            }

            private set
            {
                this.PersistentDataField.FullName = value;
            }
        }

        public int ConstructorCount
        {
            get
            {
                return this.PersistentDataField.ConstructorCount;
            }

            private set
            {
                this.PersistentDataField.ConstructorCount = value;
            }
        }

        public int EnumMemberCount
        {
            get
            {
                return this.PersistentDataField.EnumMemberCount;
            }

            private set
            {
                this.PersistentDataField.EnumMemberCount = value;
            }
        }

        public int EventCount
        {
            get
            {
                return this.PersistentDataField.EventCount;
            }

            private set
            {
                this.PersistentDataField.EventCount = value;
            }
        }

        public int FieldCount
        {
            get
            {
                return this.PersistentDataField.FieldCount;
            }

            private set
            {
                this.PersistentDataField.FieldCount = value;
            }
        }

        public string Id
        {
            get
            {
                return this.PersistentDataField.Id;
            }

            private set
            {
                this.PersistentDataField.Id = value;
            }
        }

        public bool IsSubject
        {
            get
            {
                return this.SubjectOrAssociate == SubjectOrAssociate.Subject;
            }
        }

        /// <summary>
        /// Gets or sets the Lines of Code. This is the number of IL Lines of Code not C#.
        /// </summary>
        /// <value>The LinesOfCode.</value>
        public int LinesOfCode
        {
            get
            {
                return this.PersistentDataField.LinesOfCode;
            }

            set
            {
                this.PersistentDataField.LinesOfCode = value;
                this.RaisePropertyChangedEvent("LinesOfCode");
            }
        }

        public string LinesOfCodeToolTip
        {
            get
            {
                return this.PersistentDataField.LinesOfCodeToolTip ?? this.LinesOfCode.ToString("F0", CultureInfo.CurrentCulture);
            }
        }

        public int MethodCount
        {
            get
            {
                return this.PersistentDataField.MethodCount;
            }

            private set
            {
                this.PersistentDataField.MethodCount = value;
            }
        }

        public ModifiersData Modifiers
        {
            get
            {
                return this.PersistentDataField.Modifiers;
            }
        }

        public string Name
        {
            get
            {
                return this.PersistentDataField.Name;
            }

            private set
            {
                this.PersistentDataField.Name = value;
            }
        }

        public string NamespaceQualifiedName
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}.{1}", this.ThisTypeNamespace, this.Name);
            }
        }

        public int PropertyCount
        {
            get
            {
                return this.PersistentDataField.PropertyCount;
            }

            private set
            {
                this.PersistentDataField.PropertyCount = value;
            }
        }

        public SubjectOrAssociate SubjectOrAssociate
        {
            get
            {
                return this.PersistentDataField.SubjectOrAssociate;
            }

            protected set
            {
                this.PersistentDataField.SubjectOrAssociate = value;
            }
        }

        public string ThisTypeNamespace
        {
            get
            {
                return this.PersistentDataField.Namespace;
            }

            private set
            {
                this.PersistentDataField.Namespace = value;
            }
        }

        public string TypeToolTip
        {
            get
            {
                return this.PersistentDataField.ToolTipName;
            }

            private set
            {
                this.PersistentDataField.ToolTipName = value;
            }
        }

        protected IContainer Factory
        {
            get
            {
                return this.doNotUseFactory ?? (this.doNotUseFactory = IoC.Default);
            }
        }

        protected Task LinesOfCodeTask { get; private set; }

        protected Type NetType { get; private set; }

        /// <summary>
        /// Gets the persistent data. This method is not public because I do not want WPF binding to consume it directly. It doesn't implement INotifyPropertyChanged.
        /// </summary>
        /// <value>The persistent data.</value>
        protected VisualisableTypeData PersistentDataField { get; private set; }

        public static bool operator ==(VisualisableType operand1, VisualisableType operand2)
        {
            object boxed1 = operand1;
            object boxed2 = operand2;

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(boxed1, boxed2))
            {
                return true;
            }

            if (boxed1 == null || boxed2 == null)
            {
                return false;
            }

            return operand1.AssemblyQualifiedName.Equals(operand2.AssemblyQualifiedName);
        }

        public static bool operator !=(VisualisableType operand1, VisualisableType operand2)
        {
            return !(operand1 == operand2);
        }

        public override bool Equals(object obj)
        {
            var otherType = obj as VisualisableType;
            if (otherType == null)
            {
                return false;
            }

            return this.AssemblyQualifiedName.Equals(otherType.AssemblyQualifiedName);
        }

        /// <summary>
        /// Gets the persistent data. This method is separate to the protected property because I do not want WPF binding to consume it directly. 
        /// It doesn't implement INotifyPropertyChanged.       
        /// This method needs to be called to get the persistent data object for saving to disk. This gives the objects a change to ensure all
        /// data is up to date and stored in the persistent objects correctly.
        /// </summary>
        /// <returns>
        /// The <see cref="VisualisableTypeData"/>.
        /// </returns>
        public virtual VisualisableTypeData ExtractPersistentData()
        {
            return this.PersistentDataField;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Sets the consumes collection. Intended to be used by <see cref="VisualisableTypeWithAssociations"/>. Must be on this base class to be polymorphic.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="reader">
        /// The il reader.
        /// </param>
        protected virtual void SetConsumes(MethodBase method, IMethodBodyReader reader)
        {
            if (method == null)
            {
                throw new ArgumentNullResourceException("method", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (reader == null)
            {
                throw new ArgumentNullResourceException("reader", Resources.General_Given_Parameter_Cannot_Be_Null);
            }
        }

        protected virtual void SetStaticAssociations(MethodBase consumedStatics, IMethodBodyReader reader)
        {
            if (consumedStatics == null)
            {
                throw new ArgumentNullResourceException("consumedStatics", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (reader == null)
            {
                throw new ArgumentNullResourceException("reader", Resources.General_Given_Parameter_Cannot_Be_Null);
            }
        }

        private static string GetAccessor(Type type)
        {
            if (type.IsPublic)
            {
                return "Public";
            }

            if (type.IsNestedPrivate)
            {
                return "Private Nested";
            }

            return "Internal";
        }

        private int CalculateLoc(IEnumerable<MethodBase> methods)
        {
            int locCalc = 0;
            Logger.Instance.WriteEntry("{0} Begin Calculate LOC", DateTime.Now);
            Stopwatch stopWatch = Stopwatch.StartNew();
            foreach (MethodBase method in methods)
            {
                IMethodBodyReader methodBodyReader = this.CreateMethodBodyReader(method);
                this.SetConsumes(method, methodBodyReader);
                this.SetStaticAssociations(method, methodBodyReader);
                locCalc += methodBodyReader.Instructions.Count();
            }

            Logger.Instance.WriteEntry("{0} Finished Calculate LOC {1}", DateTime.Now, stopWatch.ElapsedMilliseconds);
            return locCalc;
        }

        private void CalculateLocAndConsumptionAssociations(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Logger.Instance.WriteEntry("{0} SetLinesOfCodeAndStaticAssociations(type) - invoking CalculateLoc(methods)", DateTime.Now);
            int locCalc = this.CalculateLoc(methods);
            if (locCalc != -1)
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                Logger.Instance.WriteEntry("{0} SetLinesOfCodeAndStaticAssociations(type) - invoking CalculateLoc(constructors)", DateTime.Now);
                locCalc += this.CalculateLoc(constructors);
            }

            this.LinesOfCode = locCalc;
        }

        private IMethodBodyReader CreateMethodBodyReader(MethodBase method)
        {
            IMethodBodyReader reader = this.Factory.TryGetInstance<IMethodBodyReader>() ?? new MethodBodyReader();

            reader.Read(method);
            return reader;
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SetAssociations(Type type)
        {
            this.PropertyCount = (from p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) select p).Count();

            var excludeMethodList = new[] { "ToString", "GetHashCode", "GetType", "Equals", "get_", "set_" };
            if (type.IsClass || type.IsInterface || type.IsValueType)
            {
                this.MethodCount =
                    (from m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     where !excludeMethodList.Contains(m.Name) && !m.IsSpecialName
                     select m).Count();
            }
            else
            {
                this.MethodCount = 0;
            }

            this.ConstructorCount = (from m in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) select m).Count();

            this.EventCount = (from e in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) select e).Count();

            IEnumerable<FieldInfo> fields = from f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                                             where !(f.Name.Contains("__BackingField") || typeof(EventHandler).IsAssignableFrom(f.FieldType))
                                             select f;
            this.FieldCount = fields.Count();
        }

        private void SetLinesOfCodeAndStaticAssociations(Type type)
        {
            if (this.IsSubject)
            {
                // Subject needs to know about consume and static associations up front so they can be drawn into the diagram.
                this.CalculateLocAndConsumptionAssociations(type);
                return;
            }

            // Non-subject types on diagram (Associations of the subject) only calculate LOC on a background thread, but check the cache first.
            if (BackgroundLimitedScheduler != null)
            {
                this.LinesOfCodeTask = Task.Factory.StartNew(
                    () => this.CalculateLocAndConsumptionAssociations(type), CancellationToken.None, TaskCreationOptions.LongRunning, BackgroundLimitedScheduler);
            }
            else
            {
                this.LinesOfCodeTask = Task.Factory.StartNew(() => this.CalculateLocAndConsumptionAssociations(type), TaskCreationOptions.LongRunning);
            }
        }

        private void SetToolTip(Type type)
        {
            if (type.IsClass)
            {
                this.TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} Class: {1}", GetAccessor(type), this.Name);
                return;
            }

            if (type.IsEnum)
            {
                this.TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} Enumeration: {1}", GetAccessor(type), this.Name);
                return;
            }

            if (type.IsValueType)
            {
                this.TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} ValueType: {1}", GetAccessor(type), this.Name);
                return;
            }

            if (type.IsInterface)
            {
                this.TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} Interface: {1}", GetAccessor(type), this.Name);
            }
        }
    }
}