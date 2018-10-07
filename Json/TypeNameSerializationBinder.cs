using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace Starship.Core.Json {

    public class TypeNameSerializationBinder : ISerializationBinder {

        public TypeNameSerializationBinder(string typeFormat, bool attemptTypeConversion = true) {
            TypeFormat = typeFormat;
            AttemptTypeConversion = attemptTypeConversion;
            Assemblies = new List<Assembly>();
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName) {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName) {
            if (!AttemptTypeConversion) {
                return typeof(Object);
            }

            var name = string.Format(TypeFormat, typeName);

            foreach (var type in Assemblies.SelectMany(assembly => assembly.GetTypes().Where(type => type.Name == name))) {
                return type;
            }

            return Type.GetType(name);
        }

        public string TypeFormat { get; private set; }

        public bool AttemptTypeConversion { get; set; }

        public List<Assembly> Assemblies { get; set; }
    }
}