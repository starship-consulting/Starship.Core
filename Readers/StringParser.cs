using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Starship.Core.Attributes;

namespace Starship.Core.Readers {
    public class StringParser {
        public StringParser(string text) {
            Text = text;
        }

        public List<T> DeserializeList<T>(int size) where T : new() {
            var results = new List<T>();

            while (Index + size <= Text.Length) {
                results.Add(Deserialize<T>());
                Index += size;
            }
            
            return results;
        }
        
        public T Deserialize<T>() where T : new() {
            var result = new T();

            foreach (var property in typeof(T).GetProperties()) {
                var attribute = property.GetCustomAttributes<ParseAttribute>().FirstOrDefault();

                if (attribute != null) {
                    property.SetValue(result, Text.Substring(attribute.Index - 1 + Index, attribute.Length));
                }
            }

            return result;
        }
        
        public string Text { get; set; }

        public int Index { get; set; }
    }
}