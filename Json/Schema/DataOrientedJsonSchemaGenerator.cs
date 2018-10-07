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

        /*public void AddType(Type type) {
          Types.Add(type);
        }*/

        /*private static string ResolveTypeName(Type type) {
          if (type.Is<HttpResponseMessage>()) {
            return "HttpStatusCode";
          }
    
          if (type.IsGenericType) {
            if (type.IsCollection()) {
              return "array";
            }
    
            return type.Name.Split('`').First().Replace("Model", "");
          }
    
          return type.Name.Replace("Model", "");
        }*/

        /*public static SimpleJsonSchema GenerateSchemaSimple(Type type) {
          var typeSchema = Schema();
          var typeName = ResolveTypeName(type).CamelCase();
    
          typeSchema.Add("type", typeName);
    
          if (type.IsGenericType) {
            type = type.GenericTypeArguments.First();
            typeSchema.Add("of", type.Name.CamelCase());
          }
    
          if (type.Namespace == "System" || type.IsPrimitive) {
            return typeSchema;
          }
    
          var typeProperties = GetSchema(type);
    
          if (!type.Is<HttpResponseMessage>()) {
            typeSchema.Add("body", typeProperties);
          }
    
          return typeSchema;
        }*/

        /*public static SimpleJsonSchema GetSchema(params ParameterInfo[] parameters) {
          var schema = Schema();
    
          foreach (var parameter in parameters) {
            schema.Add(parameter.Name, GetSchema(parameter.ParameterType));
          }
    
          return schema;
        }*/

        /*public static SimpleJsonSchema GenerateJavascriptSchema(Type type) {
          var typeSchema = Schema();
    
          if (type.IsEnum) {
            typeSchema.Add("type", "number");
    
            var options = Schema();
    
            foreach (var value in Enum.GetValues(type)) {
              options.Add(Enum.GetName(type, value), value);
            }
    
            typeSchema.Add("options", options);
    
            return typeSchema;
          }
    
          var typeProperties = Schema();
          typeSchema.Add("properties", typeProperties);
    
          foreach (var property in type.GetProperties().OrderByDescending(each => each.Name == "Id").ThenBy(each => each.Name)) {
            typeProperties.Add(GetPropertyName(property), GetPropertyDefinition(property.PropertyType).ToString());
          }
    
          var instanceMethods = type.GetMethods().Where(method => method.IsPublic && IsVoidReturnType(method) && !method.IsStatic && !method.IsSpecialName && !method.IsVirtual).ToList();
          var staticMethods = type.GetMethods().Where(method => method.IsPublic && method.IsStatic && !method.IsSpecialName && !method.IsVirtual).ToList();
    
          var instanceMethodsSchema = MapSchema(instanceMethods);
          var staticMethodsSchema = MapSchema(staticMethods);
    
          if (instanceMethodsSchema != null) {
            typeSchema.Add("methods", instanceMethodsSchema);
          }
    
          if (staticMethodsSchema != null) {
            typeSchema.Add("static", staticMethodsSchema);
          }
    
          return typeSchema;
        }*/

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

            var typeSchema = GenerateSchema(type, true);

            typeSchema.Add("id", "#" + type.Name);
            typeSchema.Add("title", type.Name);

            if (type.GetInterfaces().Any()) {
                typeSchema.Add("interfaces", type.GetInterfaces().Select(each => each.Name).ToList());
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

        private static void MapInterfaces(SimpleJsonSchema schema, Type type) {
            foreach (var _interface in type.GetInterfaces()) {
                var interfaceSchema = Schema();

                schema.Add("interfaces", type.GetInterfaces());
            }
        }

        public static SimpleJsonSchema GenerateSchema(Type type, bool includeMethods = false) {
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
                    else if (property.Is<int>() || property.Is<decimal>() || property.Is<double>() || property.Is<float>()) {
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

                    typeProperties.Add(property.Name.CamelCase(), typeProperty);
                }

                if (includeMethods) {
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

        private static List<Type> LateBoundTypes { get; set; }
    }
}