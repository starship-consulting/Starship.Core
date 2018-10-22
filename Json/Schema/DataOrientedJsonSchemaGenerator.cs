using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Starship.Core.Extensions;

namespace Starship.Core.Json.Schema {

    public abstract class DataOrientedJsonSchemaGenerator {

        protected DataOrientedJsonSchemaGenerator() {
            Types = new List<Type>();
            LateBoundTypes = new List<Type>();
        }

        public abstract JsonObjectClassifierTypes GetClassification(Type type);

        public void AddNamespace(Type type) {
            AddNamespace(type.Assembly, type.Namespace);
        }

        public void AddNamespace(Assembly assembly, string nameSpace) {
            var types = assembly.GetTypes();

            Types.AddRange(types.Where(type => type.Namespace != null && type.Namespace.StartsWith(nameSpace) && ShouldIncludeType(type)).ToList());
        }

        public void AddAssembly(Assembly assembly) {
            Types.AddRange(assembly.GetTypes().Where(ShouldIncludeType).ToList());
        }

        public virtual bool ShouldIncludeType(Type type) {
            if (type.IsStatic()) {
                return false;
            }

            if (type.IsClass && !type.IsNestedPrivate) {
                return true;
            }

            if (type.IsEnum) {
                return true;
            }

            return false;
        }

        private static SimpleJsonSchema MapSchema(List<MethodInfo> methods) {
            if (methods.Any()) {
                var methodSchema = Schema();

                foreach (var method in methods) {
                    if (method.IsSpecialName) {
                        continue;
                    }

                    var name = method.Name.CamelCase();
                    var parameters = Schema();

                    // Duplicate
                    if (methodSchema.ContainsKey(name)) {
                        continue;
                    }

                    // Todo:  Map method parameters
                    /*foreach (var param in method.GetParameters()) {
                      parameters.Add(param.Name, GetPropertyDefinition(param.ParameterType).ToString());
                    }*/

                    methodSchema.Add(name, parameters);
                }

                return methodSchema;
            }

            return null;
        }

        public SimpleJsonSchema GenerateSchemaObject() {
            var schema = Schema();

            schema.Add("$schema", "http://json-schema.org/draft-04/schema#");
            schema.Add("title", "root");
            schema.Add("id", "#root");

            var properties = Schema();

            schema.Add("properties", properties);

            foreach (var type in Types.OrderBy(each => each.Name)) {
                AddToSchema(properties, type);
            }

            foreach (var latebound in LateBoundTypes) {
                AddToSchema(properties, latebound);
            }

            LateBoundTypes.Clear();

            return schema;
        }

        private void AddToSchema(SimpleJsonSchema parent, Type type) {
            // Silently ignore duplicates
            if (parent.ContainsKey(type.Name)) {
                return;
            }

            // Ignore generics
            if (type.Name.Contains("<>")) {
                return;
            }

            var typeSchema = GenerateSchema(type);

            typeSchema.Add("id", "#" + type.Name);
            typeSchema.Add("title", type.Name);

            if(IncludeInterfaces) {
                if (type.GetInterfaces().Any()) {
                    typeSchema.Add("interfaces", type.GetInterfaces().Select(each => each.Name).ToList());
                }
            }

            var classification = GetClassification(type);

            if(classification == JsonObjectClassifierTypes.Entity) {
                typeSchema.Add("isEntity", true);
            }
            else if(classification == JsonObjectClassifierTypes.Model) {
                typeSchema.Add("isModel", true);
            }

            parent.Add(type.Name, typeSchema);
        }
        
        public SimpleJsonSchema GenerateSchema(Type type) {
            var typeSchema = Schema();

            if (type.IsEnum) {
                typeSchema.Add("type", "enum");

                var options = Schema();

                foreach (var value in Enum.GetValues(type)) {
                    options.Add(Enum.GetName(type, value), value);
                }

                typeSchema.Add("options", options);
            }
            else {
                typeSchema.Add("type", "object");

                var typeProperties = Schema();
                typeSchema.Add("properties", typeProperties);

                foreach (var property in type.GetProperties()) {
                    var typeProperty = Schema();

                    if (property.Is<string>()) {
                        typeProperty.Add("type", "string");
                    }
                    else if (property.Is<Guid>()) {
                        typeProperty.Add("type", "id");
                    }
                    else if (property.Is<bool>()) {
                        typeProperty.Add("type", "boolean");
                    }
                    else if (property.Is<DateTime>()) {
                        typeProperty.Add("type", "date");
                    }
                    else if (property.Is<int>() || property.Is<decimal>() || property.Is<double>() || property.Is<float>() || property.Is<long>()) {
                        typeProperty.Add("type", "number");
                    }
                    else if (property.PropertyType.IsGenericType && GetEnumerableType(property.PropertyType) != null) {
                        //continue;
                        typeProperty.Add("type", "array");

                        var items = Schema();
                        items.Add("$ref", GetEnumerableType(property.PropertyType).Name);
                        typeProperty.Add("items", items);
                    }
                    else {
                        if (property.PropertyType.IsEnum) {
                            typeProperty.Add("type", "enum");

                            if (!LateBoundTypes.Contains(property.PropertyType)) {
                                LateBoundTypes.Add(property.PropertyType);
                            }
                        }

                        if (property.PropertyType.IsClass) {
                            continue;
                            typeProperty.Add("type", "object");
                        }

                        typeProperty.Add("$ref", property.PropertyType.Name);
                    }

                    typeProperty.Add("set", property.GetSetMethod() != null ? "true" : "false");

                    var propertyName = property.Name.CamelCase();

                    if(!typeProperties.ContainsKey(propertyName)) {
                        typeProperties.Add(property.Name.CamelCase(), typeProperty);
                    }
                }

                if (IncludeMethods) {
                    var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToList();
                    typeSchema.Add("methods", MapSchema(methods));
                }
            }

            return typeSchema;
        }

        private static Type GetEnumerableType(Type type) {
            return (from intType in type.GetInterfaces()
                where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                select intType.GetGenericArguments()[0]).FirstOrDefault();
        }

        private static SimpleJsonSchema Schema() {
            return new SimpleJsonSchema();
        }

        public List<Type> Types { get; set; }

        public bool IncludeInterfaces { get; set; }

        public bool IncludeMethods { get; set; }

        private static List<Type> LateBoundTypes { get; set; }
    }
}