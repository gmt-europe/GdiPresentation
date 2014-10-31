using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GdiPresentation.Demo
{
    public static class ReflectionExtensions
    {
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            return GetCustomAttribute<T>(type, true, false);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit)
            where T : Attribute
        {
            return GetCustomAttribute<T>(type, inherit, false);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit, bool throwWhenNotFound)
            where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), inherit);

            if (attributes.Length == 0 && throwWhenNotFound)
                throw new ArgumentException("Attribute not found");

            return attributes.Length == 0 ? null : (T)attributes[0];
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            return GetCustomAttributes<T>(type, true, false);
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type, bool inherit)
            where T : Attribute
        {
            return GetCustomAttributes<T>(type, inherit, false);
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type, bool inherit, bool throwWhenNotFound)
            where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), inherit);

            if (attributes.Length == 0 && throwWhenNotFound)
                throw new ArgumentException("Attribute not found");

            var result = new T[attributes.Length];

            for (int i = 0; i < attributes.Length; i++)
            {
                result[i] = (T)attributes[i];
            }

            return result;
        }

        public static bool IsAssignableFromGeneric(this Type self, Type type)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (type == null)
                throw new ArgumentNullException("type");
            if (!self.IsGenericTypeDefinition)
                throw new ArgumentException("Not a generic type definition");

            if (self.IsInterface)
            {
                if (
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == self
                )
                    return true;

                foreach (var item in type.GetInterfaces())
                {
                    if (
                        item.IsGenericType &&
                        item.GetGenericTypeDefinition() == self
                    )
                        return true;
                }

                type = type.BaseType;
            }
            else
            {
                while (type != typeof(object))
                {
                    if (
                        type.IsGenericType &&
                        type.GetGenericTypeDefinition() == self
                    )
                        return true;

                    type = type.BaseType;
                }
            }

            return false;
        }

        public static string GetUnversionedName(this Type self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            var sb = new StringBuilder();

            GetUnversionedName(sb, self);

            return sb.ToString();
        }

        private static void GetUnversionedName(StringBuilder sb, Type type)
        {
            string name = type.FullName;

            int pos = name.IndexOf('[');

            if (pos == -1)
            {
                sb.Append(name);
            }
            else
            {
                sb.Append(name.Substring(0, pos));
                sb.Append("[[");

                bool hadOne = false;

                foreach (var templateType in type.GetGenericArguments())
                {
                    if (hadOne)
                        sb.Append("],[");
                    else
                        hadOne = true;

                    GetUnversionedName(sb, templateType);
                }

                sb.Append("]]");
            }

            sb.Append(", ");
            sb.Append(type.Assembly.GetName().Name);
        }
    }
}
