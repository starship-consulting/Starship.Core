using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Starship.Core.Attributes;

namespace Starship.Core.Extensions {
    public static class TypeExtensions {

        public static PropertyInfo GetPropertyWithAttribute<T>(this Type type) where T : Attribute {
            foreach (var property in type.GetProperties()) {
                if (property.HasAttribute<T>()) {
                    return property;
                }
            }
            
            return null;
        }

        public static bool IsSimple(this Type type) {
            if (!type.IsEnum && !type.FullName.StartsWith("System."))
                return false;

            if (type.IsGenericType && type.GetEnumerableType() != null)
                return false;

            return true;
        }

        public static List<MethodInfo> GetRealMethods(this Type type, bool publicOnly = false) {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            if (publicOnly)
                flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

            return type.GetMethods(flags).Where(m => !m.IsSpecialName).ToList();
        }

        public static List<string> GetTypeNames(this Type type) {
            var names = type.GetCustomAttributes<AliasAttribute>(true)
                .Select(each => each.Value)
                .Where(each => !each.IsEmpty())
                .ToList();

            names.Add(type.Name);
            return names;
        }

        public static IEnumerable<Type> GetGenericInterfacesOfType<T>(this Type type) {
            return type.GetInterfacesOfType<T>().Where(each => each.IsGenericType);
        }

        public static IEnumerable<Type> GetInterfacesOfType<T>(this Type type) {
            return type.GetBaseTypes().Where(each => each.IsInterface && typeof(T).IsAssignableFrom(each));
        }

        public static List<Type> GetBaseTypes(this Type type) {
            var types = type.GetInterfaces().ToList();

            if (type.BaseType != null && !types.Contains(type.BaseType))
                types.Add(type.BaseType);

            foreach (var eachBaseType in types.ToList()) {
                var parentTypes = eachBaseType.GetBaseTypes();

                foreach (var parentType in parentTypes)
                    if (!types.Contains(parentType))
                        types.Add(parentType);
            }

            return types;
        }

        public static object New(this Type type) {
            return Activator.CreateInstance(type);
        }

        public static T New<T>(this Type type, params Type[] genericTypes) {
            if (genericTypes.Any())
                type = type.MakeGenericType(genericTypes);

            return (T) Activator.CreateInstance(type);
        }

        public static Type GetEnumerableType(this Type type) {
            return (from intType in type.GetInterfaces()
                where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                select intType.GetGenericArguments()[0]).FirstOrDefault();
        }

        public static IList MakeList(this Type type) {
            var genericListType = typeof(List<>).MakeGenericType(type);
            return (IList) Activator.CreateInstance(genericListType);
        }

        public static bool Is<T>(this Type type) {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsCollection(this Type type) {
            return type.Is<IList>() || type.Is<ICollection>() || type.Is<IEnumerable>();
        }

        public static bool IsNullableType(this Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNonSystemClass(this Type type) {
            return type.Namespace != "System" && !type.IsPrimitive;
        }

        public static List<Type> GetTypesOf(this Type type, List<Assembly> assemblies = null, bool includeAbstract = false) {
            if (assemblies == null) {
                assemblies = new List<Assembly> { Assembly.GetAssembly(type) };
            }

            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(each => type.IsAssignableFrom(each) && (includeAbstract || each.IsAbstract == false))
                .ToList();
        }
    }
}