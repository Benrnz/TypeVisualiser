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

namespace TypeVisualiser.Model
{
    [DebuggerDisplay("VisualisableType {AssemblyQualifiedName}")]
    public class VisualisableType : INotifyPropertyChanged, IVisualisableType
    {
        private static readonly TaskScheduler BackgroundLimitedScheduler = new LimitedConcurrencyLevelTaskScheduler(3);
        //private static readonly TaskScheduler BackgroundLimitedScheduler = null;

        private IContainer doNotUseFactory;

        public VisualisableType(Type type) : this(type, new VisualisableTypeData(), SubjectOrAssociate.Associate)
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
            NetType = type;
            PersistentDataField = data;
            SubjectOrAssociate = subjectOrAssociate;
            PersistentDataField.Modifiers = new ModifiersData(type);
            AssemblyName = type.Assembly.GetName().Name;
            AssemblyFullName = type.Assembly.FullName;
            AssemblyFileName = type.Assembly.Location;
            var genericNameHelper = new TypeDescriptorHelper(type);
            Id = genericNameHelper.GenerateId(); // It is not reliable to use type.GUID different generic parameters do not yeild different guids.
            Name = genericNameHelper.IsGeneric ? genericNameHelper.GenerateName() : type.Name;
            AssemblyQualifiedName = type.AssemblyQualifiedName ?? string.Format(CultureInfo.InvariantCulture, "{0}, {1}", type.Name, AssemblyFullName);
            ThisTypeNamespace = type.Namespace;
            if (type.IsEnum)
            {
                EnumMemberCount = Enum.GetNames(type).Length;
            }

            SetToolTip(type);
            SetAssociations(type);
            SetLinesOfCodeAndStaticAssociations(type);
        }

        protected VisualisableType(Type type, VisualisableTypeData data, SubjectOrAssociate subjectOrAssociate)
            : this(IoC.Default, type, data, subjectOrAssociate)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AssemblyFileName
        {
            get { return PersistentDataField.AssemblyFileName; }

            private set { PersistentDataField.AssemblyFileName = value; }
        }

        public string AssemblyFullName
        {
            get { return PersistentDataField.AssemblyFullName; }

            private set { PersistentDataField.AssemblyFullName = value; }
        }

        public string AssemblyName
        {
            get { return PersistentDataField.AssemblyName; }

            private set { PersistentDataField.AssemblyName = value; }
        }

        public string AssemblyQualifiedName
        {
            get { return PersistentDataField.FullName; }

            private set { PersistentDataField.FullName = value; }
        }

        public int ConstructorCount
        {
            get { return PersistentDataField.ConstructorCount; }

            private set { PersistentDataField.ConstructorCount = value; }
        }

        public int EnumMemberCount
        {
            get { return PersistentDataField.EnumMemberCount; }

            private set { PersistentDataField.EnumMemberCount = value; }
        }

        public int EventCount
        {
            get { return PersistentDataField.EventCount; }

            private set { PersistentDataField.EventCount = value; }
        }

        public int FieldCount
        {
            get { return PersistentDataField.FieldCount; }

            private set { PersistentDataField.FieldCount = value; }
        }

        public string Id
        {
            get { return PersistentDataField.Id; }

            private set { PersistentDataField.Id = value; }
        }

        public bool IsSubject
        {
            get { return SubjectOrAssociate == SubjectOrAssociate.Subject; }
        }

        /// <summary>
        /// Gets or sets the Lines of Code. This is the number of IL Lines of Code not C#.
        /// </summary>
        /// <value>The LinesOfCode.</value>
        public int LinesOfCode
        {
            get { return PersistentDataField.LinesOfCode; }

            set
            {
                PersistentDataField.LinesOfCode = value;
                RaisePropertyChangedEvent("LinesOfCode");
            }
        }

        public string LinesOfCodeToolTip
        {
            get { return PersistentDataField.LinesOfCodeToolTip ?? LinesOfCode.ToString("F0", CultureInfo.CurrentCulture); }
        }

        public int MethodCount
        {
            get { return PersistentDataField.MethodCount; }

            private set { PersistentDataField.MethodCount = value; }
        }

        public ModifiersData Modifiers
        {
            get { return PersistentDataField.Modifiers; }
        }

        public string Name
        {
            get { return PersistentDataField.Name; }

            private set { PersistentDataField.Name = value; }
        }

        public string NamespaceQualifiedName
        {
            get { return string.Format(CultureInfo.CurrentCulture, "{0}.{1}", ThisTypeNamespace, Name); }
        }

        public int PropertyCount
        {
            get { return PersistentDataField.PropertyCount; }

            private set { PersistentDataField.PropertyCount = value; }
        }

        public SubjectOrAssociate SubjectOrAssociate
        {
            get { return PersistentDataField.SubjectOrAssociate; }

            protected set { PersistentDataField.SubjectOrAssociate = value; }
        }

        public string ThisTypeNamespace
        {
            get { return PersistentDataField.Namespace; }

            private set { PersistentDataField.Namespace = value; }
        }

        public string TypeToolTip
        {
            get { return PersistentDataField.ToolTipName; }

            private set { PersistentDataField.ToolTipName = value; }
        }

        protected IContainer Factory
        {
            get { return this.doNotUseFactory ?? (this.doNotUseFactory = IoC.Default); }
        }

        protected Task LinesOfCodeTask { get; private set; }

        protected Type NetType { get; private set; }

        /// <summary>
        /// Gets the persistent data. This method is not public because I do not want WPF binding to consume it directly. It doesnt impl INotifyPropertyChanged.
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

        /// <summary>
        /// Gets the persistent data. This method is separate to the protected property because I do not want WPF binding to consume it directly. 
        /// It doesnt impl INotifyPropertyChanged.       
        /// This method needs to be called to get the persistent data object for saving to disk. This gives the objects a change to ensure all
        /// data is up to date and stored in the persistent objects correctly.
        /// </summary>
        /// <returns></returns>
        public virtual VisualisableTypeData ExtractPersistentData()
        {
            return PersistentDataField;
        }

        public override bool Equals(object obj)
        {
            var otherType = obj as VisualisableType;
            if (otherType == null)
            {
                return false;
            }

            return AssemblyQualifiedName.Equals(otherType.AssemblyQualifiedName);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Sets the consumes collection. Intended to be used by <see cref="VisualisableTypeWithAssociations"/>. Must be on this base class to be polymorphic.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="ilReader">The il reader.</param>
        protected virtual void SetConsumes(MethodBase method, IMethodBodyReader ilReader)
        {
            if (method == null)
            {
                throw new ArgumentNullResourceException("method", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (ilReader == null)
            {
                throw new ArgumentNullResourceException("ilReader", Resources.General_Given_Parameter_Cannot_Be_Null);
            }
        }

        protected virtual void SetStaticAssociations(MethodBase consumedStatics, IMethodBodyReader ilReader)
        {
            if (consumedStatics == null)
            {
                throw new ArgumentNullResourceException("consumedStatics", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (ilReader == null)
            {
                throw new ArgumentNullResourceException("ilReader", Resources.General_Given_Parameter_Cannot_Be_Null);
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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
                IMethodBodyReader ilReader = CreateMethodBodyReader(method);
                SetConsumes(method, ilReader);
                SetStaticAssociations(method, ilReader);
                locCalc += ilReader.Instructions.Count();
            }

            Logger.Instance.WriteEntry("{0} Finished Calculate LOC {1}", DateTime.Now, stopWatch.ElapsedMilliseconds);
            return locCalc;
        }

        private void CalculateLocAndConsumptionAssociations(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Logger.Instance.WriteEntry("{0} SetLinesOfCodeAndStaticAssociations(type) - invoking CalculateLoc(methods)", DateTime.Now);
            int locCalc = CalculateLoc(methods);
            if (locCalc != -1)
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                Logger.Instance.WriteEntry("{0} SetLinesOfCodeAndStaticAssociations(type) - invoking CalculateLoc(constructors)", DateTime.Now);
                locCalc += CalculateLoc(constructors);
            }

            LinesOfCode = locCalc;
        }

        private IMethodBodyReader CreateMethodBodyReader(MethodBase method)
        {
            var reader = Factory.TryGetInstance<IMethodBodyReader>();
            if (reader == null)
            {
                reader = new MethodBodyReader();
            }

            reader.Read(method);
            return reader;
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SetAssociations(Type type)
        {
            PropertyCount = (from p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) select p).Count();

            var excludeMethodList = new[] { "ToString", "GetHashCode", "GetType", "Equals", "get_", "set_" };
            if (type.IsClass || type.IsInterface || type.IsValueType)
            {
                MethodCount =
                    (from m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     where !excludeMethodList.Contains(m.Name) && !m.IsSpecialName
                     select m).Count();
            } else
            {
                MethodCount = 0;
            }

            ConstructorCount = (from m in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) select m).Count();

            EventCount = (from e in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly) select e).Count();

            IEnumerable<FieldInfo> fields = (from f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                                             where !(f.Name.Contains("__BackingField") || typeof (EventHandler).IsAssignableFrom(f.FieldType))
                                             select f);
            FieldCount = fields.Count();
        }

        private void SetLinesOfCodeAndStaticAssociations(Type type)
        {
            if (IsSubject)
            {
                // Subject needs to know about consume and static associations up front so they can be drawn into the diagram.
                CalculateLocAndConsumptionAssociations(type);
                return;
            }

            // Non-subject types on diagram (Associations of the subject) only calculate LOC on a background thread, but check the cache first.
            if (BackgroundLimitedScheduler != null)
            {
                LinesOfCodeTask = Task.Factory.StartNew(() => CalculateLocAndConsumptionAssociations(type), CancellationToken.None, TaskCreationOptions.LongRunning, BackgroundLimitedScheduler);
            } else
            {
                LinesOfCodeTask = Task.Factory.StartNew(() => CalculateLocAndConsumptionAssociations(type), TaskCreationOptions.LongRunning);
            }
        }

        private void SetToolTip(Type type)
        {
            if (type.IsClass)
            {
                TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} Class: {1}", GetAccessor(type), Name);
                return;
            }

            if (type.IsEnum)
            {
                TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} Enumeration: {1}", GetAccessor(type), Name);
                return;
            }

            if (type.IsValueType)
            {
                TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} ValueType: {1}", GetAccessor(type), Name);
                return;
            }

            if (type.IsInterface)
            {
                TypeToolTip = string.Format(CultureInfo.CurrentCulture, "{0} Interface: {1}", GetAccessor(type), Name);
            }
        }
    }
}