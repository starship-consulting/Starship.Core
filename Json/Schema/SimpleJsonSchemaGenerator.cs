using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Starship.Core.Extensions;
using Starship.Core.Json.Attributes;

namespace Starship.Core.Json.Schema {
    public class SimpleJsonSchemaGenerator {
        public SimpleJsonSchemaGenerator() {
            Types = new List<Type>();
        }

        public void AddNamespace(Assembly assembly, String nameSpace) {
            var types = assembly.GetTypes();

            Types.AddRange(types.Where(each => each.IsClass && !each.IsNestedPrivate && each.Namespace != null && each.Namespace.StartsWith(nameSpace)).ToList());
        }

        public void AddType(Type type) {
            Types.Add(type);
        }

        public static SimpleJsonSchema GenerateSchemaSimple(Type type) {
            var typeSchema = Schema();

            var typeSections = Schema();
            typeSchema.Add(type.Name, typeSections);

            var description = type.GetCustomAttribute<DescriptionAttribute>();

            if (description != null) {
                typeSections.Add("description", description.Description);
            }

            var typeProperties = Schema();
            typeSections.Add("properties", typeProperties);

            foreach (var property in type.GetProperties()) {

                if (property.GetCustomAttribute<ApiIgnoreAttribute>() != null) {
                    continue;
                }

                var typeProperty = Schema();

                if (property.Is<string>()) {
                    typeProperty.Add("type", "string");
                }
                else if (property.Is<bool>()) {
                    typeProperty.Add("type", "boolean");
                }
                else if (property.Is<DateTime>()) {
                    typeProperty.Add("type", "string");
                }
                else if (property.PropertyType.IsEnum || property.Is<int>() || property.Is<decimal>() || property.Is<double>() || property.Is<float>()) {
                    typeProperty.Add("type", "number");
                }
                else if (property.PropertyType.IsGenericType && GetEnumerableType(property.PropertyType) != null) {
                    typeProperty.Add("type", "array");

                    var items = Schema();
                    items.Add("$ref", GetEnumerableType(property.PropertyType).Name);
                    typeProperty.Add("items", items);
                }
                else if (property.PropertyType.IsClass) {
                    typeProperty.Add("$ref", property.PropertyType.Name);
                }
                else {
                    typeProperty.Add("type", "object");
                }

                typeProperties.Add(property.Name.CamelCase(), GetPropertyType(property));
            }

            return typeSchema;
        }

        private static string GetPropertyType(PropertyInfo property) {

            if (property.Is<string>()) {
                return "string";
            }

            if (property.Is<bool>()) {
                return "boolean";
            }

            if (property.Is<DateTime>()) {
                return "date";
            }

            if (property.PropertyType.IsEnum || property.Is<int>() || property.Is<decimal>() || property.Is<double>() || property.Is<float>()) {
                return "number";
            }

            if (property.PropertyType.IsGenericType && GetEnumerableType(property.PropertyType) != null) {
                return "array";
            }

            return "object";
        }

        public static SimpleJsonSchema GenerateSchema(Type type) {
            var typeSchema = Schema();

            typeSchema.Add("type", "object");

            var typeProperties = Schema();
            typeSchema.Add("properties", typeProperties);

            foreach (var property in type.GetProperties()) {
                var typeProperty = Schema();

                if (property.Is<string>()) {
                    typeProperty.Add("type", new[] {"string", "null"});
                }
                else if (property.Is<bool>()) {
                    typeProperty.Add("type", "boolean");
                }
                else if (property.Is<DateTime>()) {
                    typeProperty.Add("type", "string");
                }
                else if (property.PropertyType.IsEnum || property.Is<int>() || property.Is<decimal>() || property.Is<double>() || property.Is<float>()) {
                    typeProperty.Add("type", "number");
                }
                else if (property.PropertyType.IsGenericType && GetEnumerableType(property.PropertyType) != null) {
                    typeProperty.Add("type", "array");

                    var items = Schema();
                    items.Add("$ref", GetEnumerableType(property.PropertyType).Name);
                    typeProperty.Add("items", items);
                }
                else if (property.PropertyType.IsClass) {
                    typeProperty.Add("$ref", property.PropertyType.Name);
                }
                else {
                    typeProperty.Add("type", "object");
                }

                typeProperties.Add(property.Name.CamelCase(), typeProperty);
            }

            return typeSchema;
        }

        public void InterceptType(Action<Type> action) {
        }

        public List<SimpleJsonSchema> GenerateSchemaArray(JsonSchemaConfiguration configuration = null) {
            if (configuration == null) {
                configuration = JsonSchemaConfiguration.Default;
            }

            var schema = new List<SimpleJsonSchema>();

            foreach (var type in Types) {
                if (configuration.SimpleOutput) {
                    schema.Add(GenerateSchemaSimple(type));
                }
                else {
                    var typeSchema = GenerateSchema(type);
                    typeSchema.Add("name", type.Name);
                    schema.Add(typeSchema);
                }
            }

            return schema;
        }

        public SimpleJsonSchema GenerateSchemaObject() {
            var schema = Schema();

            schema.Add("$schema", "http://json-schema.org/draft-04/schema#");
            schema.Add("title", "root");
            schema.Add("id", "#root");

            var properties = Schema();

            schema.Add("properties", properties);

            foreach (var type in Types) {

                // Silently ignore duplicates
                if (properties.ContainsKey(type.Name)) {
                    continue;
                }

                // Ignore generics
                if (type.Name.Contains("<>")) {
                    continue;
                }

                var typeSchema = GenerateSchema(type);

                typeSchema.Add("id", "#" + type.Name);
                typeSchema.Add("title", type.Name);

                properties.Add(type.Name, typeSchema);
            }

            return schema;
        }

        private static Type GetEnumerableType(Type type) {
            return (from intType in type.GetInterfaces()
                where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof (IEnumerable<>)
                select intType.GetGenericArguments()[0]).FirstOrDefault();
        }

        private static SimpleJsonSchema Schema() {
            return new SimpleJsonSchema();
        }

        public List<Type> Types { get; set; }
    }
}