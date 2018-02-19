using System;
using System.Collections.Generic;
using System.Reflection;

namespace Starship.Core.Reflection {
    public class TypeMap {
        public TypeMap(Type type) {
            Type = type;
            ExtensionMethods = new List<MethodInfo>();
            DerivedTypes = new List<Type>();
        }

        internal void AddDerivedType(Type type) {
            lock (SyncRoot) {
                DerivedTypes.Add(type);
            }
        }

        internal void AddExtensionMethod(MethodInfo method) {
            lock (SyncRoot) {
                ExtensionMethods.Add(method);
            }
        }

        public Type Type { get; set; }

        public List<Type> DerivedTypes { get; set; }

        public List<MethodInfo> ExtensionMethods { get; set; }

        private readonly object SyncRoot = new object();
    }
}