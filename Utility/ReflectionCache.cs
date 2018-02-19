using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Starship.Core.Extensions;
using Starship.Core.Reflection;

namespace Starship.Core.Utility {
    public static class ReflectionCache {
        static ReflectionCache() {
            DuplicateTypes = new List<Type>();
            Types = new Dictionary<string, TypeMap>();
        }

        public static void Initialize(params Assembly[] assemblies) {
            try {
                lock (SyncRoot) {
                    if (IsInitialized) {
                        throw new Exception("Reflection cache is already initialized.");
                    }

                    IsInitialized = true;

                    foreach (var assembly in assemblies) {
                        foreach (var type in assembly.DefinedTypes) {
                            AddTypeMap(type);
                        }

                        if (DuplicateTypes.Any()) {
                            //throw new Exception("Duplicate types found: " + string.Join(" ", duplicates.Select(each => each.FullName + Environment.NewLine)));
                        }
                    }

                    MapDerivedTypes();
                    MapExtensionMethods(assemblies);
                }
            }
            catch (ReflectionTypeLoadException ex) {
                var exceptions = ex.LoaderExceptions;
                throw ex;
            }
        }

        private static void MapDerivedTypes() {
            foreach (var type in GetTypes().ToList()) {
                var baseTypes = type.GetBaseTypes();

                foreach (var baseType in baseTypes) {
                    GetTypeMap(baseType)?.AddDerivedType(type);
                }
            }
        }

        private static void MapExtensionMethods(params Assembly[] assemblies) {
            foreach (var assembly in assemblies) {
                foreach (var type in assembly.DefinedTypes) {
                    foreach (var method in type.GetMethods()) {
                        if (method.IsExtensionMethod()) {
                            var parameterType = method.GetParameters().First().ParameterType;
                            var map = GetTypeMap(parameterType);

                            if (map != null) {
                                var extensionMethodTypes = map.DerivedTypes.ToList();
                                extensionMethodTypes.Add(parameterType);

                                foreach (var extensionMethodType in extensionMethodTypes) {
                                    GetTypeMap(extensionMethodType)?.AddExtensionMethod(method);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static object InvokeStaticMethod(this Type type, string name, Type genericType, params object[] parameters) {
            var method = type.FindMethod(name, parameters);
            var genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(null, parameters);
        }

        public static object InvokeStaticMethod(this Type type, string name, params object[] parameters) {
            return type.FindMethod(name, parameters).Invoke(null, parameters);
        }

        public static object InvokeMethod(this object target, string name, params object[] parameters) {
            var method = target.GetType().FindMethod(name, parameters);

            if (method == null) {
                throw new Exception("No method found to invoke.");
            }

            return InternalInvoke(method, target, parameters);
        }

        public static object InvokeGenericMethod(this object target, string methodName, Type genericType, params object[] parameters) {
            var type = target.GetType();
            var method = type.FindMethod(methodName, parameters);
            var genericMethod = method.MakeGenericMethod(genericType);

            return InternalInvoke(genericMethod, target, parameters);
        }

        private static object InternalInvoke(MethodInfo method, object target, params object[] parameters) {
            return method.Invoke(target, parameters);
        }

        public static MethodInfo FindMethod(this Type type, string name, params object[] parameters) {
            var methods = type.GetMethods()
                .Concat(type.GetExtensionMethods())
                .Where(each => each.Name.ToLower() == name.ToLower())
                .ToList();

            foreach (var method in methods) {
                var match = true;
                var allParameters = method.GetParameters();
                var parameterCount = allParameters.Count();

                if (method.IsExtensionMethod()) {
                    parameterCount -= 1;
                }

                if (parameterCount != parameters.Count()) {
                    continue;
                }

                for (var index = 0; index < parameterCount; index++) {
                    var expectedType = allParameters[index].ParameterType;
                    var inType = parameters[index].GetType();

                    if (!expectedType.IsAssignableFrom(inType)) {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    return method;
                }
            }

            return null;
        }

        public static List<MethodInfo> GetExtensionMethods(this Type type) {
            lock (SyncRoot) {
                var map = GetTypeMap(type);

                if (map == null) {
                    return new List<MethodInfo>();
                }

                return map.ExtensionMethods.ToList();
            }
        }

        private static TypeMap AddTypeMap(Type type) {
            lock (SyncRoot) {
                var typeName = type.Name.ToLower();

                // Skip dynamic/runtime classes
                if (typeName.Contains(">")) {
                    return null;
                }

                if (Types.ContainsKey(typeName)) {
                    var existing = Types[typeName];
                    DuplicateTypes.Add(type);
                    DuplicateTypes.Add(existing.Type);
                }
                else {
                    var map = new TypeMap(type);

                    Types.Add(typeName, map);

                    //foreach (var name in type.GetTypeNames()) {
                    //    Types.Add(name.ToLower(), map);
                    //}
                }

                return Types[typeName];
            }
        }

        private static TypeMap GetTypeMap(Type type) {
            lock (SyncRoot) {
                var typeName = type.Name.ToLower();

                return Types.ContainsKey(typeName) ? Types[typeName] : null;
            }
        }

        public static Type Lookup(string typeName) {
            lock (SyncRoot) {
                typeName = typeName.ToLower();

                return Types.ContainsKey(typeName) ? Types[typeName].Type : null;
            }
        }

        public static IEnumerable<T> InstantiateTypesOf<T>(bool includeAbstract = true) {
            return GetTypesOf(typeof(T), includeAbstract).Select(each => each.New()).Cast<T>();
        }

        public static IEnumerable<Type> GetTypesOf<T>(bool includeAbstract = true) {
            return GetTypesOf(typeof(T), includeAbstract);
        }

        public static IEnumerable<Type> GetTypesOf(Type type, bool includeAbstract = true) {
            var types = GetTypes().Where(type.IsAssignableFrom);

            if (!includeAbstract) {
                types = types.Where(each => !each.IsAbstract);
            }

            return types.Distinct();
        }

        private static IEnumerable<Type> GetTypes() {
            return Types.Values.Select(each => each.Type);
        }

        private static Dictionary<string, TypeMap> Types { get; set; }

        private static List<Type> DuplicateTypes { get; set; }

        private static bool IsInitialized { get; set; }

        private static readonly object SyncRoot = new object();
    }
}