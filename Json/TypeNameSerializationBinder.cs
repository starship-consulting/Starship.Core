using System;
using System.Runtime.Serialization;
using Starship.Core.Utility;

namespace Starship.Core.Json {
    public class TypeNameSerializationBinder : SerializationBinder {
        public TypeNameSerializationBinder(string typeFormat) {
            TypeFormat = typeFormat;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName) {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public override Type BindToType(string assemblyName, string typeName) {
            var resolvedTypeName = string.Format(TypeFormat, typeName);
            return ReflectionCache.Lookup(resolvedTypeName);
        }

        public string TypeFormat { get; private set; }
    }
}