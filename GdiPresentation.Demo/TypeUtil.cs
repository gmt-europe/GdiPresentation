using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GdiPresentation.Demo
{
    internal static class TypeUtil
    {
        public static bool CanBeInstantiated(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return
                !type.IsAbstract &&
                !type.IsGenericType &&
                type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length > 0;
        }
    }
}
