using System.Collections.Generic;
using System.Reflection;

namespace TypeVisualiser.Model.Persistence
{
    internal class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Name.Equals(x.Name);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(PropertyInfo obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.Name.GetHashCode();
        }
    }
}