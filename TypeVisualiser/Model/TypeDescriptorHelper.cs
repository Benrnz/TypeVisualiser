using System;
using System.Text;

namespace TypeVisualiser.Model
{
    /// <summary>
    /// A helper class to handle all types (some possibly generic) and provide methods used to
    /// identify types, determine if generic, and provide meaningful descriptions.
    /// </summary>
    internal class TypeDescriptorHelper
    {
        public TypeDescriptorHelper(Type type)
        {
            DescribedType = type;
        }

        public bool IsGeneric
        {
            get { return DescribedType.IsGenericType; }
        }

        private Type DescribedType { get; set; }

        public string GenerateId()
        {
            if (IsGeneric)
            {
                var builder = new StringBuilder();
                builder.Append(DescribedType.GUID);
                foreach (Type genericArgument in DescribedType.GetGenericArguments())
                {
                    builder.Append("_");
                    builder.Append(genericArgument.GUID);
                }

                return builder.ToString();
            }

            return DescribedType.GUID.ToString();
        }

        public string GenerateName()
        {
            string name;
            int numberOfGenerics;
            int position = DescribedType.Name.IndexOf('`');

            if (position > 0 && Int32.TryParse(DescribedType.Name.Substring(position + 1), out numberOfGenerics))
            {
                var builder = new StringBuilder();
                builder.Append(DescribedType.Name.Substring(0, position) + "<");
                for (int index = 0; index < numberOfGenerics; index++)
                {
                    builder.Append(DescribedType.GetGenericArguments()[index].Name);
                    builder.Append(",");
                }

                builder.Remove(builder.Length - 1, 1);
                builder.Append(">");
                name = builder.ToString();
            } else
            {
                name = DescribedType.Name;
            }

            return name;
        }

        public static bool AreEqual(Type typeId1, string typeId2)
        {
            if (string.IsNullOrWhiteSpace(typeId2))
            {
                return false;
            }

            if (typeId1 == null)
            {
                return false;
            }

            if (typeId1.GUID.ToString() == typeId2)
            {
                return true;
            }

            var helper = new TypeDescriptorHelper(typeId1);
            var id1 = helper.GenerateId();
            return id1 == typeId2;
        }
    }
}