using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Starship.Core.Extensions;
using Starship.Core.Interfaces;
using Starship.Core.Reflection.Converters;

namespace Starship.Core.Reflection {
    public class MethodInvoker {
        public MethodInvoker(MethodInfo method, params IConverter[] converters) {
            Method = method;
            Converters = converters.ToList();
        }

        public static MethodInvoker GetDefault(MethodInfo method) {
            return new MethodInvoker(method,
                new NumberConverter(),
                new JObjectConverter(),
                new EnumConverter(),
                new GuidConverter(),
                new JArrayConverter(),
                new DateTimeConverter());
        }

        internal object TryConvert(Type type, object value) {
            var converters = Converters.Where(converter => converter.CanConvert(type, value)).ToList();

            if (converters.Any()) {
                foreach (var converter in converters) {
                    value = converter.Convert(this, type, value);
                }
            }

            return value;
        }

        public object Invoke(object target, Dictionary<string, object> values) {
            var parameters = Method.GetParameters();

            if (parameters.Count() == 1 && parameters.First().ParameterType.IsNonSystemClass()) {
                return Invoke(target, new List<object> {
                    JsonConvert.DeserializeObject(JsonConvert.SerializeObject(values))
                });
            }

            var orderedValues = new List<object>();

            if (values != null) {
                foreach (var each in Method.GetParameters()) {
                    var match = values.Any(value => value.Key.ToLower() == each.Name.ToLower());

                    if (!match) {
                        if (each.IsOptional) {
                            continue;
                        }

                        throw new Exception("Missing parameter: " + each.Name);
                    }

                    var keyvalue = values.FirstOrDefault(value => value.Key.ToLower() == each.Name.ToLower());
                    orderedValues.Add(keyvalue.Value);
                }
            }

            return Invoke(target, orderedValues);
        }

        public async Task InvokeAsync(object target, Dictionary<string, object> values) {
            var task = Invoke(target, values) as Task;

            if (task != null) {
                await task;
            }
        }

        private object ConvertType(Type type, object instance) {
            var converters = Converters.Where(converter => converter.CanConvert(type, instance)).ToList();

            if (converters.Any()) {
                foreach (var converter in converters) {
                    instance = converter.Convert(this, type, instance);
                }
            }

            return instance;
        }

        public object Invoke(object target, List<object> values) {
            var parameters = new List<object>();
            var index = 0;

            if (Method.GetParameters().Count() == 1 && Method.GetParameters().First().ParameterType.IsCollection()) {
                var type = Method.GetParameters().First().ParameterType.GetGenericArguments().First();
                var items = type.MakeList();

                foreach (var item in values) {
                    items.Add(ConvertType(type, item));
                }

                parameters.Add(items);
            }
            else {
                foreach (var each in Method.GetParameters()) {
                    if (values.Count <= index) {
                        if (each.IsOptional) {
                            parameters.Add(each.DefaultValue);
                            index += 1;
                            continue;
                        }

                        throw new Exception("Event is missing parameter: " + each.Name);
                    }

                    var value = values[index];

                    if (value != null) {
                        value = ConvertValue(value, each.ParameterType);
                    }

                    parameters.Add(value);
                    index += 1;
                }
            }

            return Method.Invoke(target, parameters.ToArray());
        }

        private object ConvertValue(object value, Type targetType) {
            var converters = Converters.Where(converter => converter.CanConvert(targetType, value)).ToList();

            if (converters.Any()) {
                foreach (var converter in converters) {
                    value = converter.Convert(this, targetType, value);
                }
            }
            else {
                if (!targetType.IsNullableType() && !targetType.IsCollection()) {
                    // Coerce the type (In case the serialized type is different from the parameter type)
                    value = Convert.ChangeType(value, targetType);
                }
                else if (targetType.IsCollection()) {
                    var enumerable = value as IEnumerable;

                    if (targetType.IsArray && enumerable != null) {
                        var type = targetType.GetElementType();
                        var result = type.MakeList();

                        foreach (var item in enumerable) {
                            result.Add(ConvertValue(item, type));
                        }

                        value = result.ToArray(type);
                    }
                }
            }

            return value;
        }

        public MethodInfo Method { get; set; }

        private List<IConverter> Converters { get; set; }
    }
}