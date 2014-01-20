namespace TypeVisualiserUnitTests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A utility class to allow test only access to private members for Dependency injection etc.
    /// </summary>
    public static class PrivateAccessor
    {
        /// <summary>
        /// Gets a constant value from a type.
        /// </summary>
        /// <param name="type">The type on which to look for the constant.</param>
        /// <param name="constName">Name of the constant.</param>
        /// <returns>The value of the constant.</returns>
        public static object GetConstant(Type type, string constName)
        {
            try
            {
                return GetStaticField(type, constName);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;                    
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a private field.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the private field.</returns>
        public static object GetField(object instance, string fieldName)
        {
            try
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentNullException("fieldName");
                }

                return GetFieldInfo(instance.GetType(), fieldName).GetValue(instance);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a private property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the private property.</returns>
        public static object GetProperty(object instance, string propertyName)
        {
            try
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new ArgumentNullException("propertyName");
                }

                return GetPropertyInfo(instance.GetType(), propertyName).GetValue(instance, new object[] { });
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a static private field.
        /// </summary>
        /// <param name="type">The type on which to look for the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the private field.</returns>
        public static object GetStaticField(Type type, string fieldName)
        {
            try
            {
                ////Guard.Against<ArgumentNullException>(type == null, "type cannot be null");
                ////Guard.Against<ArgumentNullException>(fieldName == null, "fieldName cannot be null");
                return GetStaticFieldInfo(type, fieldName).GetValue(null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the type of an internal class that resides in the same assembly as <typeparamref name="TSameAssemblyType"/>.
        /// </summary>
        /// <typeparam name="TSameAssemblyType">A type in the same assembly as the target internal type.</typeparam>
        /// <param name="concreteNamespaceAndTypeName">Name of the concrete namespace and type you want to get the typeof.</param>
        /// <returns>The internal type reference</returns>
        public static Type GetInternalType<TSameAssemblyType>(string concreteNamespaceAndTypeName) where TSameAssemblyType : class
        {
            try
            {
                Type type = typeof(TSameAssemblyType);
                Assembly assembly = type.Assembly;
                Type concreteType = assembly.GetType(concreteNamespaceAndTypeName);
                if (concreteType == null)
                {
                    throw new NotSupportedException("The type '" + concreteNamespaceAndTypeName + "' cannot be found in the same assembly as '" + assembly.FullName + "'");
                }

                return concreteType;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Invokes a non-public function.
        /// </summary>
        /// <typeparam name="T">The return type of the member to invoke</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The return value of the method</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Typed function saves time and code")]
        public static T InvokeFunction<T>(object instance, string name, params object[] arguments)
        {
            try
            {
                ////Guard.Against<ArgumentNullException>(instance == null, "instance cannot be null");
                ////Guard.Against<ArgumentNullException>(name == null, "name cannot be null");

                Type returnType = typeof(T);
                object result = GetMethod(instance.GetType(), name, arguments, instance);
                try
                {
                    return (T)result;
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "The return type was not the expected type. Expected {0}. But was {1}", returnType.Name, result.GetType().Name), ex);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Invokes a non-public method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="arguments">The arguments.</param>
        public static void InvokeMethod(object instance, string name, params object[] arguments)
        {
            ////Guard.Against<ArgumentNullException>(instance == null, "instance cannot be null");
            ////Guard.Against<ArgumentNullException>(name == null, "name cannot be null");

            try
            {
                GetMethod(instance.GetType(), name, arguments, instance);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Invokes a non-public static function.
        /// </summary>
        /// <typeparam name="T">The return type of the member to invoke</typeparam>
        /// <param name="type">The Type on which to look for the static method.</param>
        /// <param name="name">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The typed return value of the method</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Typed function saves time and code")]
        public static T InvokeStaticFunction<T>(Type type, string name, params object[] arguments)
        {
            try
            {
                ////Guard.Against<ArgumentNullException>(type == null, "type cannot be null");
                ////Guard.Against<ArgumentNullException>(name == null, "name cannot be null");

                Type returnType = typeof(T);
                object result = GetMethod(type, name, arguments, null);
                try
                {
                    return (T)result;
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "The return type was not the expected type. Expected {0}. But was {1}", returnType.Name, result.GetType().Name), ex);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Invokes a non-public static method.
        /// </summary>
        /// <param name="type">The Type on which to look for the static method.</param>
        /// <param name="name">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        public static void InvokeStaticMethod(Type type, string name, params object[] arguments)
        {
            try
            {
                ////Guard.Against<ArgumentNullException>(type == null, "type cannot be null");
                ////Guard.Against<ArgumentNullException>(name == null, "name cannot be null");

                GetMethod(type, name, arguments, null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Accesses a Private / Internal / Protected default constructor and creates an object.
        /// </summary>
        /// <typeparam name="T">The type on which to access the constructor</typeparam>
        /// <returns>A newly constructed object</returns>
        public static T PrivateConstructor<T>() where T : class
        {
            try
            {
                var type = typeof(T);
                var constructor = type.GetConstructor(new Type[] { });
                return constructor.Invoke(new object[] { }) as T;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Accesses a Private / Internal / Protected default constructor and creates an object.
        /// </summary>
        /// <typeparam name="TClass">The type on which to access the constructor</typeparam>
        /// <param name="arguments">The arguments for the constructor.</param>
        /// <returns>A newly constructed object</returns>
        public static TClass PrivateConstructor<TClass>(params object[] arguments) where TClass : class
        {
            try
            {
                var argTypes = (from arg in arguments
                                select arg.GetType())
                                .ToArray();
                var ctor = typeof(TClass).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, argTypes, null);
                if (ctor == null)
                {
                    throw new NotSupportedException("There is no constructor that takes the exact type arguments you supplied.");
                }

                return ctor.Invoke(arguments) as TClass;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Accesses a Private / Internal / Protected default constructor and creates an object. This overload is commonly used with internal types.
        /// </summary>
        /// <param name="concreteTypeToCreate">The concrete type to create.</param>
        /// <param name="constructorArgumentTypes">The constructor argument types. These types are used to find a constructor that takes these exact types.</param>
        /// <param name="arguments">The arguments to pass into the constructor.</param>
        /// <returns>A newly constructed object</returns>
        public static object PrivateConstructor(Type concreteTypeToCreate, Type[] constructorArgumentTypes, params object[] arguments)
        {
            try
            {
                var ctor = concreteTypeToCreate.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, constructorArgumentTypes, null);
                if (ctor == null)
                {
                    throw new NotSupportedException("There is no constructor that takes the type arguments you supplied.");
                }

                return ctor.Invoke(arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Accesses a Private / Internal / Protected default constructor and creates an object. This overload is commonly used with public types who use
        /// internal constructors.
        /// </summary>
        /// <typeparam name="TConcreteTypeToCreate">The concrete type to create.</typeparam>
        /// <param name="constructorArgumentTypes">The constructor argument types. These types are used to find a constructor that takes these exact types.</param>
        /// <param name="arguments">The arguments to pass into the constructor.</param>
        /// <returns>A newly constructed object</returns>
        public static TConcreteTypeToCreate PrivateConstructor<TConcreteTypeToCreate>(
            Type[] constructorArgumentTypes,
            params object[] arguments)
            where TConcreteTypeToCreate : class
        {
            try
            {
                return PrivateConstructor(typeof(TConcreteTypeToCreate), constructorArgumentTypes, arguments) as TConcreteTypeToCreate;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Accesses an Internal type and invoke a constructor and create an object.
        /// </summary>
        /// <typeparam name="TInterface">The type on which to access the constructor</typeparam>
        /// <param name="concreteNamespaceAndTypeName">Name of the concrete namespace and type to create. This is the internal type.</param>
        /// <param name="argumentTypes">The argument types for the constructor. These are used to find the appropriate constructor to use.</param>
        /// <param name="arguments">The arguments for the constructor.</param>
        /// <returns>A newly constructed object</returns>
        public static object PrivateConstructor<TInterface>(string concreteNamespaceAndTypeName, Type[] argumentTypes, params object[] arguments) where TInterface : class
        {
            try
            {
                Type concreteType = GetInternalType<TInterface>(concreteNamespaceAndTypeName);
                return PrivateConstructor(concreteType, argumentTypes, arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Sets the private field.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value to set it to.</param>
        public static void SetField(object instance, string fieldName, object value)
        {
            try
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentNullException("fieldName");
                }

                GetFieldInfo(instance.GetType(), fieldName).SetValue(instance, value);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Sets the private field.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value to set it to.</param>
        public static void SetField<T>(T instance, string fieldName, object value)
        {
            try
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new ArgumentNullException("fieldName");
                }

                GetFieldInfo(typeof(T), fieldName).SetValue(instance, value);
            } catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Sets the private property.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value to set it to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            try
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new ArgumentNullException("propertyName");
                }

                GetPropertyInfo(instance.GetType(), propertyName).SetValue(instance, value, new object[] { });
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        /// <summary>
        /// Sets the static private field.
        /// </summary>
        /// <param name="type">The type on which to look for the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value to set it to.</param>
        public static void SetStaticField(Type type, string fieldName, object value)
        {
            try
            {
                ////Guard.Against<ArgumentNullException>(type == null, "type cannot be null");
                ////Guard.Against<ArgumentNullException>(fieldName == null, "fieldName cannot be null");
                GetStaticFieldInfo(type, fieldName).SetValue(null, value);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            try
            {
                var info = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (info == null)
                {
                    throw new ArgumentException("Field does not exist (is the field static?)", "fieldName");
                }

                return info;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private static object GetMethod(Type type, string name, object[] arguments, object instance)
        {
            try
            {
                bool isStatic = instance == null;
                var flags = isStatic ? BindingFlags.NonPublic | BindingFlags.Static : BindingFlags.Instance | BindingFlags.NonPublic;
                var method = type.GetMethod(name, flags);
                if (method == null)
                {
                    throw new NotSupportedException("Type does not support this member name. " + name);
                }

                return method.Invoke(instance, arguments);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            try
            {
                var info = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (info == null)
                {
                    throw new ArgumentException("Field does not exist", "propertyName");
                }

                return info;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private static FieldInfo GetStaticFieldInfo(Type type, string fieldName)
        {
            try
            {
                var info = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
                if (info == null)
                {
                    throw new ArgumentException("Static Field does not exist (is the field instance?)", "fieldName");
                }

                return info;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }
    }
}
