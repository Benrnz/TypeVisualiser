namespace TypeVisualiser.Model.Persistence
{
    /// <summary>
    /// The catergory of the reflected type.
    /// </summary>
    public enum TypeKind
    {
        /// <summary>
        /// Its unknown or not set.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Its a class
        /// </summary>
        Class,

        /// <summary>
        /// Its an interface
        /// </summary>
        Interface,

        /// <summary>
        /// Its an enumeration
        /// </summary>
        Enum,

        /// <summary>
        /// Its a value type aka scalar type.
        /// </summary>
        ValueType,
    }
}