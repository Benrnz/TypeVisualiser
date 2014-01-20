using System;

namespace TypeVisualiser.Model.Persistence
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class PersistentAttribute : Attribute
    {
    }
}
