using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Starship.Core.Models;
using Starship.Core.Reflection;

namespace Starship.Core.Extensions {
    public static class ObjectExtensions {
        
        public static T GetFieldValue<T>(this object source, string fieldName) {
            return (T) source.GetType().FindField(fieldName).GetValue(source);
        }

        public static T GetPropertyValue<T>(this object source, string propertyName) {
            return (T) source.GetType().FindProperty(propertyName).GetValue(source, null);
        }

        public static IQueryable<T> ToQueryable<T>(this T entity) {
            return new List<T> {
                entity
            }.AsQueryable();
        }

        public static void SetPropertyValue(this object target, string name, object value) {
            var propertyInstance = target.InternalGetProperty(name);
            propertyInstance.Property.SetValue(propertyInstance.Owner, value);
        }

        public static object GetPropertyValue(this object target, string name) {
            return target.InternalGetProperty(name).Value;
        }

        public static PropertyInfo GetProperty(this object target, string name) {
            return target.InternalGetProperty(name).Property;
        }

        private static PropertyInstance InternalGetProperty(this object target, string name) {
            var propertyNames = name.Split('.');

            var propertyInstance = new PropertyInstance {
                Value = target
            };

            foreach (var propertyName in propertyNames) {
                propertyInstance.Property = propertyInstance.Value.GetType()
                    .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance |
                                   BindingFlags.Public)
                    .First(each => each.Name.ToLower() == propertyName.ToLower());

                propertyInstance.Owner = propertyInstance.Value;
                propertyInstance.Value = propertyInstance.Property.GetValue(propertyInstance.Value);
            }

            return propertyInstance;
        }

        public static string ToQueryString(this object data) {
            var json = (JObject) JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data));
            var fields = json.Children().Cast<JProperty>().Where(each => each.Value.Type != JTokenType.Null);
            var pairs = fields.Select(jp => jp.Name + "=" + Uri.EscapeDataString(jp.Value.ToString().Replace(@"""", "")));

            return string.Join("&", pairs.ToArray());
        }

        public static List<ExtractedProperty<T>> ExtractProperties<T>(this object source) {
            var results = new List<ExtractedProperty<T>>();

            foreach (var property in source.GetType().GetProperties()) {
                if (property.PropertyType.Is<T>()) {
                    var value = (T) property.GetValue(source, null);

                    if (value != null) {
                        var extractedProperty = new ExtractedProperty<T> {
                            Property = property,
                            Value = value
                        };

                        results.Add(extractedProperty);
                    }
                }
            }

            return results;
        }
        
        public static object As<T>(this object source, Func<T, object> action) where T : class {
            var target = source.As<T>();

            if (target != null) {
                return action(target);
            }

            return default(T);
        }

        public static T As<T>(this object source) where T : class {
            return source.As(typeof(T)) as T;
        }

        public static object As(this object source, Type type) {
            return TypeConverter.Convert(source, type);
        }

        public static object InvokeStaticGenericMethod(this Type type, string methodName, Type genericType,
            params object[] parameters) {
            var methods = type.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var method = methods.First(each => each.Name == methodName && each.IsGenericMethod);
            var genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(null, parameters);
        }

        public static object InvokeGenericMethod(this object source, string methodName, Type genericType,
            params object[] parameters) {
            var type = source.GetType();
            var methods = type.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var method = methods.First(each => each.Name == methodName && each.IsGenericMethod);
            var genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(source, parameters);
        }

        public static T DeepJsonClone<T>(this T source) {
            return JsonConvert.DeserializeObject<T>(source.DeepSerialize());
        }

        public static string ShallowSerialize(this object instance) {
            return JsonConvert.SerializeObject(instance, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        public static string DeepSerialize(this object instance) {
            return JsonConvert.SerializeObject(instance, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }

        private static UniversalTypeConverter TypeConverter = new UniversalTypeConverter();
    }
}