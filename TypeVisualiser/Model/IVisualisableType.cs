namespace TypeVisualiser.Model
{
    using System;
    using Persistence;

    /// <summary>
    /// An interface for visualisable type. Provides a wrapper around <see cref="System.Type"/> with properties for convenience.
    /// </summary>
    public interface IVisualisableType
    {
        string AssemblyFileName { get; }

        /// <summary>
        /// Gets the full name of the assembly.
        /// </summary>
        /// <value>The full name of the assembly.</value>
        string AssemblyFullName { get; }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        string AssemblyName { get; }

        /// <summary>
        /// Gets the number of constructors.
        /// </summary>
        /// <value>The constructor count.</value>
        int ConstructorCount { get; }

        /// <summary>
        /// Gets the number of enum members.
        /// </summary>
        /// <value>The enum member count.</value>
        int EnumMemberCount { get; }

        /// <summary>
        /// Gets the number of events.
        /// </summary>
        /// <value>The event count.</value>
        int EventCount { get; }

        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        /// <value>The field count.</value>
        int FieldCount { get; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        string AssemblyQualifiedName { get; }

        /// <summary>
        /// Gets the type id. <see cref="Type.get_GUID"/>
        /// This could be either a single Guid indicating a non-generic type, otherwise multiple Guids 
        /// </summary>
        /// <value>The id.</value>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is the main subject of the diagram.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is subject; otherwise, <c>false</c>.
        /// </value>
        bool IsSubject { get; }

        /// <summary>
        /// Gets or sets the Lines of Code. This is the number of IL Lines of Code not C#.
        /// </summary>
        /// <value>The LinesOfCode.</value>
        int LinesOfCode { get; set; }

        /// <summary>
        /// Gets the lines of code tool tip.
        /// </summary>
        /// <value>The lines of code tool tip.</value>
        string LinesOfCodeToolTip { get; }

        /// <summary>
        /// Gets the number of methods.
        /// </summary>
        /// <value>The method count.</value>
        int MethodCount { get; }

        /// <summary>
        /// Gets the modifiers class containing information about accessors etc.
        /// </summary>
        /// <value>The modifiers.</value>
        ModifiersData Modifiers { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the name with the namespace prefix.
        /// </summary>
        /// <value>
        /// The name of the namespaced.
        /// </value>
        string NamespaceQualifiedName { get; }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        string ThisTypeNamespace { get; }

        /// <summary>
        /// Gets the number of properties.
        /// </summary>
        /// <value>The property count.</value>
        int PropertyCount { get; }

        /// <summary>
        /// Gets the kind of this type, is it the subject or an associate.
        /// </summary>
        /// <value>The subject or associate.</value>
        SubjectOrAssociate SubjectOrAssociate { get; }

        /// <summary>
        /// Gets the name of the tool tip.
        /// </summary>
        /// <value>The name of the tool tip.</value>
        string TypeToolTip { get; }

        /// <summary>
        /// Gets the persistent data. This method is separate to the protected property because I do not want WPF binding to consume it directly. 
        /// It doesnt impl INotifyPropertyChanged.       
        /// This method needs to be called to get the persistent data object for saving to disk. This gives the objects a change to ensure all
        /// data is up to date and stored in the persistent objects correctly.
        /// </summary>
        VisualisableTypeData ExtractPersistentData();
    }
}